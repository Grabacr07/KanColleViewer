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
		#region UserStyleSheet 変更通知プロパティ

		private string _UserStyleSheet;

		public string UserStyleSheet
		{
			get { return this._UserStyleSheet; }
			set
			{
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
			this.Cancel();
			GeneralSettings.UserStyleSheet.Subscribe(x => this.UserStyleSheet = x).AddTo(this);
		}

		public void Apply()
		{
			GeneralSettings.UserStyleSheet.Value = this.UserStyleSheet;
		}

		public void Cancel()
		{
			this.UserStyleSheet = GeneralSettings.UserStyleSheet;
		}

		public void BackToDefaultValue()
		{
			this.UserStyleSheet = GeneralSettings.UserStyleSheet.Default;
		}
	}
}
