using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Models
{
	class ResourceLogManager
	{
		#region 자원 프로퍼티들
		private int CurrentFuel { get; set; }
		private int CurrentAmmo { get; set; }
		private int CurrentSteel { get; set; }
		private int CurrentBauxite { get; set; }
		private int CurrentRepairBucket { get; set; }
		private int CurrentInstantConstruction { get; set; }
		private int CurrentDevelopmentMaterial { get; set; }
		private int CurrentImprovementMaterial { get; set; }
		#endregion

		private string ResourceCachePath => Path.Combine("Record", "resourcelog.csv");

		private Random RandomInstance { get; } = new Random();
		private double ListenerEventID { get; set; }


		public ResourceLogManager()
		{
			var client = KanColleClient.Current;
			CurrentFuel = 0;
			CurrentAmmo = 0;
			CurrentSteel = 0;
			CurrentBauxite = 0;
			CurrentRepairBucket = 0;
			CurrentInstantConstruction = 0;
			CurrentDevelopmentMaterial = 0;
			CurrentImprovementMaterial = 0;

			client.PropertyChanged+=(_, __) =>
			{
				if (__.PropertyName != nameof(client.IsStarted)) return;

				var materials = KanColleClient.Current.Homeport.Materials;

				#region PropertyChangedEventListener
				materials.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == nameof(materials.Fuel))
					{
						CurrentFuel = materials.Fuel;
						Task.Run(() => ListenerEventWorker());
					}
					if (e.PropertyName == nameof(materials.Ammunition))
					{
						CurrentAmmo = materials.Ammunition;
						Task.Run(() => ListenerEventWorker());
					}
					if (e.PropertyName == nameof(materials.Steel))
					{
						CurrentSteel = materials.Steel;
						Task.Run(() => ListenerEventWorker());
					}
					if (e.PropertyName == nameof(materials.Bauxite))
					{
						CurrentBauxite = materials.Bauxite;
						Task.Run(() => ListenerEventWorker());
					}
					if (e.PropertyName == nameof(materials.InstantRepairMaterials))
					{
						CurrentRepairBucket = materials.InstantRepairMaterials;
						Task.Run(() => ListenerEventWorker());
					}
					if (e.PropertyName == nameof(materials.InstantBuildMaterials))
					{
						CurrentInstantConstruction = materials.InstantBuildMaterials;
						Task.Run(() => ListenerEventWorker());
					}
					if (e.PropertyName == nameof(materials.DevelopmentMaterials))
					{
						CurrentDevelopmentMaterial = materials.DevelopmentMaterials;
						Task.Run(() => ListenerEventWorker());
					}
					if (e.PropertyName == nameof(materials.ImprovementMaterials))
					{
						CurrentImprovementMaterial = materials.ImprovementMaterials;
						Task.Run(() => ListenerEventWorker());
					}
				};
				#endregion
			};
		}

		private async void ListenerEventWorker()
		{
			var EventID = RandomInstance.Next();
			ListenerEventID = EventID;

			await Task.Delay(500);
			if (EventID != ListenerEventID) return; // Another event called

			string zItemsPath = ResourceCachePath;

			var res = new ResourceModel
			{
				Date = DateTime.Now,

				Fuel = CurrentFuel,
				Ammo = CurrentAmmo,
				Steel = CurrentSteel,
				Bauxite = CurrentBauxite,

				RepairBucket = CurrentRepairBucket,
				DevelopmentMaterial = CurrentDevelopmentMaterial,
				InstantConstruction = CurrentInstantConstruction,
				ImprovementMaterial = CurrentImprovementMaterial
			};

			CriticalSection("ResourceChartWrite", () =>
			{
				int tries = 5;

				while (tries > 0)
				{
					try
					{
						using (FileStream fs = new FileStream(zItemsPath, FileMode.Append))
						{
							CSV.Write(fs,
								res.Date.ToString("yyyy-MM-dd HH:mm:ss"),
								res.Fuel, res.Ammo, res.Steel, res.Bauxite,
								res.RepairBucket, res.DevelopmentMaterial,
								res.InstantConstruction, res.ImprovementMaterial
							);
						}
						break;
					}
					catch (IOException) { tries--; }
					catch (Exception) { break; }
				}
			});
		}

		// Lock CriticalSection
		private Dictionary<string, object> LockTable = new Dictionary<string, object>();
		private void CriticalSection(string Name, Action Worker)
		{
			lock (LockTable)
			{
				if (!LockTable.ContainsKey(Name))
					LockTable.Add(Name, new object());
			}
			lock (LockTable[Name]) Worker();
		}
	}
}
