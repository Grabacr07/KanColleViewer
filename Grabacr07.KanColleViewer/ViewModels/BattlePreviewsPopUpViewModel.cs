using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class BattlePreviewsPopUpViewModel : WindowViewModel
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

		#region RankOuts 変更通知プロパティ

		private RankResult _RankOuts;

		public RankResult RankOuts
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

		#region PerfectRank 変更通知プロパティ

		private Visibility _PerfectRank;

		public Visibility PerfectRank
		{
			get { return this._PerfectRank; }
			set
			{
				if (this._PerfectRank != value)
				{
					this._PerfectRank = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RankS 変更通知プロパティ

		private Visibility _RankS;

		public Visibility RankS
		{
			get { return this._RankS; }
			set
			{
				if (this._RankS != value)
				{
					this._RankS = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RankA 変更通知プロパティ

		private Visibility _RankA;

		public Visibility RankA
		{
			get { return this._RankA; }
			set
			{
				if (this._RankA != value)
				{
					this._RankA = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RankB 変更通知プロパティ

		private Visibility _RankB;

		public Visibility RankB
		{
			get { return this._RankB; }
			set
			{
				if (this._RankB != value)
				{
					this._RankB = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RankC 変更通知プロパティ

		private Visibility _RankC;

		public Visibility RankC
		{
			get { return this._RankC; }
			set
			{
				if (this._RankC != value)
				{
					this._RankC = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RankD 変更通知プロパティ

		private Visibility _RankD;

		public Visibility RankD
		{
			get { return this._RankD; }
			set
			{
				if (this._RankD != value)
				{
					this._RankD = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region RankOut 変更通知プロパティ

		private Visibility _RankOut;

		public Visibility RankOut
		{
			get { return this._RankOut; }
			set
			{
				if (this._RankOut != value)
				{
					this._RankOut = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsCalculated 変更通知プロパティ

		private bool _IsCalculated;

		public bool IsCalculated
		{
			get { return this._IsCalculated; }
			set
			{
				if (this._IsCalculated != value)
				{
					this._IsCalculated = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		private void RankIntToVisibility(int value)
		{
			switch (value)
			{
				case 0:
					PerfectRank = Visibility.Visible;
					RankS = Visibility.Hidden;
					RankA = Visibility.Hidden;
					RankB = Visibility.Hidden;
					RankC = Visibility.Hidden;
					RankD = Visibility.Hidden;
					RankOut = Visibility.Hidden;

					break;
				case 1:
					PerfectRank = Visibility.Hidden;
					RankS = Visibility.Visible;
					RankA = Visibility.Hidden;
					RankB = Visibility.Hidden;
					RankC = Visibility.Hidden;
					RankD = Visibility.Hidden;
					RankOut = Visibility.Hidden;

					break;
				case 2:
					PerfectRank = Visibility.Hidden;
					RankS = Visibility.Hidden;
					RankA = Visibility.Visible;
					RankB = Visibility.Hidden;
					RankC = Visibility.Hidden;
					RankD = Visibility.Hidden;
					RankOut = Visibility.Hidden;

					break;
				case 3:
					PerfectRank = Visibility.Hidden;
					RankS = Visibility.Hidden;
					RankA = Visibility.Hidden;
					RankB = Visibility.Visible;
					RankC = Visibility.Hidden;
					RankD = Visibility.Hidden;
					RankOut = Visibility.Hidden;

					break;
				case 4:
					PerfectRank = Visibility.Hidden;
					RankS = Visibility.Hidden;
					RankA = Visibility.Hidden;
					RankB = Visibility.Hidden;
					RankC = Visibility.Visible;
					RankD = Visibility.Hidden;
					RankOut = Visibility.Hidden;

					break;
				case 5:
					PerfectRank = Visibility.Hidden;
					RankS = Visibility.Hidden;
					RankA = Visibility.Hidden;
					RankB = Visibility.Hidden;
					RankC = Visibility.Hidden;
					RankD = Visibility.Visible;
					RankOut = Visibility.Hidden;

					break;
				case -1:
					PerfectRank = Visibility.Hidden;
					RankS = Visibility.Hidden;
					RankA = Visibility.Hidden;
					RankB = Visibility.Hidden;
					RankC = Visibility.Hidden;
					RankD = Visibility.Hidden;
					RankOut = Visibility.Visible;

					break;
			}
		}


		public BattlePreviewsPopUpViewModel()
		{
			this.Title = "전투예측 윈도우";

			PerfectRank = Visibility.Hidden;
			RankS = Visibility.Hidden;
			RankA = Visibility.Hidden;
			RankB = Visibility.Hidden;
			RankC = Visibility.Hidden;
			RankD = Visibility.Hidden;
			RankOut = Visibility.Hidden;

			this.IsCalculated = KanColleClient.Current.PreviewBattle.IsCalculated;

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
						if (!KanColleClient.Current.PreviewBattle.Combined) this.KanResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.KanResult());
						else
						{
							this.KanResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.KanResult(1));
							this.SecondResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.SecondResult());
						}
						this.EnemyResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.EnemyResult());

						this.RankOuts = KanColleClient.Current.PreviewBattle.RankOut();
						this.RankIntToVisibility(this.RankOuts.RankNum);
					};
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}

		}
		public void Update()
		{
			this.RewriteFleetStatus();
		}
		private void RewriteFleetStatus()
		{
			try
			{
				if (KanColleClient.Current.PreviewBattle.EnableBattlePreview)
				{
					if (!KanColleClient.Current.PreviewBattle.Combined) this.KanResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.KanResult());
					else
					{
						this.KanResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.KanResult(1));
						this.SecondResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.SecondResult());
					}
					this.EnemyResultReport = new List<PreviewBattleResults>(KanColleClient.Current.PreviewBattle.EnemyResult());

					this.RankOuts = KanColleClient.Current.PreviewBattle.RankOut();
					this.RankIntToVisibility(this.RankOuts.RankNum);
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}

		}

	}
}
