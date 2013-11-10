using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using Grabacr07.Desktop.Metro.Chrome;

namespace Grabacr07.Desktop.Metro.Behaviors
{
	internal class DirectWindowAction : TriggerAction<FrameworkElement>
	{
		#region WindowAction 依存関係プロパティ

		public WindowAction WindowAction
		{
			get { return (WindowAction)this.GetValue(WindowActionProperty); }
			set { this.SetValue(WindowActionProperty, value); }
		}

		public static readonly DependencyProperty WindowActionProperty =
			DependencyProperty.Register("WindowAction", typeof (WindowAction), typeof (DirectWindowAction), new UIPropertyMetadata(WindowAction.Active));

		#endregion

		protected override void Invoke(object parameter)
		{
			this.WindowAction.Invoke(this.AssociatedObject);
		}
	}
}
