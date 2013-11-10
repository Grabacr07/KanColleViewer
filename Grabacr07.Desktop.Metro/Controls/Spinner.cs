using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Grabacr07.Desktop.Metro.Controls
{
	public class Spinner : Control
	{
		static Spinner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));
		}
	}
}
