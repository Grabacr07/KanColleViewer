using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleViewer.Views.Controls;
using Grabacr07.KanColleViewer.Win32;
using Livet;
using Livet.Commands;
using System.Runtime.InteropServices;

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

		public ViewModelCommand NavigateCommand
		{
			get
			{
				if (this._NavigateCommand == null)
				{
					this._NavigateCommand = new ViewModelCommand(this.Navigate);
				}
				return this._NavigateCommand;
			}
		}

		#endregion

		public event EventHandler GoBackRequested;
		public event EventHandler GoForwardRequested;
		public event EventHandler RefreshRequested;
		public event EventHandler<Uri> UriRequested;
        public event EventHandler ClearCacheRequested;

		public void GoBack()
		{
			if (this.GoBackRequested != null) this.GoBackRequested(this, new EventArgs());
		}

		public void GoForward()
		{
			if (this.GoForwardRequested != null) this.GoForwardRequested(this, new EventArgs());
		}

		public void Refresh()
		{
			if (this.UriRequested != null) this.RefreshRequested(this, new EventArgs());
		}

		public void ReNavigate()
		{
			if (this.UriRequested != null) this.UriRequested(this, this.Source);
		}

		public void Navigate()
		{
			Uri uri;
			if (this.UriRequested != null && Uri.TryCreate(this.SourceString, UriKind.Absolute, out uri))
			{
				this.UriRequested(this, uri);
			}
		}

        public void ClearCache()
        {
            if (this.ClearCacheRequested != null) this.ClearCacheRequested(this, new EventArgs());

            // No more items have been found.
            const int ERROR_NO_MORE_ITEMS = 259;

            // Local variables
            int cacheEntryInfoBufferSizeInitial = 0;
            int cacheEntryInfoBufferSize = 0;
            IntPtr cacheEntryInfoBuffer = IntPtr.Zero;
            DeleteCache.INTERNET_CACHE_ENTRY_INFOA internetCacheEntry;
            IntPtr enumHandle = IntPtr.Zero;
            bool returnValue = false;

            // Start to delete URLs that do not belong to any group.
            enumHandle = DeleteCache.FindFirstUrlCacheEntry(null, IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);
            if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                return;

            cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
            cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
            enumHandle = DeleteCache.FindFirstUrlCacheEntry(null, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

            while (true)
            {
                internetCacheEntry = (DeleteCache.INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(cacheEntryInfoBuffer, typeof(DeleteCache.INTERNET_CACHE_ENTRY_INFOA));

                cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;
                returnValue = DeleteCache.DeleteUrlCacheEntry(internetCacheEntry.lpszSourceUrlName);
                if (!returnValue)
                {
                    returnValue = DeleteCache.FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                }
                if (!returnValue && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                {
                    break;
                }
                if (!returnValue && cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
                {
                    cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                    cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, (IntPtr)cacheEntryInfoBufferSize);
                    returnValue = DeleteCache.FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                }
            }
            Marshal.FreeHGlobal(cacheEntryInfoBuffer);
        }
	}
}
