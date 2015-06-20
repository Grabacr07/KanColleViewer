using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
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
