using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Livet;

namespace Grabacr07.KanColleViewer.Models
{
	public class NotifierHost : NotificationObject
	{
		#region singleton members

		private static readonly NotifierHost instance = new NotifierHost();

		public static NotifierHost Instance
		{
			get { return instance; }
		}

		#endregion

		private NotifierHost() { }

		public void Initialize(KanColleClient client)
		{
			client.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == "IsStarted") this.InitializeCore(client);
			};

		}

		private void InitializeCore(KanColleClient client)
		{
			client.Homeport.Repairyard.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == "Docks") UpdateRepairyard(client.Homeport.Repairyard);
			};
			UpdateRepairyard(client.Homeport.Repairyard);

			client.Homeport.Dockyard.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == "Docks") UpdateDockyard(client.Homeport.Dockyard);
			};
			UpdateDockyard(client.Homeport.Dockyard);

			client.Homeport.Organization.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == "Fleets") UpdateExpedition(client.Homeport.Organization);
			};
			UpdateExpedition(client.Homeport.Organization);
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
							NotifyType.Repair,
							Resources.Repairyard_NotificationMessage_Title,
							string.Format(Resources.Repairyard_NotificationMessage, args.DockId, args.Ship.Info.Name),
							() => App.ViewModelRoot.Activate());
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
						PluginHost.Instance.GetNotifier().Show(
							NotifyType.Build,
							Resources.Dockyard_NotificationMessage_Title,
							string.Format(Resources.Dockyard_NotificationMessage, args.DockId, args.Ship.Name),
							() => App.ViewModelRoot.Activate());
					}
				};
			}
		}

		private static void UpdateExpedition(Organization organization)
		{
			foreach (var fleet in organization.Fleets.Values)
			{
				fleet.Expedition.Returned += (sender, args) =>
				{
					if (Settings.Current.NotifyExpeditionReturned)
					{
						PluginHost.Instance.GetNotifier().Show(
							NotifyType.Expedition,
							Resources.Expedition_NotificationMessage_Title,
							string.Format(Resources.Expedition_NotificationMessage, args.FleetName),
							() => App.ViewModelRoot.Activate());
					}
				};

				fleet.Condition.Rejuvenated += (sender, args) =>
				{
					if (Settings.Current.NotifyFleetRejuvenated)
					{
						PluginHost.Instance.GetNotifier().Show(
							NotifyType.Rejuvenated,
							"疲労回復完了",
							string.Format("「{0}」に編成されている艦娘の疲労が回復しました。", args.FleetName),
							() => App.ViewModelRoot.Activate());
					}
				};
			}
		}
	}
}
