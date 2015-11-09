using System.Windows;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	public class CollapsibleGridViewColumn : GridViewColumn
	{
		public Visibility Visibility
		{
			get
			{
				return (Visibility)GetValue(VisibilityProperty);
			}
			set
			{
				SetValue(VisibilityProperty, value);
			}
		}

		public static readonly DependencyProperty VisibilityProperty =
			DependencyProperty.Register("Visibility", typeof(Visibility),
			typeof(CollapsibleGridViewColumn),
			new FrameworkPropertyMetadata(Visibility.Visible,
			FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
			OnVisibilityPropertyChanged));

		private static void OnVisibilityPropertyChanged(DependencyObject d,
									  DependencyPropertyChangedEventArgs e)
		{
			(d as CollapsibleGridViewColumn)?.OnVisibilityChanged((Visibility)e.NewValue);
		}

		private void OnVisibilityChanged(Visibility visibility)
		{
			if (visibility == Visibility.Visible)
			{
				Width = visibleWidth;
				CellTemplate = visibleTemplate;
			}
			else
			{
				visibleWidth = Width;
				visibleTemplate = CellTemplate;
				Width = 0.0;
				CellTemplate = new DataTemplate();
			}
		}

		private double visibleWidth;
		private DataTemplate visibleTemplate;
	}
}