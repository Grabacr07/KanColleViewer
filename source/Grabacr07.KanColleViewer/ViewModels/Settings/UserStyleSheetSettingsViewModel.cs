using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models.Settings;
using Livet;
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
					this.IsEditing = true;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsEditing 変更通知プロパティ

		private bool _IsEditing;

		public bool IsEditing
		{
			get { return this._IsEditing; }
			set
			{
				if (this._IsEditing != value)
				{
					this._IsEditing = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public void Initialize()
		{
			GeneralSettings.UserStyleSheet
				.Subscribe(x =>
				{
					this.UserStyleSheet = x;
					this.IsEditing = false;
				})
				.AddTo(this);

			this._UserStyleSheet = GeneralSettings.UserStyleSheet;
			this.RaisePropertyChanged(nameof(this.UserStyleSheet));
		}

		public void Apply()
		{
			if (this.UserStyleSheet == GeneralSettings.UserStyleSheet.Default)
			{
				// User CSS の場合は、既定値と同じ設定をあえて保持したいケースはないでしょう
				// (むしろ下手に残して既定値が変わったときに追従できないトラブルのほうが問題)
				GeneralSettings.UserStyleSheet.Reset();
			}
			else
			{
				GeneralSettings.UserStyleSheet.Value = this.UserStyleSheet;
			}
		}

		public void Cancel()
		{
			this.UserStyleSheet = GeneralSettings.UserStyleSheet;
			this.IsEditing = false;
		}

		public void BackToDefaultValue()
		{
			this.UserStyleSheet = GeneralSettings.UserStyleSheet.Default;
		}
	}
}
