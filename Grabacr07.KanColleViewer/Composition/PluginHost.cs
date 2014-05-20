using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	public class PluginHost : IDisposable
	{
		private static readonly PluginHost _instance = new PluginHost();

		private readonly CompositionContainer _container;

		public static PluginHost Instance { get { return _instance; } }

		[ImportMany]
		public IList<INotifier> Notifiers { get; set; }

		public PluginHost()
		{
			this._container = new CompositionContainer(new AggregateCatalog(
				new AssemblyCatalog(Assembly.GetExecutingAssembly()),
				new DirectoryCatalog("Plugins")
			));
		}

		public void Dispose()
		{
			this._container.Dispose();
			this.GetNotifier().Dispose();
		}

		public void Initialize()
		{
			this._container.ComposeParts(this);
			this.GetNotifier().Initialize();
		}

		public INotifier GetNotifier()
		{
			return new AggregateNotifier(this.Notifiers);
		}
	}
}
