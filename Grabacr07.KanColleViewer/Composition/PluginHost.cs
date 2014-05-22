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

		private static readonly PluginHost instance = new PluginHost();

		private readonly CompositionContainer container;

		/// <summary>
		/// <see cref="PluginHost"/> のインスタンスを取得します。
		/// </summary>
		/// <value><see cref="PluginHost"/> のインスタンス。</value>
		public static PluginHost Instance { get { return instance; } }

		/// <summary>
		/// プラグインによって提供される通知機能を表すオブジェクトのシーケンスを取得します。
		/// </summary>
		/// <value>プラグインによって提供される通知機能を表すオブジェクトのシーケンス。</value>
		[ImportMany(typeof(INotifier))]
		public IEnumerable<INotifier> Notifiers { get; set; }

		private PluginHost()
		{
			var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			if (Directory.Exists(PluginsDirectory))
			{
				catalog.Catalogs.Add(new DirectoryCatalog("Plugins"));
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
		}

		/// <summary>
		/// ロードされている全ての通知機能を集約して操作するオブジェクトを返します。
		/// </summary>
		/// <returns>ロードされている全ての通知機能を集約して操作するオブジェクト。</returns>
		public INotifier GetNotifier()
		{
			return new AggregateNotifier(this.Notifiers);
		}
	}
}
