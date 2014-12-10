using Grabacr07.KanColleViewer.ViewModels.Messages;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	public class SetWindowLocationAction : InteractionMessageAction<Window>
	{
		protected override void InvokeAction(InteractionMessage message)
		{
			var setWindowLocMessage = message as SetWindowLocationMessage;
			if (setWindowLocMessage == null) return;

			if (setWindowLocMessage.Left.HasValue) this.AssociatedObject.Left = setWindowLocMessage.Left.Value;
			if (setWindowLocMessage.Top.HasValue) this.AssociatedObject.Top = setWindowLocMessage.Top.Value;
		}
	}
}
