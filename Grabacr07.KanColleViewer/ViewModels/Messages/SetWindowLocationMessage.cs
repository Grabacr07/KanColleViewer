using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Livet.Messaging;

namespace Grabacr07.KanColleViewer.ViewModels.Messages
{
	public class SetWindowLocationMessage : InteractionMessage
	{
		#region Top 依存関係プロパティ

		public double? Top
		{
			get { return (double?)this.GetValue(TopProperty); }
			set { this.SetValue(TopProperty, value); }
		}
		public static readonly DependencyProperty TopProperty =
			DependencyProperty.Register("Top", typeof(double?), typeof(SetWindowLocationMessage), new UIPropertyMetadata(null));

		#endregion

		#region Left 依存関係プロパティ

		public double? Left
		{
			get { return (double?)this.GetValue(LeftProperty); }
			set { this.SetValue(LeftProperty, value); }
		}
		public static readonly DependencyProperty LeftProperty =
			DependencyProperty.Register("Left", typeof(double?), typeof(SetWindowLocationMessage), new UIPropertyMetadata(null));

		#endregion

		protected override Freezable CreateInstanceCore()
		{
			return new SetWindowLocationMessage
			{
				MessageKey = this.MessageKey,
				Top = this.Top,
				Left = this.Left,
			};
		}
	}
}
