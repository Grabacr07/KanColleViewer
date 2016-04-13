using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// プラグインを読み込み、機能へのアクセスを提供します。
	/// </summary>
	internal class PluginService : IDisposable
	{
		/// <summary>
		/// プラグインを検索するディレクトリへの相対パスを表す文字列を取得します。
		/// </summary>
		/// <value>プラグインを検索するディレクトリへの相対パスを表す文字列。</value>
		public const string PluginsDirectory = "Plugins";

		#region singleton members

		/// <summary>
		/// <see cref="PluginService"/> のインスタンスを取得します。
		/// </summary>
		/// <value><see cref="PluginService"/> のインスタンス。</value>
		public static PluginService Current { get; } = new PluginService();

		#endregion


		private CompositionContainer container;
		private Dictionary<Guid, Plugin> loadedPlugins;
		private readonly List<LoadFailedPluginData> failedPlugins = new List<LoadFailedPluginData>();

#pragma warning disable 649

		[ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
		private IEnumerable<Lazy<IPlugin, IPluginMetadata>> importedAll;

		[ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
		private IEnumerable<Lazy<ISettings, IPluginGuid>> importedSettings;

		[ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
		private IEnumerable<Lazy<INotifier, IPluginGuid>> importedNotifiers;

		[ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
		private IEnumerable<Lazy<IRequestNotify, IPluginGuid>> importedNotifyRequesters;

		[ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
		private IEnumerable<Lazy<ITool, IPluginGuid>> importedTools;

		[ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
		private IEnumerable<Lazy<ILocalizable, IPluginGuid>> importedLocalizables;

		[ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
		private IEnumerable<Lazy<ITaskbarProgress, IPluginGuid>> importedTaskbarProgress;

#pragma warning restore 649

		private static class Cache<TContract>
		{
			private static List<TContract> plugins;

			public static List<TContract> Plugins => plugins ?? (plugins = new List<TContract>());
		}

		/// <summary>
		/// 読み込まれたすべてのプラグインを配列で取得します。
		/// </summary>
		public Plugin[] Plugins => this.loadedPlugins.Values.ToArray();

		/// <summary>
		/// 何らかの原因で読み込みに失敗したプラグインの情報を配列で取得します。
		/// </summary>
		public LoadFailedPluginData[] FailedPlugins => this.failedPlugins.ToArray();


		private PluginService() { }


		/// <summary>
		/// プラグインをロードし、初期化を行います。
		/// </summary>
		public void Initialize()
		{
			var currentDir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
			if (currentDir == null) // なんじゃそりゃ
			{
				this.loadedPlugins = new Dictionary<Guid, Plugin>();
				return;
			}

			var pluginsDir = Path.Combine(currentDir, PluginsDirectory);
			if (!Directory.Exists(pluginsDir))
			{
				this.loadedPlugins = new Dictionary<Guid, Plugin>();
				return;
			}

			var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			var plugins = Directory.EnumerateFiles(pluginsDir, "*.dll", SearchOption.AllDirectories);

			foreach (var plugin in plugins)
			{
				var filepath = plugin;
				try
				{
					var asmCatalog = new AssemblyCatalog(filepath);
					if (asmCatalog.Parts.ToList().Count > 0)
					{
						catalog.Catalogs.Add(asmCatalog);
					}
				}
				catch (ReflectionTypeLoadException ex)
				{
					this.failedPlugins.Add(new LoadFailedPluginData
					{
						FilePath = filepath,
						Message = ex.LoaderExceptions.Select(x => x.Message).ToString(Environment.NewLine),
					});
				}
				catch (BadImageFormatException ex)
				{
					this.failedPlugins.Add(new LoadFailedPluginData
					{
						FilePath = filepath,
						Message = ex.ToString(),
					});
				}
				catch (FileLoadException ex)
				{
					this.failedPlugins.Add(new LoadFailedPluginData
					{
						FilePath = filepath,
						Message = ex.ToString(),
					});
				}
			}

			this.container = new CompositionContainer(catalog);
			this.container.ComposeParts(this);

			this.loadedPlugins = this.Load(this.importedAll).ToDictionary(x => x.Id);

			this.Load(this.importedSettings);
			this.Load(this.importedNotifiers);
			this.Load(this.importedNotifyRequesters);
			this.Load(this.importedTools);
			this.Load(this.importedLocalizables);
			this.Load(this.importedTaskbarProgress);
		}

		/// <summary>
		/// ロードされている全ての通知機能を集約して操作するオブジェクトを返します。
		/// </summary>
		/// <returns>ロードされている全ての通知機能を集約して操作するオブジェクト。</returns>
		public INotifier GetNotifier()
		{
			return new AggregateNotifier(Cache<INotifier>.Plugins);
		}

		/// <summary>
		/// 指定したコントラクト型に一致するプラグイン機能のインポートがある場合、それらを含む配列を返します。
		/// </summary>
		/// <typeparam name="TContract">取得するプラグイン機能のコントラクト型。</typeparam>
		/// <returns><typeparamref name="TContract"/> コントラクト型に一致するプラグイン機能の配列。</returns>
		public TContract[] Get<TContract>()
		{
			return Cache<TContract>.Plugins.ToArray();
		}


		private IEnumerable<Plugin> Load(IEnumerable<Lazy<IPlugin, IPluginMetadata>> imported)
		{
			var ids = new HashSet<Guid>();

			foreach (var lazy in imported)
			{
				Guid guid;
				if (!Guid.TryParse(lazy.Metadata.Guid, out guid)) continue;

				var plugin = new Plugin(lazy.Metadata);
				var success = false;

				try
				{
					lazy.Value.Initialize();
					success = true;
				}
				catch (CompositionException ex)
				{
					var failds = ex.RootCauses
						.Select(x => x as ComposablePartException)
						.Select(x => x?.Element.Origin as AssemblyCatalog)
						.Select(x => new LoadFailedPluginData
						{
							FilePath = x?.Assembly.Location,
							Metadata = plugin.Metadata,
							Message = ex.ToString(),
						});
					this.failedPlugins.AddRange(failds);
				}
				catch (Exception ex)
				{
					this.failedPlugins.Add(new LoadFailedPluginData
					{
						Metadata = plugin.Metadata,
						Message = ex.ToString(),
					});
				}

				if (!ids.Add(plugin.Id))
				{
					this.failedPlugins.Add(new LoadFailedPluginData
					{
						Metadata = plugin.Metadata,
						Message = "プラグインの ID が重複しています。" + Environment.NewLine + "プラグインには一意の GUID が必要です。プラグインの開発者に連絡してください。",
					});
					success = false;
				}

				if (success)
				{
					yield return plugin;
				}
			}
		}

		private void Load<TContract>(IEnumerable<Lazy<TContract, IPluginGuid>> imported) where TContract : class
		{
			foreach (var lazy in imported)
			{
				Guid guid;
				if (!Guid.TryParse(lazy.Metadata.Guid, out guid)) continue;

				Plugin plugin;
				if (!this.loadedPlugins.TryGetValue(guid, out plugin)) continue;

				try
				{
					var function = lazy.Value;

					plugin.Add(function);
					Cache<TContract>.Plugins.Add(function);
				}
				catch (Exception ex)
				{
					this.failedPlugins.Add(new LoadFailedPluginData
					{
						Metadata = plugin.Metadata,
						Message = ex.ToString(),
					});
				}
			}
		}

		public void Dispose()
		{
			this.container?.Dispose();
		}
	}
}
