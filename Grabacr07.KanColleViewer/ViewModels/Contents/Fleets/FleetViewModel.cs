using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

		public int Id
		{
			get { return this.source.Id; }
		}

		public string Name
		{
			get { return string.IsNullOrEmpty(this.source.Name.Trim()) ? "(第 " + this.source.Id + " 艦隊)" : this.source.Name; }
		}

		/// <summary>
		/// 艦隊に所属している艦娘のコレクションを取得します。
		/// </summary>
		public ShipViewModel[] Ships
		{
			get { return this.source.Ships.Select(x => new ShipViewModel(x)).ToArray(); }
		}

		public FleetStateViewModel State { get; private set; }

		public ExpeditionViewModel Expedition { get; private set; }

		public ViewModel QuickStateView
		{
			get
			{
				var situation = this.source.State.Situation;
				if (situation == FleetSituation.Empty)
				{
					return NullViewModel.Instance;
				}
				if (situation.HasFlag(FleetSituation.Sortie))
				{
					return this.State.Sortie;
				}
				if (situation.HasFlag(FleetSituation.Expedition))
				{
					return this.Expedition;
				}

				return this.State.Homeport;
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


		public FleetViewModel(Fleet fleet)
		{
			this.source = fleet;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet.State)
			{
				{ "Situation", (sender, args) => this.RaisePropertyChanged("QuickStateView") },
			});

			this.State = new FleetStateViewModel(fleet.State);
			this.CompositeDisposable.Add(this.State);

			this.Expedition = new ExpeditionViewModel(fleet.Expedition);
			this.CompositeDisposable.Add(this.Expedition);
		}
	}
}
