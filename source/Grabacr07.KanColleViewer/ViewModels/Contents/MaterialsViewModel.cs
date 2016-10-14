using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        #region SelectedItem3 変更通知プロパティ

        private MaterialViewModel _SelectedItem3;

        public MaterialViewModel SelectedItem3
        {
            get { return this._SelectedItem3; }
            set
            {
                if (this._SelectedItem3 != value)
                {
                    this._SelectedItem3 = value;
                    this.RaisePropertyChanged();
                    KanColleSettings.DisplayMaterial3.Value = value?.Key;
                }
            }
        }

        #endregion

        #region SelectedItem4 変更通知プロパティ

        private MaterialViewModel _SelectedItem4;

        public MaterialViewModel SelectedItem4
        {
            get { return this._SelectedItem4; }
            set
            {
                if (this._SelectedItem4 != value)
                {
                    this._SelectedItem4 = value;
                    this.RaisePropertyChanged();
                    KanColleSettings.DisplayMaterial4.Value = value?.Key;
                }
            }
        }

        #endregion

        public MaterialsViewModel()
		{
			this.Model = KanColleClient.Current.Homeport.Materials;

			var fuel = new MaterialViewModel(nameof(Materials.Fuel), "연료").AddTo(this);
			this.Model.Subscribe(fuel.Key, () => fuel.Value = this.Model.Fuel).AddTo(this);

			var ammunition = new MaterialViewModel(nameof(Materials.Ammunition), "탄약").AddTo(this);
			this.Model.Subscribe(ammunition.Key, () => ammunition.Value = this.Model.Ammunition).AddTo(this);

			var steel = new MaterialViewModel(nameof(Materials.Steel), "강재").AddTo(this);
			this.Model.Subscribe(steel.Key, () => steel.Value = this.Model.Steel).AddTo(this);

			var bauxite = new MaterialViewModel(nameof(Materials.Bauxite), "보크사이트").AddTo(this);
			this.Model.Subscribe(bauxite.Key, () => bauxite.Value = this.Model.Bauxite).AddTo(this);

			var develop = new MaterialViewModel(nameof(Materials.DevelopmentMaterials), "개발자재").AddTo(this);
			this.Model.Subscribe(develop.Key, () => develop.Value = this.Model.DevelopmentMaterials).AddTo(this);

			var repair = new MaterialViewModel(nameof(Materials.InstantRepairMaterials), "고속수복재").AddTo(this);
			this.Model.Subscribe(repair.Key, () => repair.Value = this.Model.InstantRepairMaterials).AddTo(this);

			var build = new MaterialViewModel(nameof(Materials.InstantBuildMaterials), "고속건조재").AddTo(this);
			this.Model.Subscribe(build.Key, () => build.Value = this.Model.InstantBuildMaterials).AddTo(this);

			var improvement = new MaterialViewModel(nameof(Materials.ImprovementMaterials), "개수자재").AddTo(this);
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
            this._SelectedItem3 = this.Values.FirstOrDefault(x => x.Key == KanColleSettings.DisplayMaterial3) ?? fuel;
            this._SelectedItem4 = this.Values.FirstOrDefault(x => x.Key == KanColleSettings.DisplayMaterial4) ?? ammunition;
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
