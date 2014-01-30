using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Navigation;
using Grabacr07.KanColleViewer.Models;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	public class NavigatorBehavior : Behavior<WebBrowser>
	{
		#region Navigator 依存関係プロパティ

		public INavigator Navigator
		{
			get { return (INavigator)this.GetValue(NavigatorProperty); }
			set { this.SetValue(NavigatorProperty, value); }
		}
		public static readonly DependencyProperty NavigatorProperty =
			DependencyProperty.Register("Navigator", typeof(INavigator), typeof(NavigatorBehavior), new UIPropertyMetadata(null, NavigatorPropertyChangedCallback));

		private static void NavigatorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (NavigatorBehavior)d;
			var oldNavigator = (INavigator)e.OldValue;
			var newNavigator = (INavigator)e.NewValue;

			if (oldNavigator != null)
			{
				oldNavigator.UriRequested -= source.NavigatorOnUriRequested;
				oldNavigator.GoBackRequested -= source.NavigatorOnGoBackRequested;
				oldNavigator.GoForwardRequested -= source.NavigatorOnGoForwardRequested;
				oldNavigator.RefreshRequested -= source.NavigatorOnRefreshRequested;
			}
			if (newNavigator != null)
			{
				newNavigator.UriRequested += source.NavigatorOnUriRequested;
				newNavigator.GoBackRequested += source.NavigatorOnGoBackRequested;
				newNavigator.GoForwardRequested += source.NavigatorOnGoForwardRequested;
				newNavigator.RefreshRequested += source.NavigatorOnRefreshRequested;
			}
		}

		#endregion


		protected override void OnAttached()
		{
			base.OnAttached();
			this.AssociatedObject.LoadCompleted += this.AssociatedObjectOnLoadCompleted;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			this.AssociatedObject.LoadCompleted -= this.AssociatedObjectOnLoadCompleted;
		}

		private void AssociatedObjectOnLoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
		{
			if (this.Navigator != null)
			{
				this.Navigator.Source = navigationEventArgs.Uri;
				this.Navigator.CanGoBack = this.AssociatedObject.CanGoBack;
				this.Navigator.CanGoForward = this.AssociatedObject.CanGoForward;
			}
		}

		private void NavigatorOnUriRequested(object sender, Uri uri)
		{
			this.AssociatedObject.Navigate(uri);
		}

		private void NavigatorOnGoBackRequested(object sender, EventArgs eventArgs)
		{
			this.AssociatedObject.GoBack();
		}

		private void NavigatorOnGoForwardRequested(object sender, EventArgs eventArgs)
		{
			this.AssociatedObject.GoForward();
		}

		private void NavigatorOnRefreshRequested(object sender, EventArgs eventArgs)
		{
			this.AssociatedObject.Refresh();
		}

	}
}
