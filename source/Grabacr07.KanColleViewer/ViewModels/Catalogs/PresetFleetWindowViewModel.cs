using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using MetroTrilithon.Mvvm;
using Livet;
using Livet.Messaging;
using Livet.EventListeners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Grabacr07.KanColleViewer.ViewModels.Contents;

using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class PresetFleetWindowViewModel : WindowViewModel
	{
		#region Fleets 변경 통지 프로퍼티
		private IReadOnlyCollection<PresetFleetData> _Fleets { get; set; }
		public IReadOnlyCollection<PresetFleetData> Fleets
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

		#region SelectedFleet 변경 통지 프로퍼티
		private PresetFleetData _SelectedFleet { get; set; }
		public PresetFleetData SelectedFleet
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

		#region ExSlotChecked 변경 통지 프로퍼티
		public static bool _ExSlotChecked { get; set; }
		public bool ExSlotChecked
		{
			get { return _ExSlotChecked; }
			set
			{
				if (_ExSlotChecked != value)
				{
					_ExSlotChecked = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		private string RecordPath => Path.Combine(
			Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
			"Record",
			"fleets.txt"
		);

		public PresetFleetWindowViewModel()
		{
			this.Title = "함대 프리셋";
			this.ExSlotChecked = true;

			this.LoadFleets();
		}

		public void LoadFleets()
		{
			var list = new List<PresetFleetData>();

			if (File.Exists(RecordPath))
			{
				try
				{
					string[] lines = File.ReadAllText(RecordPath)
						.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (var line in lines)
					{
						var item = new PresetFleetData();
						item.Deserialize(line);
						list.Add(item);
					}
				}
				catch { }
			}

			this.Fleets = list.ToArray();
			this.SelectedFleet = this.Fleets.FirstOrDefault();
		}
		public void SaveFleets()
		{
			List<string> datas = new List<string>();
			foreach (var fleet in this.Fleets)
				datas.Add(fleet.Serialize());

			try
			{
				File.WriteAllText(RecordPath, string.Join(Environment.NewLine, datas));
			}
			catch { }
		}

		public void ShowPresetAddWindow()
		{
			var fleetwd = new PresetFleetAddWindowViewModel(this);
			var message = new TransitionMessage(fleetwd, TransitionMode.Normal, "AddFleetWindow.Show");
			this.Messenger.Raise(message);
		}
		public void ShowPresetDeleteWindow()
		{
			var fleet = this.SelectedFleet;
			if (fleet == null) return;

			var fleetwd = new PresetFleetDeleteWindowViewModel(this, fleet);
			var message = new TransitionMessage(fleetwd, TransitionMode.Normal, "DeleteFleetWindow.Show");
			this.Messenger.Raise(message);
		}

		public void AddFleet(PresetFleetData fleet)
		{
			var list = this.Fleets.ToList();
			list.Add(fleet);

			this.Fleets = list.ToArray();
			SaveFleets();
		}
		public void DeleteFleet(PresetFleetData fleet)
		{
			this.Fleets = this.Fleets.Where(x => x != fleet).ToArray();
			SaveFleets();
		}
	}
}
