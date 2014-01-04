using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class FleetsViewModel : TabItemViewModel
	{
		#region Fleets 変更通知プロパティ

		private FleetViewModel[] _Fleets;

		public FleetViewModel[] Fleets
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

		#region SelectedFleet 変更通知プロパティ

		private FleetViewModel _SelectedFleet;

		/// <summary>
		/// 現在選択されている艦隊を取得または設定します。
		/// </summary>
		public FleetViewModel SelectedFleet
		{
			get { return this._SelectedFleet; }
			set
			{
				if (this._SelectedFleet != value)
				{
					this._SelectedFleet = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsNotifyReturned 変更通知プロパティ

		/// <summary>
		/// 遠征帰投時にトースト通知を表示するかどうかを示す値を取得します。
		/// </summary>
		public bool IsNotifyReturned
		{
			get { return Settings.Current.NotifyExpeditionReturned; }
			set
			{
				if (Settings.Current.NotifyExpeditionReturned != value)
				{
					Settings.Current.NotifyExpeditionReturned = value;
					this.Fleets.ForEach(x => x.Expedition.IsNotifyReturned = value);
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public bool IsSupportedNotification
		{
			get { return Helper.IsWindows8OrGreater; }
		}


		public FleetsViewModel()
		{
			this.Name = Properties.Resources.Fleets;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport)
			{
				{ "Fleets", (sender, args) => this.UpdateFleets() },
			});
			this.UpdateFleets();
		}

		private void UpdateFleets()
		{
			this.Fleets = KanColleClient.Current.Homeport.Fleets.Select(kvp => new FleetViewModel(kvp.Value)).ToArray();
			this.SelectedFleet = this.Fleets.FirstOrDefault();
			this.Fleets.ForEach(x => x.Expedition.IsNotifyReturned = this.IsNotifyReturned);
		}
	}
}
