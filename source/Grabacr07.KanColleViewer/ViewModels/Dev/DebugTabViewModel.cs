using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Web;

using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.ViewModels.Dev
{
	public class RequestResponse
	{
		public string PathAndQuery { get; set; }
		public string Request { get; set; }
		public string Response { get; set; }
	}

	public class DebugTabViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Properties.Resources.Debug; }
			protected set { throw new NotImplementedException(); }
		}

		private RequestResponse[] _listData { get; set; }
		public RequestResponse[] listData
		{
			get { return this._listData; }
			set
			{
				if (this._listData != value)
				{
					this._listData = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private bool _Capture { get; set; }
		public bool Capture
		{
			get { return this._Capture; }
			set
			{
				if (this._Capture != value)
				{
					this._Capture = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public DebugTabViewModel()
		{
			this.listData = new RequestResponse[0];

			var api = KanColleWrapper.KanColleClient.Current.Proxy.ApiSessionSource;
			api.TryParse();
			api.Subscribe(x =>
			{
				if (!Capture) return;

				var list = this.listData.ToList();

				var y = new RequestResponse();
				y.PathAndQuery = x.Request.PathAndQuery;
				y.Request = ProcessBody(x.Request.Body);
				y.Response = ProcessBody(x.Response.Body);

				list.Add(y);

				this.listData = list.ToArray();
			});
		}

		private string ProcessBody(byte[] data)
		{
			var x = System.Text.Encoding.UTF8.GetString(data, 0, data.Length).Replace("svdata=", "");
			x = System.Uri.UnescapeDataString(x);
			return x;
		}

		public void Clear()
		{
			this.listData = new RequestResponse[0];
		}
	}
}
