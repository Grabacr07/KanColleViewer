using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Livet;
using Livet.Commands;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class NavigatorViewModel : ViewModel, INavigator
	{
		#region Source 変更通知プロパティ

		private Uri _Source;

		public Uri Source
		{
			get { return this._Source; }
			set
			{
				this._Source = value;
				this.SourceString = value.ToString();

				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region SourceString 変更通知プロパティ

		private string _SourceString;

		public string SourceString
		{
			get { return this._SourceString; }
			set
			{
				if (this._SourceString != value)
				{
					this._SourceString = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsNavigating 変更通知プロパティ

		private bool _IsNavigating;

		public bool IsNavigating
		{
			get { return this._IsNavigating; }
			set
			{
				if (this._IsNavigating != value)
				{
					this._IsNavigating = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CanGoBack 変更通知プロパティ

		private bool _CanGoBack;

		public bool CanGoBack
		{
			get { return this._CanGoBack; }
			set
			{
				if (this._CanGoBack != value)
				{
					this._CanGoBack = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CanGoForward 変更通知プロパティ

		private bool _CanGoForward;

		public bool CanGoForward
		{
			get { return this._CanGoForward; }
			set
			{
				if (this._CanGoForward != value)
				{
					this._CanGoForward = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NavigateCommand コマンド

		private ViewModelCommand _NavigateCommand;

		public ViewModelCommand NavigateCommand => this._NavigateCommand ?? (this._NavigateCommand = new ViewModelCommand(this.Navigate));

		#endregion

		public event EventHandler GoBackRequested;
		public event EventHandler GoForwardRequested;
		public event EventHandler RefreshRequested;
		public event EventHandler<Uri> UriRequested;

		private bool IsCookieEdit;

		public void GoBack()
		{
			this.GoBackRequested?.Invoke(this, new EventArgs());
		}

		public void GoForward()
		{
			this.GoForwardRequested?.Invoke(this, new EventArgs());
		}

		public void Refresh()
		{
			this.RefreshRequested?.Invoke(this, new EventArgs());
		}

		public void ReNavigate()
		{
			this.UriRequested?.Invoke(this, this.Source);
		}
		public void CookieNavigate()
		{
			if (this.IsCookieEdit) return;
			if (this.SourceString.Contains("error/area/") && this.SourceString.Contains("dmm.com"))
			{
				GoKanColle();
				this.IsCookieEdit = true;
			}
		}

		public void Navigate()
		{
			Uri uri;
			if (this.UriRequested != null && Uri.TryCreate(this.SourceString, UriKind.Absolute, out uri))
			{
				this.UriRequested(this, uri);
			}
		}
		public void GoKanColle()
		{
			Uri uri;
			string SauceCookie;
			SauceCookie = "javascript:void(eval(\"document.cookie = 'cklg=ja;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=osapi.dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=203.104.209.7;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';\"));location.href=\"http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/\";";
			if (this.UriRequested != null && Uri.TryCreate(SauceCookie, UriKind.Absolute, out uri))
			{
				this.UriRequested(this, uri);
			}
		}
		public void Logout()
		{
			Uri uri;
			string LogoutUrl;
			LogoutUrl = "http://www.dmm.com/my/-/login/logout/=/path=Sg__/";
			if (this.UriRequested != null && Uri.TryCreate(LogoutUrl, UriKind.Absolute, out uri))
			{
				this.UriRequested(this, uri);
			}
		}
	}
}
