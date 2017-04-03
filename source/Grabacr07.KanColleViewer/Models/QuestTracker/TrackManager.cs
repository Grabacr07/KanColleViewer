using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Livet;

using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;

using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.QuestTracker.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Model;
using Grabacr07.KanColleViewer.QuestTracker.Models.EventArgs;

namespace Grabacr07.KanColleViewer.Models.QuestTracker
{
	internal class TrackManager
	{
		private string TrackerNamespace => "Grabacr07.KanColleViewer.QuestTracker.Models.Tracker";
		private readonly Dictionary<int, DateTime> trackingTime = new Dictionary<int, DateTime>();
		private static DateTime TokyoDateTime => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Tokyo Standard Time");

		private KanColleViewer.QuestTracker.Models.TrackManager trackManager;
		public event EventHandler QuestsEventChanged
		{
			add { trackManager.QuestsEventChanged += value; }
			remove { trackManager.QuestsEventChanged -= value; }
		}

		public List<ITracker> TrackingQuests => trackManager?.TrackingQuests;
		public List<ITracker> AllQuests => trackManager?.AllQuests;

		public TrackManager()
		{
			trackManager = new KanColleViewer.QuestTracker.Models.TrackManager(() => KanColleSettings.UseQuestTracker);

			var trackers = trackManager.Assembly.GetTypes()
					.Where(x => (x.Namespace?.StartsWith(TrackerNamespace) ?? false) && typeof(ITracker).IsAssignableFrom(x));

			foreach (var tracker in trackers)
			{
				try { trackManager?.trackingAvailable.Add((ITracker)Activator.CreateInstance(tracker)); }
				catch { }
			}

			this.QuestsEventChanged += (s, e) => WriteToStorage();
			ReadFromStorage();
			WriteToStorage();

			var quests = KanColleClient.Current.Homeport.Quests;
			quests.PropertyChanged += (s, e) => {
				if (e.PropertyName == nameof(quests.All))
					new System.Threading.Thread(ProcessQuests).Start();
			};

		}
		private void ProcessQuests()
		{
			var quests = KanColleClient.Current.Homeport.Quests;
			if (quests.All == null || quests.All.Count == 0) return;

			foreach (var quest in quests.All)
			{
				var tracker = trackManager?.trackingAvailable.Where(t => t.Id == quest.Id);
				if (!tracker.Any()) continue; // 추적할 수 없는 임무

				try
				{
					// 만료된 경우 (임무가 갱신되었다던가)
					if (trackingTime.ContainsKey(quest.Id) && !IsTrackingAvailable(quest.Type, trackingTime[quest.Id]))
					{
						// 임무 초기화
						if (trackingTime.ContainsKey(quest.Id)) trackingTime.Remove(quest.Id); // 추적중이었으면 추적 시작시간 제거
						tracker.First().ResetQuest();
					}

					switch (quest.State)
					{
						case QuestState.None:
							tracker.First().IsTracking = false;
							break;

						case QuestState.TakeOn:
							tracker.First().IsTracking = true; // quest taking

							// 임무 추적 시작시간 등록
							if (!trackingTime.ContainsKey(quest.Id))
								trackingTime.Add(quest.Id, TrackManager.TokyoDateTime);
							break;

						case QuestState.Accomplished:
							tracker.First().IsTracking = true;
							break;
					}
				}
				catch { }
			}

			trackManager?.RefreshTrackers();
			WriteToStorage();
		}
		private bool IsTrackingAvailable(QuestType type, DateTime time)
		{
			// 임무는 오전 5시, UTC+4 기준 0시에 갱신됨
			// 일광절약 없는 아랍 에미레이트 연합 표준시 (ar-AE) => UTC+4

			if (time == DateTime.MinValue)
				return false;

			var no = TrackManager.TokyoDateTime.AddHours(-5);
			time = time.AddHours(-5);

			switch (type)
			{
				case QuestType.OneTime:
				case QuestType.Other:
					return true;

				case QuestType.Daily:
					return time.Date == no.Date;

				case QuestType.Weekly:
					var cal = CultureInfo.CreateSpecificCulture("ar-AE").Calendar;
					var w_time = cal.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
					var w_now = cal.GetWeekOfYear(no, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

					return w_time == w_now && time.Year == no.Year;

				case QuestType.Monthly:
					return time.Month == no.Month && time.Year == no.Year;

				default:
					return false;
			}
		}

		private void WriteToStorage()
		{
			var baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			var list = new List<StorageData>();

			foreach (var tracker in trackManager?.trackingAvailable)
			{
				var item = new StorageData();

				DateTime dateTime = TrackManager.TokyoDateTime;
				trackingTime.TryGetValue(tracker.Id, out dateTime);

				try
				{
					if (tracker.GetProgress() == 0 || dateTime == DateTime.MinValue) continue;

					item.Id = tracker.Id;
					item.TrackTime = dateTime;
					item.Type = tracker.Type;
					item.Serialized = tracker.SerializeData();
					list.Add(item);
				}
				catch { }
			}

			string path = Path.Combine(baseDir, "TrackingQuest.csv");
			try
			{
				using (FileStream fs = new FileStream(path, FileMode.Create))
				{
					foreach (var item in list)
					{
						try {
							CSV.Write(
								fs,
								item.Id, item.TrackTime, item.Type,
								item.Serialized,
								KanColleClient.Current.Homeport.Quests.All
									.FirstOrDefault(x => x.Id == item.Id)
									.Title
							);
						}
						catch { }
					}

					fs.Flush();
				}
			}
			catch { }
		}
		private void ReadFromStorage()
		{
			var baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			string path = Path.Combine(baseDir, "TrackingQuest.csv");
			if (!File.Exists(path)) return;

			try
			{
				using (FileStream fs = new FileStream(path, FileMode.Open))
				{
					while (fs.Position < fs.Length)
					{
						string[] data = CSV.Read(fs);

						try
						{
							int Id;
							DateTime trackTime;
							string QuestTypeText;
							QuestType QuestType;
							string Serialized;

							if (!int.TryParse(data[0], out Id)) continue;
							DateTime.TryParse(data[1], out trackTime);
							QuestTypeText = data[2];
							Enum.TryParse<QuestType>(QuestTypeText, out QuestType);
							Serialized = data[3];

							if (!(trackManager?.trackingAvailable.Any(x => x.Id == Id) ?? false)) continue;
							if (IsTrackingAvailable(QuestType, trackTime))
							{
								var tracker = trackManager?.trackingAvailable.Where(x => x.Id == Id).First();

								trackingTime.Add(Id, trackTime);
								// tracker.IsTracking = true;
								tracker.DeserializeData(Serialized);
							}
						}
						catch { }
					}
				}
			}
			catch { }
		}

		public void CallCheckOverUnder(IdProgressPair[] questList)
		{
			foreach (var x in questList)
			{
				var z = this.TrackingQuests.FirstOrDefault(y => y.Id == x.Id);
				if (z == null) continue;

				z.CheckOverUnder(x.Progress);
			}
		}
	}
}
