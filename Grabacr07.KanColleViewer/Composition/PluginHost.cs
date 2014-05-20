using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	public class PluginHost : IDisposable
	{
		public const string PluginsDirectory = "Plugins";

		private static readonly PluginHost _instance = new PluginHost();

		private readonly CompositionContainer _container;

		public static PluginHost Instance { get { return _instance; } }

		[ImportMany]
		public IEnumerable<INotifier> Notifiers { get; set; }

		private PluginHost()
		{
			var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			if (Directory.Exists(PluginsDirectory))
			{
				catalog.Catalogs.Add(new DirectoryCatalog("Plugins"));
			}
			this._container = new CompositionContainer(catalog);
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
