using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class StartContentViewModel : ViewModel
	{
		public NavigatorViewModel Navigator { get; }

		#region CanDeleteInternetCache 変更通知プロパティ

		private bool _CanDeleteInternetCache = true;

		public bool CanDeleteInternetCache
		{
			get { return this._CanDeleteInternetCache; }
			set
			{
				if (this._CanDeleteInternetCache != value)
				{
					this._CanDeleteInternetCache = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region DeleteInternetCacheButtonContent 変更通知プロパティ

		private string _DeleteInternetCacheButtonContent = Properties.Resources.StartContent_ClearCacheButton;

		public string DeleteInternetCacheButtonContent
		{
			get { return this._DeleteInternetCacheButtonContent; }
			set
			{
				if (this._DeleteInternetCacheButtonContent != value)
				{
					this._DeleteInternetCacheButtonContent = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public StartContentViewModel(NavigatorViewModel navigator)
		{
			this.Navigator = navigator;
		}

		public async void DeleteInternetCache()
		{
			this.CanDeleteInternetCache = false;

			try
			{
				// ToDo: ダイアログで処理通知とか結果通知とかした方がよい
				// 今はとりあえず版ということで、1 回やったらボタンを非活性化しよう
				var result = await Helper.DeleteInternetCache();
				if (result)
				{
					this.DeleteInternetCacheButtonContent = Properties.Resources.StartContent_ClearCacheButtonMessage;
				}
				else
				{
					this.CanDeleteInternetCache = true;
				}

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
