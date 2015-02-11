using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using AppSettings = Grabacr07.KanColleViewer.Properties.Settings;

namespace Grabacr07.KanColleViewer.Models
{
    public class WindowOrientaionMode : NotificationObject, IOrientationMode, IDisposable
	{
		public static bool EventSetted { get; private set; }
        public OrientationType[] SupportedModes
		{
			get {
                OrientationType[] r = { OrientationType.Auto, OrientationType.Horizontal, OrientationType.Vertical };
                return r;
            }
		}

		#region Current 変更通知プロパティ

        private OrientationType _Current;

        public OrientationType Current
		{
			get { return this._Current; }
			private set
			{
                if (this._Current != value)
				{
					ChangeWindowSize(value);
                    this._Current = value;
                    this.RaisePropertyChanged();
                    this.RaisePropertyChanged("CurrentModeString");
                }
			}
		}

		#endregion

        #region CurrentMode 変更通知プロパティ

        private OrientationType _CurrentMode;

        public OrientationType CurrentMode
        {
            get { return this._CurrentMode; }
            set
            {
                if (this._CurrentMode != value)
                {
                    this._CurrentMode = value;
                    if (value.Equals(OrientationType.Auto))
                    {
						if (!EventSetted)
							System.Windows.SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
						EventSetted = true;
						updateOrientationMode();
                    }
                    else
                    {
                        System.Windows.SystemParameters.StaticPropertyChanged -= SystemParameters_StaticPropertyChanged;
						EventSetted = false;
                        this.Current = value;
                    }
                    this.RaisePropertyChanged("CurrentModeString");
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion
		
		public void updateOrientationMode()
        {
            if (!this.CurrentMode.Equals(OrientationType.Auto)) return;

            if (System.Windows.SystemParameters.FullPrimaryScreenWidth >= System.Windows.SystemParameters.FullPrimaryScreenHeight)
            {
				this.Current = OrientationType.Horizontal;
            }
            else
            {
				this.Current = OrientationType.Vertical;
            }
        }

        private void SystemParameters_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("FullPrimaryScreenHeight") || e.PropertyName.Equals("FullPrimaryScreenWidth"))
            {
                updateOrientationMode();
            }
        }

        public string CurrentModeString
        {
            get {
                if (this.CurrentMode == OrientationType.Auto)
                {
                    if (this.Current == OrientationType.Horizontal) return "Ah";
                    else return "Av";
                }
                else
                {
                    if (this.Current == OrientationType.Horizontal) return "H";
                    else return "V";
                }
            }
        }

		public WindowOrientaionMode()
		{
		}

		public void Dispose()
		{
			System.Windows.SystemParameters.StaticPropertyChanged -= SystemParameters_StaticPropertyChanged;
		}

		private void ChangeWindowSize(OrientationType type, System.Windows.Window window)
		{
			if (type.Equals(OrientationType.Horizontal))
			{
				if (window != null && !this._Current.Equals(OrientationType.Horizontal))
				{
					if (window.WindowState == System.Windows.WindowState.Normal)
					{
						Settings.Current.VerticalSize = new System.Windows.Point(window.Width, window.Height);
					}

					window.Height = Settings.Current.HorizontalSize.Y;
					window.Width = Settings.Current.HorizontalSize.X;
				}
			}
			else
			{
				if (window != null && !this._Current.Equals(OrientationType.Vertical))
				{
					if (window.WindowState == System.Windows.WindowState.Normal)
					{
						Settings.Current.HorizontalSize = new System.Windows.Point(window.Width, window.Height);
					}

					window.Height = Settings.Current.VerticalSize.Y;
					window.Width = Settings.Current.VerticalSize.X;
				}
			}
		}

		private void ChangeWindowSize(OrientationType type)
		{
			try
			{
				ChangeWindowSize(type, System.Windows.Application.Current.MainWindow);
			}
			catch 
			{
			}
		}
	}
}
