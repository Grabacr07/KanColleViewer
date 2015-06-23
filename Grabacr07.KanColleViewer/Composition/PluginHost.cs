using System;
using System.Collections;
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
using Livet;

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

		private CompositionContainer container;
		private readonly List<Exception> unknownExceptions = new List<Exception>();
		private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

		private Dictionary<Guid, Plugin> loadedPlugins;

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

		/// <summary>
		/// プラグインの初期化処理 (<see cref="PluginHost.Initialize"/> メソッド) の結果を示す識別子を定義します。
		/// </summary>
		public enum InitializationResult
		{
			/// <summary>
			/// プラグインの初期化に成功しました。
			/// </summary>
			Succeeded,

			/// <summary>
			/// プラグインの初期化に失敗したため、アプリケーションを起動できません。
			/// </summary>
			Failed,

			/// <summary>
			/// プラグインの初期化に失敗したため、アプリケーションの再起動が必要です。
			/// </summary>
			RequiresRestart,
		}


		public IReadOnlyCollection<Exception> UnknownExceptions
		{
			get { return this.unknownExceptions; }
		}


		private PluginHost() { }


		/// <summary>
		/// プラグインをロードし、初期化を行います。
		/// </summary>
		public InitializationResult Initialize()
		{
			var currentDir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
			if (currentDir == null) return InitializationResult.Failed;

			var pluginsDir = Path.Combine(currentDir, PluginsDirectory);
			var ignores = Settings.Current.BlacklistedPlugins.ToArray();
			var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			var plugins = Directory.EnumerateFiles(pluginsDir, "*.dll", SearchOption.AllDirectories);

			Settings.Current.BlacklistedPlugins.Clear();

			foreach (var plugin in plugins)
			{
				// プラグインのパスのコレクションから、前回起動に失敗したプラグインを除外して
				// アセンブリ カタログとして結合していく

				var isIgnore = false;
				var filepath = plugin;
				foreach (var data in ignores.Where(x => string.Compare(x.FilePath, filepath, StringComparison.OrdinalIgnoreCase) == 0))
				{
					Settings.Current.BlacklistedPlugins.Add(data);
					isIgnore = true;
				}
				if (isIgnore) continue;

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

			return InitializationResult.Succeeded;
		}

		public void Dispose()
		{
			this.container.Dispose();

			//if (this.initializationResult == InitializationResult.Succeeded)
			//{
			//	this.GetNotifier().Dispose();
			//}
		}

		/// <summary>
		/// ロードされている全ての通知機能を集約して操作するオブジェクトを返します。
		/// </summary>
		/// <returns>ロードされている全ての通知機能を集約して操作するオブジェクト。</returns>
		public INotifier GetNotifier()
		{
			return new AggregateNotifier(Cache<INotifier>.Plugins);
		}

		private static class Cache<TContract>
		{
			private static List<TContract> plugins;

			public static List<TContract> Plugins
			{
				get { return plugins ?? (plugins = new List<TContract>()); }
			}
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

				plugin.AddFunction(lazy.Value);
				Cache<TContract>.Plugins.Add(lazy.Value);
			}
		}
	}

	internal class Plugin
	{
		private readonly Dictionary<Type, object> functions = new Dictionary<Type, object>();

		public Guid Id { get; private set; }

		public PluginMetadata Metadata { get; private set; }

		public Plugin(IPluginMetadata metadata)
		{
			this.Id = new Guid(metadata.Guid);
			this.Metadata = new PluginMetadata
			{
				Title = metadata.Title,
				Description = metadata.Description,
				Version = metadata.Version,
				Author = metadata.Author,
			};
		}

		public void AddFunction<TContract>(TContract function) where TContract : class
		{
			this.functions.Add(typeof(TContract), function);
		}

		public TContract GetFunction<TContract>() where TContract : class
		{
			object function;
			return this.functions.TryGetValue(typeof(TContract), out function) ? (TContract)function : null;
		}
	}
}
