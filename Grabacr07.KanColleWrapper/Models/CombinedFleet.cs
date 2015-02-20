using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Livet.EventListeners;
using Livet.EventListeners.WeakEvents;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 二個以上の常設艦隊で編成された連合艦隊を表します。
	/// </summary>
	public class CombinedFleet : DisposableNotifier
	{
		#region Name 変更通知プロパティ

		private string _Name;

		public string Name
		{
			get { return this._Name; }
			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Fleets 変更通知プロパティ

		private Fleet[] _Fleets;

		public Fleet[] Fleets
		{
			get { return this._Fleets; }
			set
			{
				if (this._Fleets != value)
				{
					this._Fleets = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public FleetState State { get; private set; }

		public CombinedFleet(Homeport parent, params Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) throw new ArgumentException();

			this.Fleets = fleets;
			this.State = new FleetState(parent, fleets);
			this.CompositeDisposable.Add(this.State);

			foreach (var fleet in fleets)
			{
				this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet)
				{
					{ "Name", (sender, args) => this.UpdateName() },
				});

				var source = fleet;
				this.CompositeDisposable.Add(new LivetWeakEventListener<EventHandler, EventArgs>(
					h => new EventHandler(h),
					h => source.State.Updated += h,
					h => source.State.Updated -= h,
					(sender, args) => this.State.Update()));
				this.CompositeDisposable.Add(new LivetWeakEventListener<EventHandler, EventArgs>(
					h => new EventHandler(h),
					h => source.State.Calculated += h,
					h => source.State.Calculated -= h,
					(sender, args) => this.State.Calculate()));
			}

			this.UpdateName();

			this.State.Calculate();
			this.State.Update();
		}

		private void UpdateName()
		{
			this.Name = this.Fleets.Select(x => x.Name).Join(", ");
		}
	}
}