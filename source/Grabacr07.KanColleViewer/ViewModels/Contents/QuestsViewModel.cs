using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;

using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Models.QuestTracker;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using IdProgressPair =  Grabacr07.KanColleViewer.QuestTracker.Models.Model.IdProgressPair;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class QuestsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.Quests; }
			protected set { throw new NotImplementedException(); }
		}

		private int? _badge = null;

		public QuestViewModel[] Current => this.OriginalQuests.Where(x => x.State != QuestState.None).ToArray();

		#region Quests 変更通知プロパティ

		/// <summary>
		/// Filtered Quest List
		/// </summary>
		private QuestViewModel[] _OriginalQuests;
		private QuestViewModel[] OriginalQuests
		{
			get { return this._OriginalQuests; }
			set
			{
				if (this._OriginalQuests != value)
				{
					this._OriginalQuests = value;
					this.RaisePropertyChanged("Quests");
					this.RaisePropertyChanged("Current");
				}
			}
		}

		/// <summary>
		/// Original Quest List
		/// </summary>
		public QuestViewModel[] Quests
		{
			get
			{
				IEnumerable<QuestViewModel> temp;
				switch (SelectedItem.Key)
				{
					case "All":
						temp = OriginalQuests;
						break;
					case "Current":
						temp = OriginalQuests.Where(x => x.State != QuestState.None);
						break;
					case "Daily":
						temp = OriginalQuests.Where(x => x.Type == QuestType.Daily);
						break;
					case "Weekly":
						temp = OriginalQuests.Where(x => x.Type == QuestType.Weekly);
						break;
					case "Monthly":
						temp = OriginalQuests.Where(x => x.Type == QuestType.Monthly);
						break;
					case "Once":
						temp = OriginalQuests.Where(x => x.Type == QuestType.OneTime);
						break;
					case "Others":
						temp = OriginalQuests.Where(x => x.Type == QuestType.Other);
						break;
					default:
						temp = this.OriginalQuests = new QuestViewModel[0];
						break;
				}
				if (QuestCategorySelected.Display != CategoryToColor(QuestCategory.Other))
					temp = temp.Where(x => CategoryToColor(x.Category) == QuestCategorySelected.Display);

				return ComputeQuestPage(temp.ToArray());
			}
		}

		#endregion

		#region IsUntaken 変更通知プロパティ

		private bool _IsUntaken;

		public bool IsUntaken
		{
			get { return this._IsUntaken; }
			set
			{
				if (this._IsUntaken != value)
				{
					this._IsUntaken = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsEmpty 変更通知プロパティ

		private bool _IsEmpty;

		public bool IsEmpty
		{
			get { return this._IsEmpty; }
			set
			{
				if (this._IsEmpty != value)
				{
					this._IsEmpty = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		#region QuestCategories 프로퍼티

		public ICollection<QuestCategoryViewModel> QuestCategories { get; }

		#endregion

		#region QuestCategorySelected 변경통지 프로퍼티

		private QuestCategoryViewModel _QuestCategorySelected;
		public QuestCategoryViewModel QuestCategorySelected
		{
			get { return this._QuestCategorySelected; }
			set
			{
				if (this._QuestCategorySelected != value)
				{
					this._QuestCategorySelected = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("Quests");
				}
			}
		}

		#endregion

		public IList<KeyNameTabItemViewModel> TabItems { get; set; }

		private KeyNameTabItemViewModel _SelectedItem;
		public KeyNameTabItemViewModel SelectedItem
		{
			get { return this._SelectedItem; }
			set
			{
				if (this._SelectedItem != value)
				{
					this._SelectedItem = value;
					this.RaisePropertyChanged("Quests");
					this.RaisePropertyChanged("SelectedItem");
				}
			}
		}


		private TrackManager questTracker { get; set; }

		public FleetsViewModel Fleets { get; }

		public QuestsViewModel(FleetsViewModel fleets)
		{
			this.TabItems = new List<KeyNameTabItemViewModel>
				{
					new KeyNameTabItemViewModel("All", "전체"),
					new KeyNameTabItemViewModel("Current", "진행중"),
					new KeyNameTabItemViewModel("Daily", "일일"),
					new KeyNameTabItemViewModel("Weekly", "주간"),
					new KeyNameTabItemViewModel("Monthly", "월간"),
					new KeyNameTabItemViewModel("Once", "일회성"),
					new KeyNameTabItemViewModel("Others", "그 외")
				};
			this.SelectedItem = this.TabItems.FirstOrDefault(x => x.Name == "진행중");


			var categoryTable = new Dictionary<string, string>
			{
				{ "Composition", "편성" },
				{ "Sortie", "출격" },
				{ "Sortie2", "출격" },
				{ "Practice", "연습" },
				{ "Expeditions", "원정" },
				{ "Supply", "보급" },
				{ "Building", "공창" },
				{ "Remodelling", "근개수" },
			};

			var list = new List<QuestCategoryViewModel>();
			var categories = Enum.GetNames(typeof(QuestCategory));
			list.Add(new QuestCategoryViewModel("All", CategoryToColor(QuestCategory.Other), "전체"));

			foreach (var item in categories)
			{
				var t = (QuestCategory)Enum.Parse(typeof(QuestCategory), item);
				list.Add(new QuestCategoryViewModel(item, CategoryToColor(t), categoryTable.ContainsKey(item) ? categoryTable[item] : ""));
			}
			list = list.Distinct(x => x.Display).ToList();

			this.QuestCategories = list;
			this.QuestCategorySelected = this.QuestCategories.FirstOrDefault();


			var quests = KanColleClient.Current.Homeport.Quests;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(quests)
			{
				{ nameof(quests.All), (sender, args) => this.UpdateQuest(quests) },
			});
			
			// set Quest Tarcker
			questTracker = new TrackManager();
			questTracker.QuestsEventChanged += (s, e) => this.UpdateQuest(quests, true);


			KanColleSettings.ShowQuestBadge.ValueChanged += (s, e) => this.UpdateBadge();
			this.UpdateQuest(quests);

			this.Fleets = fleets;
		}

		private void UpdateBadge()
		{
			if (KanColleSettings.ShowQuestBadge)
			{
				this.Badge = _badge == 0 ? null : (int?)_badge;
			}
			else this.Badge = null;
		}

		private void UpdateQuest(Quests quests, bool fromTracker = false)
		{
			var viewList = quests.All
				.Select(x => new QuestViewModel(x))
				.Distinct(x => x.Id)
				.ToList();

			try
			{ // QuestTracker 어디서 문제가 생길지 모름
				questTracker.AllQuests.ForEach(x =>
				{
					var y = viewList.Where(z => z.Id == x.Id).FirstOrDefault();
					if (y == null) return;

					y.QuestProgressValue = x.GetProgress();
					y.QuestProgressText = x.GetProgressText();
				});
			}
			catch { }

			this.OriginalQuests = viewList.ToArray();
			this.IsEmpty = quests.IsEmpty;
			this.IsUntaken = quests.IsUntaken;

			if (!fromTracker) // Only updated from Quest screen
				questTracker.CallCheckOverUnder(
					OriginalQuests.Select(x => new IdProgressPair
					{
						Id = x.Id,
						Progress = x.Progress,
						State = x.State
					}).ToArray()
				);

			// Quests 멤버는 필터 적용된걸 get으로 반환해서 문제가 있음
			_badge = OriginalQuests.Count(x => x.QuestProgressValue == 100);
			this.UpdateBadge();
		}
		private QuestViewModel[] ComputeQuestPage(QuestViewModel[] inp)
		{
			if (inp.Length == 0) return inp;

			inp = inp
				.Select(x => { x.LastOnPage = false; return x; })
				.OrderBy(x => x.Page)
				.ThenBy(x => x.Id)
				.ToArray();

			int[] pages = inp.Select(x => x.Page)
				.Distinct()
				.ToArray();

			foreach (var page in pages)
				inp.Where(x => x.Page == page)
					.Last().LastOnPage = true;

			return inp.ToArray();
		}

		private string CategoryToColor(QuestCategory category)
		{
			switch (category)
			{
				case QuestCategory.Composition:
					return "#FF2A7D46";
				case QuestCategory.Sortie:
				case QuestCategory.Sortie2:
					return "#FFB53B36";
				case QuestCategory.Practice:
					return "#FF8DC660";
				case QuestCategory.Expeditions:
					return "#FF3BA09D";
				case QuestCategory.Supply:
					return "#FFB2932E";
				case QuestCategory.Building:
					return "#FF64443B";
				case QuestCategory.Remodelling:
					return "#FFA987BA";
			}
			return "#FF808080";
		}
	}

	public class QuestCategoryViewModel : Livet.ViewModel
	{
		public string Key { get; }
		public string Display { get; }
		public string DisplayText { get; }

		public QuestCategoryViewModel(string Key, string Display, string DisplayText = "")
		{
			this.Key = Key;
			this.Display = Display;
			this.DisplayText = DisplayText;
		}
	}
}
