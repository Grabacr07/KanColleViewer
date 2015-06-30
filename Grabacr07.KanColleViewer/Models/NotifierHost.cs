using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Livet;

namespace Grabacr07.KanColleViewer.Models
{
	public class NotifierHost : NotificationObject, IDisposable
	{
		#region singleton members

		public static NotifierHost Instance { get; } = new NotifierHost();

		#endregion

		private NotifierHost() { }

		public void Initialize()
		{
			foreach (var source in PluginHost.Instance.Get<IRequestNotify>())
			{
				source.NotifyRequested += HandleNotifyRequested;
			}

			KanColleClient.Current.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == nameof(KanColleClient.IsStarted)) InitializeCore();
			};
		}

		private static void InitializeCore()
		{
			var client = KanColleClient.Current;

			client.Homeport.Repairyard.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == nameof(Repairyard.Docks)) UpdateRepairyard(client.Homeport.Repairyard);
			};
			UpdateRepairyard(client.Homeport.Repairyard);

			client.Homeport.Dockyard.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == nameof(Dockyard.Docks)) UpdateDockyard(client.Homeport.Dockyard);
			};
			UpdateDockyard(client.Homeport.Dockyard);

			client.Homeport.Organization.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == nameof(Organization.Fleets)) UpdateFleets(client.Homeport.Organization);
			};
			UpdateFleets(client.Homeport.Organization);
		}

		private static void UpdateRepairyard(Repairyard repairyard)
		{
			foreach (var dock in repairyard.Docks.Values)
			{
				dock.Completed += (sender, args) =>
				{
					if (Settings.Current.NotifyRepairingCompleted)
					{
						PluginHost.Instance.GetNotifier().Show(
							Resources.Repairyard_NotificationMessage_Title,
							string.Format(Resources.Repairyard_NotificationMessage, args.DockId, args.Ship.Info.Name),
							() => Application.Current.MainWindowViewModel.Activate());
					}
				};
			}
		}

		private static void UpdateDockyard(Dockyard dockyard)
		{
			foreach (var dock in dockyard.Docks.Values)
			{
				dock.Completed += (sender, args) =>
				{
					if (Settings.Current.NotifyBuildingCompleted)
					{
						var shipName = Settings.Current.CanDisplayBuildingShipName
							? args.Ship.Name
							: Resources.Common_ShipGirl;

						PluginHost.Instance.GetNotifier().Show(
							Resources.Dockyard_NotificationMessage_Title,
							string.Format(Resources.Dockyard_NotificationMessage, args.DockId, shipName),
							() => Application.Current.MainWindowViewModel.Activate());
					}
				};
			}
		}

		private static void UpdateFleets(Organization organization)
		{
			foreach (var fleet in organization.Fleets.Values)
			{
				fleet.Expedition.Returned += (sender, args) =>
				{
					if (Settings.Current.NotifyExpeditionReturned)
					{
						PluginHost.Instance.GetNotifier().Show(
							Resources.Expedition_NotificationMessage_Title,
							string.Format(Resources.Expedition_NotificationMessage, args.FleetName),
							() => Application.Current.MainWindowViewModel.Activate());
					}
				};

				fleet.State.Condition.Rejuvenated += (sender, args) =>
				{
					if (Settings.Current.NotifyFleetRejuvenated)
					{
						PluginHost.Instance.GetNotifier().Show(
							"疲労回復完了",
							$"「{args.FleetName}」に編成されている艦娘の疲労が回復しました。",
							() => Application.Current.MainWindowViewModel.Activate());
					}
				};
			}
		}

		private static void HandleNotifyRequested(object sender, NotifyEventArgs e)
		{
			PluginHost.Instance.GetNotifier().Show(e.Header, e.Body, e.Activated, e.Failed);
		}

		public void Dispose()
		{
			foreach (var source in PluginHost.Instance.Get<IRequestNotify>())
			{
				source.NotifyRequested -= HandleNotifyRequested;
			}
		}
	}
}
