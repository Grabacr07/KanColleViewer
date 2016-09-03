using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using System.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class CombinedFleetViewModel : ItemViewModel
	{
		public CombinedFleet Source { get; }

		public string Name => this.Source?.Name.Replace(Environment.NewLine, " + ") ?? "";

		public FleetStateViewModel State { get; }

		public ViewModel QuickStateView => this.Source.State.Situation.HasFlag(FleetSituation.Sortie)
			? this.State.Sortie
			: this.State.Homeport as QuickStateViewViewModel;

        /// <summary>
        /// 艦隊に所属している艦娘のコレクションを取得します。
        /// </summary>
        public FleetViewModel[] Fleets
        {
            get
            {
                FleetViewModel[] temps = this.Source.Fleets.Select(x => new FleetViewModel(x)).ToArray();
                return temps;
            }
        }

        #region 보급량

        public int UsedFuel => this.Source.Fleets
            .Select(z => z.Ships.Select(x => x.UsedFuel).Sum(x => x))
            .Sum(z => z);

        public int UsedAmmo => this.Source.Fleets
            .Select(z => z.Ships.Select(x => x.UsedBull).Sum(x => x))
            .Sum(z => z);

        public int UsedBauxite => this.Source.Fleets
            .Select(z => z.Ships
                .Select(x =>
                    x.Slots
                        .Where(y => y.Item.Info.IsNumerable)
                        .Select(y => y.Maximum - y.Current)
                        .Sum(y => y * 5)
                ).Sum(x => x)
            )
            .Sum(z => z);

        #endregion

        #region FleetViewModel과의 호환성
        public int Id => 1;
        public Expedition Expedition => this.Source?.Fleets[0].Expedition;
        public Visibility IsFirstFleet => Visibility.Collapsed;

        public Visibility vTotal => Visibility.Collapsed;
        public string TotalLv => "";
        public Visibility vFlag => Visibility.Collapsed;
        public string FlagLv => "";
        public Visibility vFlagType => Visibility.Collapsed;
        public string FlagType => "";
        public Visibility vNeed => Visibility.Collapsed;
        public string ShipTypeString => "";
        public Visibility vDrum => Visibility.Collapsed;
        public Visibility nDrum => Visibility.Collapsed;
        public Visibility vResource => Visibility.Collapsed;
        public Visibility vFuel => Visibility.Collapsed;
        public int nFuelLoss => 0;
        public Visibility vArmo => Visibility.Collapsed;
        public int nArmoLoss => 0;
        public List<int> ResultList => new List<int>();
        public int ExpeditionId { get; set; }
        public bool IsPassed => false;
        #endregion

        public CombinedFleetViewModel(CombinedFleet fleet)
		{
			this.Source = fleet;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet)
			{
				{ nameof(fleet.Name), (sender, args) => this.RaisePropertyChanged(nameof(this.Name)) },
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet.State)
			{
				{ nameof(fleet.State.Situation), (sender, args) => this.RaisePropertyChanged(nameof(this.QuickStateView)) },
                {  nameof(fleet.State.UsedFuel), (sender,args) =>
                    {
                        this.RaisePropertyChanged("UsedFuel");
                        this.RaisePropertyChanged("UsedAmmo");
                        this.RaisePropertyChanged("UsedBauxite");
                    }
                },
                {  nameof(fleet.State.UsedBull), (sender,args) =>
                    {
                        this.RaisePropertyChanged("UsedFuel");
                        this.RaisePropertyChanged("UsedAmmo");
                        this.RaisePropertyChanged("UsedBauxite");
                    }
                }
            });

			this.State = new FleetStateViewModel(fleet.State);
			this.CompositeDisposable.Add(this.State);
		}
	}
}
