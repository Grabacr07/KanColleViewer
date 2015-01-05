using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using System;
using System.Collections.Generic;

namespace Grabacr07.KanColleViewer.Plugins.ViewModels
{
	public class BattlePreviewsViewModel : ViewModel
	{
		#region ResultReport 変更通知プロパティ

		private List<PreviewBattleResults> _ResultReport;

		public List<PreviewBattleResults> ResultReport
		{
			get { return this._ResultReport; }
			set
			{
				if (this._ResultReport != value)
				{
					this._ResultReport = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsCombined 変更通知プロパティ

		private double _IsCombined;

		public double IsCombined
		{
			get { return this._IsCombined; }
			set
			{
				if (this._IsCombined != value)
				{
					this._IsCombined = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsRefreshed 変更通知プロパティ

		private double _IsRefreshed;

		public double IsRefreshed
		{
			get { return this._IsRefreshed; }
			set
			{
				if (this._IsRefreshed != value)
				{
					this._IsRefreshed = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Rank 変更通知プロパティ

		private string _Rank;

		public string Rank
		{
			get { return this._Rank; }
			set
			{
				if (this._Rank != value)
				{
					this._Rank = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public BattlePreviewsViewModel()
		{
			this.UpdateFleetStatus();
		}
		private void UpdateFleetStatus()
		{
			try
			{
				if (KanColleClient.Current.PreviewBattle.EnableBattlePreview)
				{
					KanColleClient.Current.PreviewBattle.PreviewCriticalCondition += () =>
					{
						this.ResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.TotalResult());
						this.Rank = this.ResultReport[6].Rank;

						//연합함대가 편성되었을때는 연합함대 항목을 확장시켜준다. 임시코드
						if (KanColleClient.Current.Homeport.Organization.Combined) 
						{
							this.IsCombined = 0.0;
							this.IsCombined = double.NaN;
						}
						else this.IsCombined = 0.0;

						//항목이 사라지는것에 대한 임시조치
						this.IsRefreshed = 0.0;
						this.IsRefreshed = double.NaN;
					};
				}
			}
			catch(Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}
		}

	}
}
