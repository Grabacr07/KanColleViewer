using Grabacr07.KanColleWrapper;
using System.Collections.Generic;

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
					if (this._ExpeditionList.Count == this.ListCount)
						this.RaisePropertyChanged();
				}
			}
		}
		private int ListCount { get; set; }
		public ExpeditionsCatalogWindowViewModel()
		{
			this.Title = "원정 공략표";
			this.ListCount = 0;
			this.Update();
		}
		private void Update()
		{
			this.ExpeditionList = new List<ExpeditionData>();
			ListCount = KanColleClient.Current.Translations.GetExpeditionListCount();

			bool IsEnd = true;
			int i = 1;
			while (IsEnd)
			{
				ExpeditionData temp = new ExpeditionData
				{
					ID = i.ToString(),
					TRName = KanColleClient.Current.Translations.GetExpeditionData("TR-Name", i),
					FlagLv = KanColleClient.Current.Translations.GetExpeditionData("FlagLv", i),
					NeedShip = KanColleClient.Current.Translations.GetExpeditionData("NeedShip", i),
					Time = KanColleClient.Current.Translations.GetExpeditionData("Time", i),
					Fuel = KanColleClient.Current.Translations.GetExpeditionData("Fuel", i),
					Armo = KanColleClient.Current.Translations.GetExpeditionData("Armo", i),
					Metal = KanColleClient.Current.Translations.GetExpeditionData("Metal", i),
					Bo = KanColleClient.Current.Translations.GetExpeditionData("Bo", i),
					Detail = KanColleClient.Current.Translations.GetExpeditionData("Detail", i),
				};
				i++;
				if (temp.TRName != string.Empty && temp.FlagLv != string.Empty) this.ExpeditionList.Add(temp);
				if (this.ExpeditionList.Count == this.ListCount) IsEnd = false;
			}
		}
	}
	public class ExpeditionData
	{

		#region ID変更通知プロパティ
		private string _ID;

		public string ID
		{
			get
			{ return this._ID; }
			set
			{
				if (this._ID == value)
					return;
				this._ID = value;
			}
		}
		#endregion

		#region TRName変更通知プロパティ
		private string _TRName;

		public string TRName
		{
			get
			{ return this._TRName; }
			set
			{
				if (this._TRName == value)
					return;
				this._TRName = value;
			}
		}
		#endregion

		#region FlagLv変更通知プロパティ
		private string _FlagLv;

		public string FlagLv
		{
			get
			{ return this._FlagLv; }
			set
			{
				if (this._FlagLv == value)
					return;
				this._FlagLv = value;
			}
		}
		#endregion

		#region NeedShip変更通知プロパティ
		private string _NeedShip;

		public string NeedShip
		{
			get
			{ return this._NeedShip; }
			set
			{
				if (this._NeedShip == value)
					return;
				this._NeedShip = value;
			}
		}
		#endregion

		#region Time変更通知プロパティ
		private string _Time;

		public string Time
		{
			get
			{ return this._Time; }
			set
			{
				if (this._Time == value)
					return;
				this._Time = value;
			}
		}
		#endregion

		#region Fuel変更通知プロパティ
		private string _Fuel;

		public string Fuel
		{
			get
			{ return this._Fuel; }
			set
			{
				if (this._Fuel == value)
					return;
				this._Fuel = value;
			}
		}
		#endregion

		#region Armo変更通知プロパティ
		private string _Armo;

		public string Armo
		{
			get
			{ return this._Armo; }
			set
			{
				if (this._Armo == value)
					return;
				this._Armo = value;
			}
		}
		#endregion

		#region Metal変更通知プロパティ
		private string _Metal;

		public string Metal
		{
			get
			{ return this._Metal; }
			set
			{
				if (this._Metal == value)
					return;
				this._Metal = value;
			}
		}
		#endregion

		#region Bo変更通知プロパティ
		private string _Bo;

		public string Bo
		{
			get
			{ return this._Bo; }
			set
			{
				if (this._Bo == value)
					return;
				this._Bo = value;
			}
		}
		#endregion

		#region Detail変更通知プロパティ
		private string _Detail;

		public string Detail
		{
			get
			{ return this._Detail; }
			set
			{
				if (this._Detail == value)
					return;
				this._Detail = value;
			}
		}
		#endregion

	}
}
