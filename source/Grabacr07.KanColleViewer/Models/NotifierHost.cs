using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using MetroTrilithon;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.Models
{
	/// <summary>
	/// <see cref="KanColleClient"/> や関連するオブジェクトからのイベント、プラグインからのイベントを受信し、<see cref="INotifier"/>
	/// を実装する各通知機能へイベントを配信します。
	/// </summary>
	public class NotifyService : NotificationObject, IDisposableHolder
	{
		#region singleton members

		public static NotifyService Current { get; } = new NotifyService();

		#endregion

		private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
		private LivetCompositeDisposable dockyardDisposables;
		private LivetCompositeDisposable repairyardDisposables;
		private LivetCompositeDisposable organizationDisposables;

		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;


		private NotifyService() { }

		public void Initialize()
		{
			foreach (var source in PluginService.Current.Get<IRequestNotify>())
			{
				source.NotifyRequested += HandleNotifyRequested;
				this.compositeDisposable.Add(() => source.NotifyRequested -= HandleNotifyRequested);
			}

			// IsStarted が true に変わる最初にして唯一タイミングで購読登録するのよ、司令官！
			KanColleClient.Current
				.Subscribe(nameof(KanColleClient.IsStarted), this.RegisterHomeportListener, false)
				.AddTo(this);
		}

		private static void HandleNotifyRequested(object sender, NotifyEventArgs e)
		{
			PluginService.Current.GetNotifier().Show(e.Header, e.Body, e.Activated, e.Failed);
		}


		private void RegisterHomeportListener()
		{
			var client = KanColleClient.Current;

			client.Homeport.Repairyard
				.Subscribe(nameof(Repairyard.Docks), () => this.UpdateRepairyard(client.Homeport.Repairyard))
				.AddTo(this);

			client.Homeport.Dockyard
				.Subscribe(nameof(Dockyard.Docks), () => this.UpdateDockyard(client.Homeport.Dockyard))
				.AddTo(this);

			client.Homeport.Organization
				.Subscribe(nameof(Organization.Fleets), () => this.UpdateFleets(client.Homeport.Organization))
				.AddTo(this);
		}


		#region Dockyard

		private void UpdateDockyard(Dockyard dockyard)
		{
			this.dockyardDisposables?.Dispose();
			this.dockyardDisposables = new LivetCompositeDisposable();

			foreach (var dock in dockyard.Docks.Values)
			{
				dock.Completed += HandleDockyardCompleted;
				this.dockyardDisposables.Add(() => dock.Completed -= HandleDockyardCompleted);
			}
		}

		private static void HandleDockyardCompleted(object sender, BuildingCompletedEventArgs args)
		{
			if (!Settings.KanColleSettings.NotifyBuildingCompleted) return;

			var shipName = Settings.KanColleSettings.CanDisplayBuildingShipName
				? args.Ship.Name
				: Resources.Common_ShipGirl;

			PluginService.Current.GetNotifier().Show(
				Resources.Dockyard_NotificationMessage_Title,
				string.Format(Resources.Dockyard_NotificationMessage, args.DockId, shipName),
				() => WindowService.Current.MainWindow.Activate());
		}

		#endregion

		#region Repairyard

		private void UpdateRepairyard(Repairyard repairyard)
		{
			this.repairyardDisposables?.Dispose();
			this.repairyardDisposables = new LivetCompositeDisposable();

			foreach (var dock in repairyard.Docks.Values)
			{
				dock.Completed += HandleRepairyardCompleted;
				this.repairyardDisposables.Add(() => dock.Completed -= HandleRepairyardCompleted);
			}
		}

		private static void HandleRepairyardCompleted(object sender, RepairingCompletedEventArgs args)
		{
			if (!Settings.KanColleSettings.NotifyRepairingCompleted) return;

			PluginService.Current.GetNotifier().Show(
				Resources.Repairyard_NotificationMessage_Title,
				string.Format(Resources.Repairyard_NotificationMessage, args.DockId, args.Ship.Info.Name),
				() => WindowService.Current.MainWindow.Activate());
		}

		#endregion

		#region Fleet

		private void UpdateFleets(Organization organization)
		{
			this.organizationDisposables?.Dispose();
			this.organizationDisposables = new LivetCompositeDisposable();

			foreach (var fleet in organization.Fleets.Values)
			{
				fleet.Expedition.Returned += HandleExpeditionReturned;
				this.organizationDisposables.Add(() => fleet.Expedition.Returned -= HandleExpeditionReturned);

				fleet.State.Condition.Rejuvenated += HandleConditionRejuvenated;
				this.organizationDisposables.Add(() => fleet.State.Condition.Rejuvenated -= HandleConditionRejuvenated);
			}
		}

		private static void HandleExpeditionReturned(object sender, ExpeditionReturnedEventArgs args)
		{
			if (!Settings.KanColleSettings.NotifyExpeditionReturned) return;

			PluginService.Current.GetNotifier().Show(
				Resources.Expedition_NotificationMessage_Title,
				string.Format(Resources.Expedition_NotificationMessage, args.FleetName),
				() => WindowService.Current.MainWindow.Activate());
		}

		private static void HandleConditionRejuvenated(object sender, ConditionRejuvenatedEventArgs args)
		{
			if (!Settings.KanColleSettings.NotifyFleetRejuvenated) return;

			PluginService.Current.GetNotifier().Show(
				"疲労回復完了",
				$"「{args.FleetName}」に編成されている艦娘の疲労が回復しました。",
				() => WindowService.Current.MainWindow.Activate());
		}

		#endregion


		public void Dispose()
		{
			this.compositeDisposable.Dispose();
			this.dockyardDisposables?.Dispose();
			this.repairyardDisposables?.Dispose();
			this.organizationDisposables?.Dispose();
		}
	}
}
