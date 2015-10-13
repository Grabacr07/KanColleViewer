using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	public class MetroComboBox : ComboBox
	{
		private const string PART_Popup = "PART_Popup";

		private Popup popup;
		private double prevOffsetH;

		static MetroComboBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(MetroComboBox),
				new FrameworkPropertyMetadata(typeof(MetroComboBox)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (this.popup != null)
			{
				this.popup.Opened -= this.PopupOnOpened;
			}

			this.popup = this.GetTemplateChild(PART_Popup) as Popup;
			if (this.popup != null)
			{
				this.popup.Placement = PlacementMode.Relative;
				this.prevOffsetH = .0;

				this.popup.Opened += this.PopupOnOpened;
			}
		}

		private void SetOffset()
		{
			var height = .0;
			for (var i = 0; i < this.SelectedIndex; i++)
			{
				var container = this.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
				if (container == null) continue;

				height += LayoutInformation.GetLayoutSlot(container).Height;
			}

			this.popup.VerticalOffset -= this.prevOffsetH;
			this.popup.VerticalOffset += -height;

			this.prevOffsetH = -height;
		}

		private void PopupOnOpened(object sender, EventArgs eventArgs)
		{
			this.SetOffset();
		}
	}
}
