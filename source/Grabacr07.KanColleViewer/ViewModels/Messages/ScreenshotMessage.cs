using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Livet.Messaging;

namespace Grabacr07.KanColleViewer.ViewModels.Messages
{
	internal class ScreenshotMessage : InteractionMessage
	{
		public ScreenshotMessage() { }
		public ScreenshotMessage(string messageKey) : base(messageKey) { }

		public string Path { get; set; }

		protected override Freezable CreateInstanceCore()
		{
			return new ScreenshotMessage()
			{
				MessageKey = this.MessageKey,
				Path = this.Path,
			};
		}
	}
}
