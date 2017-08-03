using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.Models
{
	/// <summary>
	/// <see cref="KanColleClient"/> や関連するオブジェクトからのイベント、プラグインからのイベントを受信し、<see cref="INotifier"/>
	/// を実装する各通知機能へイベントを配信します。
	/// </summary>
	public class NotifyService : NotificationObject, INotifier, IDisposableHolder
	{
		#region singleton members

		public static NotifyService Current { get; } = new NotifyService();

		#endregion

		private INotifier notifier;
		private bool isRegistered;

		private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
		private LivetCompositeDisposable dockyardDisposables;
		private LivetCompositeDisposable repairyardDisposables;
		private LivetCompositeDisposable organizationDisposables;
        private LivetCompositeDisposable akashiDisposable;

		private NotifyService() { }

		public void Initialize()
		{
			foreach (var source in PluginService.Current.Get<IRequestNotify>())
			{
				source.NotifyRequested += this.HandleNotifyRequested;
				Disposable.Create(() => source.NotifyRequested -= this.HandleNotifyRequested).AddTo(this);
			}

			// IsStarted が true に変わる最初にして唯一タイミングで購読登録するのよ、司令官！
			KanColleClient.Current
				.Subscribe(nameof(KanColleClient.IsStarted), this.RegisterHomeportListener, false)
				.AddTo(this);
		}

		public void Notify(INotification notify)
		{
			if (this.notifier == null)
			{
				this.notifier = PluginService.Current.GetNotifier();
			}

			this.notifier.Notify(notify);
		}

		public INotification CreateTest(string header = "테스트알림", string body = "「제독업무도 바빠！」의 테스트 알림입니다.", Action activated = null, Action<Exception> failed = null)
		{
			return Notification.Create(Notification.Types.Test, header, body, activated ?? WindowService.Current.MainWindow.Activate, failed);
		}

		#region Initialize() method parts

		private void HandleNotifyRequested(object sender, NotifyEventArgs e)
		{
			this.Notify(e);
		}

		private void RegisterHomeportListener()
		{
			if (this.isRegistered) return;

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

			this.isRegistered = true;
		}

		#endregion

		#region Dockyard

		private void UpdateDockyard(Dockyard dockyard)
		{
			this.dockyardDisposables?.Dispose();
			this.dockyardDisposables = new LivetCompositeDisposable();

			foreach (var dock in dockyard.Docks.Values)
			{
				dock.Completed += this.HandleDockyardCompleted;
				this.dockyardDisposables.Add(() => dock.Completed -= this.HandleDockyardCompleted);
			}
		}

		private void HandleDockyardCompleted(object sender, BuildingCompletedEventArgs args)
		{
			if (!Settings.KanColleSettings.NotifyBuildingCompleted) return;

			var shipName = Settings.KanColleSettings.CanDisplayBuildingShipName
				? args.Ship.Name
				: Resources.Common_ShipGirl;

			var notification = Notification.Create(
				Notification.Types.BuildingCompleted,
				Resources.Dockyard_NotificationMessage_Title,
				string.Format(Resources.Dockyard_NotificationMessage, args.DockId, shipName),
				() => WindowService.Current.MainWindow.Activate());

			this.Notify(notification);
		}

		#endregion

		#region Repairyard

		private void UpdateRepairyard(Repairyard repairyard)
		{
			this.repairyardDisposables?.Dispose();
			this.repairyardDisposables = new LivetCompositeDisposable();

			foreach (var dock in repairyard.Docks.Values)
			{
				dock.Completed += this.HandleRepairyardCompleted;
				this.repairyardDisposables.Add(() => dock.Completed -= this.HandleRepairyardCompleted);
			}
		}

		private void HandleRepairyardCompleted(object sender, RepairingCompletedEventArgs args)
		{
			if (!Settings.KanColleSettings.NotifyRepairingCompleted) return;

			var notification = Notification.Create(
				Notification.Types.RepairingCompleted,
				Resources.Repairyard_NotificationMessage_Title,
				string.Format(Resources.Repairyard_NotificationMessage, args.DockId, args.Ship.Info.Name),
				() => WindowService.Current.MainWindow.Activate());

			this.Notify(notification);
		}

		#endregion

		#region Fleet

		private void UpdateFleets(Organization organization)
		{
			this.organizationDisposables?.Dispose();
			this.organizationDisposables = new LivetCompositeDisposable();

			foreach (var fleet in organization.Fleets.Values)
			{
				fleet.Expedition.Returned += this.HandleExpeditionReturned;
				this.organizationDisposables.Add(() => fleet.Expedition.Returned -= this.HandleExpeditionReturned);

				fleet.State.Condition.Rejuvenated += this.HandleConditionRejuvenated;
				this.organizationDisposables.Add(() => fleet.State.Condition.Rejuvenated -= this.HandleConditionRejuvenated);
			}
		}

		private void HandleExpeditionReturned(object sender, ExpeditionReturnedEventArgs args)
		{
			if (!Settings.KanColleSettings.NotifyExpeditionReturned) return;

			var notify = Notification.Create(
				Notification.Types.ExpeditionReturned,
				Resources.Expedition_NotificationMessage_Title,
				string.Format(Resources.Expedition_NotificationMessage, args.FleetName),
				() => WindowService.Current.MainWindow.Activate());

			this.Notify(notify);
		}

		private void HandleConditionRejuvenated(object sender, ConditionRejuvenatedEventArgs args)
		{
			if (!Settings.KanColleSettings.NotifyFleetRejuvenated) return;

			var notification = Notification.Create(
				Notification.Types.FleetRejuvenated,
				"피로회복완료",
				$"「{args.FleetName}」의 피로회복이 완료되었습니다",
				() => WindowService.Current.MainWindow.Activate());

			this.Notify(notification);
		}

        #endregion

        #region AkashiTimer

        public void UpdateAkashiTimer(AkashiTimer timer)
        {
            this.akashiDisposable?.Dispose();
            this.akashiDisposable = new LivetCompositeDisposable();

            timer.Repaired += this.HandleAkashiTimerRepaired;
            this.akashiDisposable.Add(() => timer.Repaired -= this.HandleAkashiTimerRepaired);
        }

        private void HandleAkashiTimerRepaired(object sender, EventArgs args)
        {
            if (!Settings.KanColleSettings.AkashiTwentyMinute) return;

            var notification = Notification.Create(
                Notification.Types.RepairingCompleted,
                Resources.Repairyard_NotificationMessage_Title,
                Resources.Akashi_NotificationMessage,
                () => WindowService.Current.MainWindow.Activate());

            this.Notify(notification);
        }

        #endregion

        #region IDisposable members

        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

		public void Dispose()
		{
			this.compositeDisposable.Dispose();
			this.dockyardDisposables?.Dispose();
			this.repairyardDisposables?.Dispose();
			this.organizationDisposables?.Dispose();
		}

		#endregion
	}
}
