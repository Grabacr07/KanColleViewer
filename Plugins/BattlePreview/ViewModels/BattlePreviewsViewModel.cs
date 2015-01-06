using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using System;
using System.Collections.Generic;
using System.Windows;

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
