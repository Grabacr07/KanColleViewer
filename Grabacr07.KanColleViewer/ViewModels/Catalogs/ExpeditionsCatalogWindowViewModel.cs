using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class ExpeditionsCatalogWindowViewModel : WindowViewModel
	{
		private List<ExpeditionData> _ExpeditionList;
		public List<ExpeditionData> ExpeditionList
		{
			get { return this._ExpeditionList; }
			set
			{
				if (this._ExpeditionList != value)
				{
					this._ExpeditionList = value;
					this.RaisePropertyChanged();
				}
			}
		}
		private bool _IsPerHour;
		public bool IsPerHour
		{
			get { return this._IsPerHour; }
			set
			{
				if (this._IsPerHour == value) return;
				this._IsPerHour = value;
				this.Update(value);
				this.RaisePropertyChanged();
			}
		}

		private int ListCount { get; set; }
		public ExpeditionsCatalogWindowViewModel()
		{
			this.Title = "원정 공략표";
			this.ListCount = 0;
			this.Update();
		}
		private decimal ConvertToDecimal(string context)
		{
			if (context == string.Empty) return 0;
			return Convert.ToDecimal(context);
		}
		private void Update(bool IsChecked = false)
		{
			var tempList = new List<ExpeditionData>();
			ListCount = KanColleClient.Current.Translations.GetExpeditionListCount();

			bool IsEnd = true;
			int i = 1;
			while (IsEnd)
			{
				ExpeditionData temp = new ExpeditionData
				{
					ID = i,
					TRName = KanColleClient.Current.Translations.GetExpeditionData("TR-Name", i),
					FlagLv = KanColleClient.Current.Translations.GetExpeditionData("FlagLv", i),
					NeedShip = KanColleClient.Current.Translations.GetExpeditionData("NeedShip", i),
					Time = KanColleClient.Current.Translations.GetExpeditionData("Time", i),
					Fuel = ConvertToDecimal(KanColleClient.Current.Translations.GetExpeditionData("Fuel", i)),
					Armo = ConvertToDecimal(KanColleClient.Current.Translations.GetExpeditionData("Armo", i)),
					Metal = ConvertToDecimal(KanColleClient.Current.Translations.GetExpeditionData("Metal", i)),
					Bo = ConvertToDecimal(KanColleClient.Current.Translations.GetExpeditionData("Bo", i)),
					Detail = KanColleClient.Current.Translations.GetExpeditionData("Detail", i),
				};
				if (temp.FlagLv != string.Empty && temp.FlagLv != "-") temp.FlagLv = "Lv. " + temp.FlagLv;
				if (temp.Time != string.Empty)
				{
					var splitTime = temp.Time.Split(';');
					DateTime Time = new DateTime();

					if (splitTime[0] != string.Empty)
					{
						Time = Time.AddHours(Convert.ToDouble(splitTime[0]));
					}
					if (splitTime[1] != string.Empty)
					{
						Time = Time.AddMinutes(Convert.ToDouble(splitTime[1]));
					}
					temp.RealTime = Time;
					StringBuilder timeString = new StringBuilder();
					if (Time.Day != 1) timeString.Append(Time.Day - 1 + "일");
					if (Time.Hour != 0) timeString.Append(Time.Hour + "시간");
					if (Time.Minute != 0) timeString.Append(Time.Minute + "분");
					temp.Time = timeString.ToString();
					if (IsChecked)
					{
						decimal PerHour = Convert.ToDecimal((temp.RealTime.Day - 1) * 24 * 60 + temp.RealTime.Hour * 60 + temp.RealTime.Minute) / 60m;
						if (temp.Fuel > 0)
							temp.Fuel = Math.Round(temp.Fuel / PerHour, 1,MidpointRounding.AwayFromZero);
						if (temp.Armo > 0)
							temp.Armo = Math.Round(temp.Armo / PerHour, 1,MidpointRounding.AwayFromZero);
						if (temp.Metal > 0)
							temp.Metal = Math.Round(temp.Metal / PerHour, 1,MidpointRounding.AwayFromZero);
						if (temp.Bo > 0)
							temp.Bo = Math.Round(temp.Bo / PerHour, 1,MidpointRounding.AwayFromZero);
					}

				}
				i++;
				if (temp.TRName != string.Empty && temp.FlagLv != string.Empty) tempList.Add(temp);
				if (tempList.Count == this.ListCount) IsEnd = false;
			}
			this.ExpeditionList = new List<ExpeditionData>(tempList);
		}
	}
	public class ExpeditionData
	{
		public int ID { get; set; }
		public string TRName { get; set; }
		public string FlagLv { get; set; }
		public string NeedShip { get; set; }
		public string Time { get; set; }
		public DateTime RealTime { get; set; }
		public decimal Fuel { get; set; }
		public decimal Armo { get; set; }
		public decimal Metal { get; set; }
		public decimal Bo { get; set; }
		public string Detail { get; set; }
	}
}
