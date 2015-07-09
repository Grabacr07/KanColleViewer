using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// プラグインを読み込み、機能へのアクセスを提供します。
	/// </summary>
	internal class PluginHost : IDisposable
	{
		/// <summary>
		/// プラグインを検索するディレクトリへの相対パスを表す文字列を取得します。
		/// </summary>
		/// <value>プラグインを検索するディレクトリへの相対パスを表す文字列。</value>
		public const string PluginsDirectory = "Plugins";

		#region singleton members

		/// <summary>
		/// <see cref="PluginHost"/> のインスタンスを取得します。
		/// </summary>
		/// <value><see cref="PluginHost"/> のインスタンス。</value>
		public static PluginHost Instance { get; } = new PluginHost();

		#endregion


		private CompositionContainer container;
		private Dictionary<Guid, Plugin> loadedPlugins;
		private readonly List<Exception> unknownExceptions = new List<Exception>();

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

#pragma warning restore 649

		private static class Cache<TContract>
		{
			private static List<TContract> plugins;

			public static List<TContract> Plugins => plugins ?? (plugins = new List<TContract>());
		}

		public Plugin[] Plugins => this.loadedPlugins.Values.ToArray();

		public IReadOnlyCollection<Exception> UnknownExceptions => this.unknownExceptions;


		private PluginHost() { }


		/// <summary>
		/// プラグインをロードし、初期化を行います。
		/// </summary>
		public void Initialize()
		{
			var currentDir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
			if (currentDir == null) return;

			var pluginsDir = Path.Combine(currentDir, PluginsDirectory);
			if (!Directory.Exists(pluginsDir))
			{
				this.loadedPlugins = new Dictionary<Guid, Plugin>();
				return;
			}

			var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			var plugins = Directory.EnumerateFiles(pluginsDir, "*.dll", SearchOption.AllDirectories);

			Settings.Current.BlacklistedPlugins.Clear();

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
					Settings.Current.BlacklistedPlugins.Add(new BlacklistedPluginData
					{
						FilePath = filepath,
						Exception = ex.LoaderExceptions.Select(x => x.Message).ToString(Environment.NewLine),
					});
				}
				catch (BadImageFormatException ex)
				{
					Settings.Current.BlacklistedPlugins.Add(new BlacklistedPluginData
					{
						FilePath = filepath,
						Exception = ex.ToString(),
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
					foreach (var cause in ex.RootCauses)
					{
						var partException = cause as ComposablePartException;
						if (partException != null)
						{
							var asmCatalog = partException.Element.Origin as AssemblyCatalog;
							if (asmCatalog != null)
							{
								// AggregateCatalog に AssemblyCatalog しか Add してないんだから全部ここに来るはず…？

								Settings.Current.BlacklistedPlugins.Add(new BlacklistedPluginData
								{
									FilePath = asmCatalog.Assembly.Location,
									Metadata = plugin.Metadata,
									Exception = ex.Message,
								});
							}
						}
						else
						{
							// 他にプラグインのロード失敗時の例外パターンが判れば追加していきたく
							this.unknownExceptions.Add(cause);
						}
					}
				}
				catch (Exception ex)
				{
					this.unknownExceptions.Add(ex);
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
					// ToDo: PluginHost に読み込み失敗プラグイン一覧を作って、この ex もそこに放り込む
				}
			}
		}

		public void Dispose()
		{
			this.container?.Dispose();
		}
	}
}
