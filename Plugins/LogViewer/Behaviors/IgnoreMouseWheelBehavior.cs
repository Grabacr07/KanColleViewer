using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Grabacr07.KanColleViewer.Plugins.Behaviors
{
    internal class IgnoreMouseWheelBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = UIElement.MouseWheelEvent };

            this.AssociatedObject.RaiseEvent(e2);

        }
    }
}
