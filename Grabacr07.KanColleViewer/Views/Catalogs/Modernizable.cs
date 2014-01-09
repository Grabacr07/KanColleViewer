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

namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	/// <summary>
	/// <see cref="ModernizableStatus"/> の表示をサポートします。
	/// </summary>
	public class Modernizable : Control
	{
		static Modernizable()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Modernizable), new FrameworkPropertyMetadata(typeof(Modernizable)));
		}

		
		#region Value 依存関係プロパティ

		public ModernizableStatus Status
		{
			get { return (ModernizableStatus)this.GetValue(ValueProperty); }
			set { this.SetValue(ValueProperty, value); }
		}
		/// <summary>
		/// <see cref="Status"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Status", typeof(ModernizableStatus), typeof(Modernizable), new UIPropertyMetadata(new ModernizableStatus()));

		#endregion

	}
}
