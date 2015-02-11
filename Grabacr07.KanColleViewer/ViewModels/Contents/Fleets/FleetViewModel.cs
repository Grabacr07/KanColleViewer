using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	/// <summary>
	/// 単一の艦隊情報を提供します。
	/// </summary>
	public class FleetViewModel : ViewModel
	{
		private readonly Fleet source;

		public SortieViewModel Sortie { get; private set; }

		public ExpeditionViewModel Expedition { get; private set; }

		public HomeportViewModel Homeport { get; private set; }

		/// <summary>
		/// 艦隊に所属している艦娘のコレクションを取得します。
		/// </summary>
		public ShipViewModel[] Ships
		{
			get { return this.source.Ships.Select(x => new ShipViewModel(x)).ToArray(); }
		}

		/// <summary>
		/// 艦隊の状態を取得します。
		/// </summary>
		public ViewModel State
		{
			get
			{
				switch (this.source.State)
				{
					case FleetState.Empty:
						return NullViewModel.Instance;

					case FleetState.Sortie:
						return this.Sortie;

					case FleetState.Expedition:
						return this.Expedition;

					default:
						return this.Homeport;
				}
			}
		}

		#region IsSelected 変更通知プロパティ

		private bool _IsSelected;

		public bool IsSelected
		{
			get { return this._IsSelected; }
			set
			{
				if (this._IsSelected != value)
				{
					this._IsSelected = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region wrapper properties

		public int Id
		{
			get { return this.source.Id; }
		}

		public string Name
		{
			get { return string.IsNullOrEmpty(this.source.Name.Trim()) ? "(第 " + this.source.Id + " 艦隊)" : this.source.Name; }
		}

		public string TotalLevel
		{
			get { return this.source.TotalLevel.ToString("####"); }
		}

		public string AverageLevel
		{
			get { return this.source.AverageLevel.ToString("##.##"); }
		}

		public string Speed
		{
			get { return this.source.Speed == KanColleWrapper.Models.Speed.Fast ? Resources.Fleets_Speed_Fast : Resources.Fleets_Speed_Slow; }
		}

		public int AirSuperiorityPotential
		{
			get { return this.source.AirSuperiorityPotential; }
		}

		public string TotalViewRange
		{
			get { return this.source.TotalViewRange.ToString("####"); }
		}

		#endregion


		public FleetViewModel(Fleet fleet)
		{
			this.source = fleet;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
				{ () => fleet.Ships, (sender, args) => this.RaisePropertyChanged("Planes") },
			});

			this.Sortie = new SortieViewModel(fleet);
			this.CompositeDisposable.Add(this.Sortie);

			this.Expedition = new ExpeditionViewModel(fleet.Expedition);
			this.CompositeDisposable.Add(this.Expedition);

			this.Homeport = new HomeportViewModel(fleet);
			this.CompositeDisposable.Add(this.Homeport);
		}
	}
}
