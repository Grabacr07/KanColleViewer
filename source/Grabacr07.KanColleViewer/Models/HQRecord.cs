using Grabacr07.KanColleWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
	public class HQRecord
	{
		public class HQRecordElement
		{
			/// <summary>
			/// 기록 시간
			/// </summary>
			public DateTime Date;

			/// <summary>
			/// 사령부 Lv
			/// </summary>
			public int HQLevel;

			/// <summary>
			/// 제독 Exp
			/// </summary>
			public int HQExp;

			public HQRecordElement()
			{
				Date = DateTime.Now;
			}

			public HQRecordElement(DateTime time, int level, int exp) : this()
			{
				Date = time;
				HQLevel = level;
				HQExp = exp;
			}

			public HQRecordElement(int level, int exp) : this()
			{
				HQLevel = level;
				HQExp = exp;
			}
		}
		public const string RecordHeader = "#일시,사령부 Lv,제독 Exp";

		private List<HQRecordElement> Record;
		private DateTime _prevTime;
		private bool _initialFlag;

		public HQRecord()
		{
			Record = new List<HQRecordElement>();
			_prevTime = DateTime.Now;
			_initialFlag = true;
		}

		public void Updated()
		{
			if (_initialFlag || IsCrossedHour(_prevTime))
			{
				_prevTime = DateTime.Now;
				_initialFlag = false;

				var admiral = KanColleClient.Current.Homeport.Admiral;
				Record.Add(new HQRecordElement(
					admiral.Level,
					admiral.Experience));

				Save();
			}
		}

		/// <summary>
		/// HQ 데이터를 저장합니다.
		/// </summary>
		public void Save()
		{
			
			string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Record");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			path += @"\HQRecord.csv";

			bool exist = File.Exists(path);

			using (StreamWriter sw = new StreamWriter(path, false))
			{
				sw.WriteLine(RecordHeader);

				var list = new List<HQRecordElement>(Record);
				list.Sort((e1, e2) => e1.Date.CompareTo(e2.Date));

				foreach (var elem in list)
				{
					sw.WriteLine($"{TimeToCSVString(elem.Date)},{elem.HQLevel},{elem.HQExp}");
				}
			}
		}
		

		public void Load()
		{

			string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Record");
			path += @"\HQRecord.csv";
			
			if (File.Exists(path))
			{
				using (StreamReader sr = new StreamReader(path))
				{
					Record.Clear();

					string line;
					int linecount = 1;
					sr.ReadLine();          //ヘッダを読み飛ばす

					while ((line = sr.ReadLine()) != null)
					{
						if (line.Trim().StartsWith("#"))
							continue;

						string[] elem = line.Split(",".ToCharArray());

						Record.Add(new HQRecordElement(CSVStringToTime(elem[0]), int.Parse(elem[1]), int.Parse(elem[2])));

						linecount++;
					}

				}
			}
		}

		/// <summary>
		/// 指定した日時以降の最も古い記録を返します。
		/// </summary>
		public HQRecordElement GetRecord(DateTime target)
		{

			int i;
			for (i = Record.Count - 1; i >= 0; i--)
			{
				if (Record[i].Date < target)
				{
					i++;
					break;
				}
			}
			// Record内の全ての記録がtarget以降だった
			if (i < 0)
				i = 0;

			if (0 <= i && i < Record.Count)
			{
				return Record[i];
			}
			else {
				return null;
			}
		}

		/// <summary>
		/// 前回の戦果更新以降の最も古い記録を返します。
		/// </summary>
		public HQRecordElement GetRecordPrevious()
		{

			DateTime now = DateTime.Now;
			DateTime target;
			if (now.TimeOfDay.Hours < 2)
			{
				target = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0).Subtract(TimeSpan.FromDays(1));
			}
			else if (now.TimeOfDay.Hours < 14)
			{
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
			}
			else {
				target = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
			}

			return GetRecord(target);
		}

		/*/// <summary>
		/// 今日の戦果更新以降の最も古い記録を返します。
		/// </summary>
		public HQRecordElement GetRecordDay()
		{

			DateTime now = DateTime.Now;
			DateTime target;
			if (now.TimeOfDay.Hours < 2)
			{
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0).Subtract(TimeSpan.FromDays(1));
			}
			else {
				target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
			}

			return GetRecord(target);
		}*/

		/// <summary>
		/// 今月の戦果更新以降の最も古い記録を返します。
		/// </summary>
		public HQRecordElement GetRecordMonth()
		{
			DateTime now = DateTime.Now;

			return GetRecord(new DateTime(now.Year, now.Month, 1));
		}

		/// <summary>
		/// 毎時0分をまたいだかを取得します。
		/// </summary>
		/// <param name="prev">前回処理したときの日時。</param>
		/// <returns>またいでいるか。</returns>
		public static bool IsCrossedHour(DateTime prev)
		{

			DateTime nexthour = prev.Date.AddHours(prev.Hour + 1);
			return nexthour <= DateTime.Now;
		}

		public static string TimeToCSVString(DateTime time)
		{
			return time.ToString("yyyy\\/MM\\/dd HH\\:mm\\:ss", System.Globalization.CultureInfo.InvariantCulture);
		}

		public static DateTime CSVStringToTime(string str)
		{
			string[] elem = str.Split("/ :".ToCharArray());

			// Excel様が *うっかり* データを破損させることがあるので対応
			return new DateTime(
				elem.Length > 0 ? int.Parse(elem[0]) : 1970,
				elem.Length > 1 ? int.Parse(elem[1]) : 1,
				elem.Length > 2 ? int.Parse(elem[2]) : 1,
				elem.Length > 3 ? int.Parse(elem[3]) : 0,
				elem.Length > 4 ? int.Parse(elem[4]) : 0,
				elem.Length > 5 ? int.Parse(elem[5]) : 0);
		}
	}
}
