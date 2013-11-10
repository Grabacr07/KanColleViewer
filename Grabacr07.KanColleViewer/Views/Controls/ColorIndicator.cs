using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class ColorIndicator : ProgressBar
	{
		//static ColorIndicator()
		//{
		//	DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorIndicator), new FrameworkPropertyMetadata(typeof(ColorIndicator)));
		//}

		#region LimitedValue 依存関係プロパティ

		public LimitedValue LimitedValue
		{
			get { return (LimitedValue)this.GetValue(LimitedValueProperty); }
			set { this.SetValue(LimitedValueProperty, value); }
		}
		public static readonly DependencyProperty LimitedValueProperty =
			DependencyProperty.Register("LimitedValue", typeof(LimitedValue), typeof(ColorIndicator), new UIPropertyMetadata(new LimitedValue(), LimitedValuePropertyChangedCallback));

		private static void LimitedValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (ColorIndicator)d;
			var value = (LimitedValue)e.NewValue;

			source.ChangeColor(value);
		}

		#endregion


		private void ChangeColor(LimitedValue value)
		{
			this.Maximum = value.Maximum;
			this.Minimum = value.Minimum;
			this.Value = value.Current;

			var percentage = value.Current / (double)value.Maximum;
			var color = percentage > 0.5
				? Color.FromRgb((byte)(180 - 140 * ((percentage - 0.5) * 2)), 180, 0)
				: Color.FromRgb(180, (byte)(40 + 140 * (percentage * 2)), 0);

			this.Foreground = new SolidColorBrush(color);
		}
	}
}
