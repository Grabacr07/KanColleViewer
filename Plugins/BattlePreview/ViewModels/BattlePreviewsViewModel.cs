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


		public BattlePreviewsViewModel()
		{
			PerfectRank = Visibility.Hidden;
			RankS = Visibility.Hidden;
			RankA = Visibility.Hidden;
			RankB = Visibility.Hidden;
			RankC = Visibility.Hidden;
			RankD = Visibility.Hidden;
			RankOut = Visibility.Hidden;
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

						this.RankIntToVisibility(this.ResultReport[6].RankNum);
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
