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
	/// <summary>
	/// プラグインを読み込み、機能へのアクセスを提供します。
	/// </summary>
	public class PluginHost : IDisposable
	{
		/// <summary>
		/// プラグインを検索するディレクトリへの相対パスを表す文字列を取得します。
		/// </summary>
		/// <value>プラグインを検索するディレクトリへの相対パスを表す文字列。</value>
		public const string PluginsDirectory = "Plugins";

		#region singleton members

		private static readonly PluginHost instance = new PluginHost();

		/// <summary>
		/// <see cref="PluginHost"/> のインスタンスを取得します。
		/// </summary>
		/// <value><see cref="PluginHost"/> のインスタンス。</value>
		public static PluginHost Instance
		{
			get { return instance; }
		}

		#endregion

		private readonly CompositionContainer container;

		/// <summary>
		/// プラグインによって提供される通知機能を表すオブジェクトのシーケンスを取得します。
		/// </summary>
		[ImportMany]
		public IEnumerable<Lazy<INotifier, IPluginMetadata>> Notifiers { get; set; }

		/// <summary>
		/// プラグインによって提供されるツール機能を表すオブジェクトのシーケンスを取得します。
		/// </summary>
		[ImportMany]
		public IEnumerable<Lazy<IToolPlugin, IPluginMetadata>> Tools { get; set; }

		/// <summary>
		/// プラグインによって提供される拡張機能を表すオブジェクトのシーケンスを取得します。
		/// </summary>
		[ImportMany]
		public IEnumerable<Lazy<IPlugin, IPluginMetadata>> Plugins { get; set; }

		/// <summary>
		/// ロートされているすべてのプラグインのシーケンスを取得します。
		/// </summary>
		public IEnumerable<IPlugin> All
		{
			get
			{
				return this.Plugins.Select(x => x.Value)
					.Concat(this.Tools.Select(x => x.Value))
					.Concat(this.Notifiers.Select(x => x.Value));
			}
		}

		/// <summary>
		/// ロードに失敗したプラグインのリストを取得します。
		/// </summary>
		public List<LoadFailurePluginData> LoadFailurePlugins { get; private set; }


		private PluginHost()
		{
			this.LoadFailurePlugins = new List<LoadFailurePluginData>();

			var currentDir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
			if (currentDir == null) return;

			var pluginsDir = Path.Combine(currentDir, PluginsDirectory);
			var plugins = Directory.EnumerateFileSystemEntries(pluginsDir, "*.dll", SearchOption.AllDirectories);
			var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

			foreach (var plugin in plugins)
			{
				try
				{
					var asmCatalog = new AssemblyCatalog(plugin);
					if (asmCatalog.Parts.ToList().Count > 0)
					{
						catalog.Catalogs.Add(asmCatalog);
					}
				}
				catch (ReflectionTypeLoadException ex)
				{
					this.LoadFailurePlugins.Add(new LoadFailurePluginData
					{
						Filename = Path.GetFileNameWithoutExtension(plugin),
						Exception = ex,
					});
				}
				catch (BadImageFormatException ex)
				{
					this.LoadFailurePlugins.Add(new LoadFailurePluginData
					{
						Filename = Path.GetFileNameWithoutExtension(plugin),
						Exception = ex,
					});
				}
			}

			this.container = new CompositionContainer(catalog);
		}

		public void Dispose()
		{
			this.container.Dispose();
			this.GetNotifier().Dispose();
		}

		/// <summary>
		/// プラグインをロードし、初期化を行います。
		/// </summary>
		public void Initialize()
		{
			this.container.ComposeParts(this);
			this.GetNotifier().Initialize();

			// この時点で失敗したら、
			// this.container = new CompositionContainer(catalog); をやり直せばいいのでは？

			this.CheckPlugins();
		}

		/// <summary>
		/// ロードされている全ての通知機能を集約して操作するオブジェクトを返します。
		/// </summary>
		/// <returns>ロードされている全ての通知機能を集約して操作するオブジェクト。</returns>
		public INotifier GetNotifier()
		{
			return new AggregateNotifier(this.Notifiers.Select(x => x.Value));
		}

		private void CheckPlugins()
		{
			//foreach (var plugin in this.All)
			//{
			//	plugin.GetSettingsView();
			//}

			foreach (var plugin in this.Tools)
			{
				try
				{
					var p = plugin.Value;
				}
				catch (Exception ex)
				{
					
				}
			}
		}
	}
}
