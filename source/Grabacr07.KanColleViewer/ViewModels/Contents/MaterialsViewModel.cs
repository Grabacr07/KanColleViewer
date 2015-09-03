using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using Livet;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class MaterialsViewModel : ViewModel
	{
		public Materials Model { get; }

		public ICollection<MaterialViewModel> Values { get; }

		#region SelectedItem1 変更通知プロパティ

		private MaterialViewModel _SelectedItem1;

		public MaterialViewModel SelectedItem1
		{
			get { return this._SelectedItem1; }
			set
			{
				if (this._SelectedItem1 != value)
				{
					this._SelectedItem1 = value;
					this.RaisePropertyChanged();
					KanColleSettings.DisplayMaterial1.Value = value?.Key;
				}
			}
		}

		#endregion

		#region SelectedItem2 変更通知プロパティ

		private MaterialViewModel _SelectedItem2;

		public MaterialViewModel SelectedItem2
		{
			get { return this._SelectedItem2; }
			set
			{
				if (this._SelectedItem2 != value)
				{
					this._SelectedItem2 = value;
					this.RaisePropertyChanged();
					KanColleSettings.DisplayMaterial2.Value = value?.Key;
				}
			}
		}

		#endregion

		public MaterialsViewModel()
		{
			this.Model = KanColleClient.Current.Homeport.Materials;

			var fuel = new MaterialViewModel(nameof(Materials.Fuel), Resources.Common_Resources_Fuel).AddTo(this);
			this.Model.Subscribe(fuel.Key, () => fuel.Value = this.Model.Fuel).AddTo(this);

			var ammunition = new MaterialViewModel(nameof(Materials.Ammunition), Resources.Common_Resources_Ammo).AddTo(this);
			this.Model.Subscribe(ammunition.Key, () => ammunition.Value = this.Model.Ammunition).AddTo(this);

			var steel = new MaterialViewModel(nameof(Materials.Steel), Resources.Common_Resources_Steel).AddTo(this);
			this.Model.Subscribe(steel.Key, () => steel.Value = this.Model.Steel).AddTo(this);

			var bauxite = new MaterialViewModel(nameof(Materials.Bauxite), Resources.Common_Resources_Bauxite).AddTo(this);
			this.Model.Subscribe(bauxite.Key, () => bauxite.Value = this.Model.Bauxite).AddTo(this);

			var develop = new MaterialViewModel(nameof(Materials.DevelopmentMaterials), Resources.Common_Resources_DevKits).AddTo(this);
			this.Model.Subscribe(develop.Key, () => develop.Value = this.Model.DevelopmentMaterials).AddTo(this);

			var repair = new MaterialViewModel(nameof(Materials.InstantRepairMaterials), Resources.Common_Resources_InstantRepair).AddTo(this);
			this.Model.Subscribe(repair.Key, () => repair.Value = this.Model.InstantRepairMaterials).AddTo(this);

			var build = new MaterialViewModel(nameof(Materials.InstantBuildMaterials), Resources.Common_Resources_InstantBuild).AddTo(this);
			this.Model.Subscribe(build.Key, () => build.Value = this.Model.InstantBuildMaterials).AddTo(this);

			var improvement = new MaterialViewModel(nameof(Materials.ImprovementMaterials), Resources.Common_Resources_Improvement).AddTo(this);
			this.Model.Subscribe(improvement.Key, () => improvement.Value = this.Model.ImprovementMaterials).AddTo(this);

			this.Values = new List<MaterialViewModel>
			{
				fuel,
				ammunition,
				steel,
				bauxite,
				develop,
				repair,
				build,
				improvement,
			};

			this._SelectedItem1 = this.Values.FirstOrDefault(x => x.Key == KanColleSettings.DisplayMaterial1) ?? repair;
			this._SelectedItem2 = this.Values.FirstOrDefault(x => x.Key == KanColleSettings.DisplayMaterial2) ?? build;
		}

		public class MaterialViewModel : ViewModel
		{
			public string Key { get; }

			public string Display { get; }

			#region Value 変更通知プロパティ

			private int _Value;

			public int Value
			{
				get { return this._Value; }
				set
				{
					if (this._Value != value)
					{
						this._Value = value;
						this.RaisePropertyChanged();
					}
				}
			}

			#endregion

			public MaterialViewModel(string key, string display)
			{
				this.Key = key;
				this.Display = display;
			}
		}
	}
}
