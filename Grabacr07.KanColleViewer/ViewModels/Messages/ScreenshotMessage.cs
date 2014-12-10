using Livet.Messaging;
using System.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Messages
{
	internal class ScreenshotMessage : ResponsiveInteractionMessage<Processing>
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
				Response = this.Response,
			};
		}
	}
}
