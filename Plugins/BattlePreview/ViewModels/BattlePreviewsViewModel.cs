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
		#region KanResultReport 変更通知プロパティ

		private List<PreviewBattleResults> _KanResultReport;

		public List<PreviewBattleResults> KanResultReport
		{
			get { return this._KanResultReport; }
			set
			{
				if (this._KanResultReport != value)
				{
					this._KanResultReport = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnemyResultReport 変更通知プロパティ

		private List<PreviewBattleResults> _EnemyResultReport;

		public List<PreviewBattleResults> EnemyResultReport
		{
			get { return this._EnemyResultReport; }
			set
			{
				if (this._EnemyResultReport != value)
				{
					this._EnemyResultReport = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SecondResultReport 変更通知プロパティ

		private List<PreviewBattleResults> _SecondResultReport;

		public List<PreviewBattleResults> SecondResultReport
		{
			get { return this._SecondResultReport; }
			set
			{
				if (this._SecondResultReport != value)
				{
					this._SecondResultReport = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CellStatus 変更通知プロパティ

		private int _CellStatus;

		public int CellStatus
		{
			get { return this._CellStatus; }
			set
			{
				if (this._CellStatus != value)
				{
					this._CellStatus = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RankOuts 変更通知プロパティ

		private int _RankOuts;

		public int RankOuts
		{
			get { return this._RankOuts; }
			set
			{
				if (this._RankOuts != value)
				{
					this._RankOuts = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public BattlePreviewsViewModel()
		{
			RankOuts = 6;

			this.UpdateFleetStatus();
		}
		private void UpdateFleetStatus()
		{
			try
			{
				if (KanColleClient.Current.OracleOfCompass.EnableBattlePreview)
				{
					KanColleClient.Current.OracleOfCompass.ReadyForNextCell += () =>
					{
						CellStatus = KanColleClient.Current.OracleOfCompass.CellData;
					};
					KanColleClient.Current.OracleOfCompass.PreviewCriticalCondition += () =>
					{
						if (!KanColleClient.Current.OracleOfCompass.Combined) this.KanResultReport = new List<PreviewBattleResults>(KanColleClient.Current.OracleOfCompass.KanResult());
						else
						{
							this.KanResultReport = new List<PreviewBattleResults>(KanColleClient.Current.OracleOfCompass.KanResult(1));
							this.SecondResultReport = new List<PreviewBattleResults>(KanColleClient.Current.OracleOfCompass.SecondResult());
						}
						this.EnemyResultReport = new List<PreviewBattleResults>(KanColleClient.Current.OracleOfCompass.EnemyResult());

						this.RankOuts = KanColleClient.Current.OracleOfCompass.RankResult;
					};
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}
		}

	}
}
