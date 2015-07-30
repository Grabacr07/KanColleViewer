using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleViewer.Composition
{
	public class NotifyEventArgs : EventArgs, INotification
	{
		public string Type { get; }

		public string Header { get; }

		public string Body { get; }

		public Action Activated { get; set; }

		public Action<Exception> Failed { get; set; }

		public NotifyEventArgs(string type, string header, string body)
		{
			this.Type = type;
			this.Header = header;
			this.Body = body;
		}
	}
}
