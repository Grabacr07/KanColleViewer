using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class UserStyleSheetSettingsViewModel : ViewModel
	{
		private bool isBackToDefaultValue;

		#region UserStyleSheet 変更通知プロパティ

		private string _UserStyleSheet;

		public string UserStyleSheet
		{
			get { return this._UserStyleSheet; }
			set
			{
				this.isBackToDefaultValue = false;
				if (this._UserStyleSheet != value)
				{
					this._UserStyleSheet = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public void Initialize()
		{
			GeneralSettings.UserStyleSheet.Subscribe(x => this.UserStyleSheet = x).AddTo(this);
			
			if (!GeneralSettings.UserStyleSheet.Provider.IsLoaded)
				GeneralSettings.UserStyleSheet.Provider.Load();
			this.isBackToDefaultValue = !GeneralSettings.UserStyleSheet.Provider.TryGetValue(GeneralSettings.UserStyleSheet.Key, out this._UserStyleSheet);
			this._UserStyleSheet = GeneralSettings.UserStyleSheet;
			this.RaisePropertyChanged(nameof(this.UserStyleSheet));
		}

		public void Apply()
		{
			if (this.isBackToDefaultValue)
				GeneralSettings.UserStyleSheet.Reset();
			else
				GeneralSettings.UserStyleSheet.Value = this.UserStyleSheet;
		}

		public void Cancel()
		{
			this.UserStyleSheet = GeneralSettings.UserStyleSheet;
		}

		public void BackToDefaultValue()
		{
			this.UserStyleSheet = GeneralSettings.UserStyleSheet.Default;
			this.isBackToDefaultValue = true;
		}
	}
}
