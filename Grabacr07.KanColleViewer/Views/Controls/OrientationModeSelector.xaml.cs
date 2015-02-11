using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using Livet;
using Livet.EventListeners;
using MetroRadiance.Core;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// OrientationModeSelector.xaml の相互作用ロジック
	/// </summary>
	public partial class OrientationModeSelector
    {
        private Dpi? systemDpi;
        private List<OrientationModeSelectorItem> items;
        private IDisposable orientationModeNotifyListener;

        public OrientationModeSelector()
		{
			InitializeComponent();

            this.Popup.CustomPopupPlacementCallback = this.PopupPlacementCallback;
            this.Popup.Opened += (sender, args) => this.ChangeBackground();
            this.Popup.Closed += (sender, args) => this.ChangeBackground();
		}

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            this.ChangeBackground();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            this.ChangeBackground();
        }


        private CustomPopupPlacement[] PopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            var dpi = this.systemDpi ?? (this.systemDpi = this.GetSystemDpi()) ?? Dpi.Default;
            return new[]
			{
				new CustomPopupPlacement(new Point(offset.X * dpi.ScaleX, offset.Y* dpi.ScaleY), PopupPrimaryAxis.None),
			};
        }

        private void ChangeBackground()
        {
            if (this.Popup.IsOpen)
            {
                try
                {
                    this.Background = this.FindResource("AccentBrushKey") as Brush;
                }
                catch (ResourceReferenceKeyNotFoundException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else if (this.IsMouseOver)
            {
                try
                {
                    this.Background = this.FindResource("ActiveBackgroundBrushKey") as Brush;
                }
                catch (ResourceReferenceKeyNotFoundException ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            else
            {
                this.Background = Brushes.Transparent;
            }
        }

        internal class OrientationModeSelectorItem : NotificationObject
        {
            public Action SelectAction { get; set; }
            public OrientationType Value { get; set; }

            #region IsSelected 変更通知プロパティ

            private bool _IsSelected;

            public bool IsSelected
            {
                get { return this._IsSelected; }
                set
                {
                    if (this._IsSelected != value)
                    {
                        this._IsSelected = value;
                        this.RaisePropertyChanged();
                    }
                }
            }

            #endregion

            public void Select()
            {
                if (this.SelectAction != null) this.SelectAction();
            }
        }

        #region OrientationMode 依存関係プロパティ

        public IOrientationMode OrientationMode
        {
            get { return (IOrientationMode)this.GetValue(OrientationModeProperty); }
            set { this.SetValue(OrientationModeProperty, value); }
        }
        public static readonly DependencyProperty OrientationModeProperty =
            DependencyProperty.Register("OrientationMode", typeof(IOrientationMode), typeof(OrientationModeSelector), new UIPropertyMetadata(null, OrientationModePropertyChangedCallback));

        private static void OrientationModePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (OrientationModeSelector)d;
            var newValue = (IOrientationMode)e.NewValue;

            if (source.orientationModeNotifyListener != null)
            {
                source.orientationModeNotifyListener.Dispose();
            }

            if (newValue != null)
            {
                var dpi = source.systemDpi ?? (source.systemDpi = source.GetSystemDpi()) ?? Dpi.Default;
                source.items = newValue.SupportedModes
                    .Select(x => new OrientationModeSelectorItem
                    {
                        Value = x,
                        IsSelected = x.Equals(newValue.CurrentMode),
                        SelectAction = () => newValue.CurrentMode = x,
                    })
                    .ToList();
                source.SupportedList.ItemsSource = source.items;
            }

            var notifySource = newValue as INotifyPropertyChanged;
            if (notifySource != null)
            {
                source.orientationModeNotifyListener = new PropertyChangedEventListener(notifySource)
				{
					{
						"CurrentMode",
						(sender, args) =>
						{
                            var target = source.items.FirstOrDefault(x => x.Value == newValue.CurrentMode);
							if (target != null) target.IsSelected = true;
						}
					}
				};
            }
        }

        #endregion
	}
}
