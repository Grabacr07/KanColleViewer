using Grabacr07.KanColleViewer.Views.Controls;
using Livet.Behaviors.Messaging;
using Livet.Messaging;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	public class ZoomAction : InteractionMessageAction<KanColleHost>
	{
		protected override void InvokeAction(InteractionMessage message)
		{
			this.AssociatedObject.Update();
		}
	}
}
