using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class ColorIndicator : ProgressBar
	{
		static ColorIndicator()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(ColorIndicator),
				new FrameworkPropertyMetadata(typeof(ColorIndicator)));
		}

		#region LimitedValue 依存関係プロパティ

		public LimitedValue LimitedValue
		{
			get { return (LimitedValue)this.GetValue(LimitedValueProperty); }
			set { this.SetValue(LimitedValueProperty, value); }
		}
		public static readonly DependencyProperty LimitedValueProperty =
			DependencyProperty.Register(nameof(LimitedValue), typeof(LimitedValue), typeof(ColorIndicator), new UIPropertyMetadata(new LimitedValue(), LimitedValuePropertyChangedCallback));

		private static void LimitedValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (ColorIndicator)d;
			var value = (LimitedValue)e.NewValue;

			source.ChangeColor(value);
		}

		#endregion

		#region Columns 依存関係プロパティ

		public int Columns
		{
			get { return (int)this.GetValue(ColumnsProperty); }
			set { this.SetValue(ColumnsProperty, value); }
		}
		public static readonly DependencyProperty ColumnsProperty =
			DependencyProperty.Register(nameof(Columns), typeof(int), typeof(ColorIndicator), new UIPropertyMetadata(4));

		#endregion

		private void ChangeColor(LimitedValue value)
		{
			this.Maximum = value.Maximum;
			this.Minimum = value.Minimum;
			this.Value = value.Current;

			Color color;
			var percentage = value.Maximum == 0 ? 0.0 : value.Current / (double)value.Maximum;

			// 0.25 以下のとき、「大破」
			if (percentage <= 0.25) color = Color.FromRgb(255, 32, 32);

			// 0.5 以下のとき、「中破」
			else if (percentage <= 0.5) color = Color.FromRgb(240, 128, 32);

			// 0.75 以下のとき、「小破」
			else if (percentage <= 0.75) color = Color.FromRgb(240, 240, 0);

			// 0.75 より大きいとき、「小破未満」
			else color = Color.FromRgb(64, 200, 32);

			this.Foreground = new SolidColorBrush(color);
		}
	}
}
