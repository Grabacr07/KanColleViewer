using System;
using System.Net;
using System.Reflection;

namespace Grabacr07.KanColleWrapper
{
	public class ViewerWebClient : WebClient
	{
		private System.Net.CookieContainer cookieContainer;
		private string userAgent;
		private int timeout;

		public System.Net.CookieContainer CookieContainer
		{
			get { return cookieContainer; }
			set { cookieContainer = value; }
		}

		public string UserAgent
		{
			get { return userAgent; }
			set { userAgent = value; }
		}

		public int Timeout
		{
			get { return timeout; }
			set { timeout = value; }
		}

		public ViewerWebClient()
		{
			timeout = 18000;
			var assemblyName = Assembly.GetEntryAssembly().GetName();
			var wrapperName = typeof(ViewerWebClient).Assembly.GetName();

			userAgent = string.Format("{0}/{1} (compatible; {2}/{3}; smooth and flat)", wrapperName.Name, wrapperName.Version, assemblyName.Name, assemblyName.Version);
			cookieContainer = new CookieContainer();

			//cookieContainer.Add(new Cookie("example", "example_value"));
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);

			if (request?.GetType() == typeof(HttpWebRequest))
			{
				((HttpWebRequest)request).CookieContainer = cookieContainer;
				((HttpWebRequest)request).UserAgent = userAgent;
				((HttpWebRequest)request).Timeout = timeout;
			}

			return request;
		}

	}
}