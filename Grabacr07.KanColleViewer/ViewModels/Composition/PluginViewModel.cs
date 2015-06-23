using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class PluginViewModel : ViewModel
	{
		private readonly INotifier notifier;

		protected Plugin Plugin { get; private set; }

		#region metadata

		public string Title
		{
			get { return this.Plugin.Metadata.Title; }
		}

		public string Description
		{
			get { return this.Plugin.Metadata.Description; }
		}

		public string Author
		{
			get { return this.Plugin.Metadata.Author; }
		}

		public string Version
		{
			get { return this.Plugin.Metadata.Version; }
		}

		#endregion

		#region ErrorMessage 変更通知プロパティ

		private string _ErrorMessage;

		public string ErrorMessage
		{
			get { return this._ErrorMessage; }
			set
			{
				if (this._ErrorMessage != value)
				{
					this._ErrorMessage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// プラグインが設定画面を持っているかどうかを示す値を取得します。
		/// </summary>
		public bool HasSettingsView { get; private set; }

		/// <summary>
		/// プラグイン側からの通知があるかどうかを示す値を取得します。
		/// </summary>
		public bool HasNotifySource { get; private set; }

		/// <summary>
		/// プラグインが通知機能を持っているかどうかを示す値を取得します。
		/// </summary>
		public bool HasNotifier
		{
			get { return this.notifier != null; }
		}

		/// <summary>
		/// プラグインの設定画面を取得します。
		/// </summary>
		public object SettingsView
		{
			get { return this.Plugin.OfType<ISettings>().Select(x => x.View).FirstOrDefault(); }
		}


		public PluginViewModel(Plugin plugin)
		{
			this.Plugin = plugin;

			var notifiers = plugin.OfType<INotifier>().ToArray();
			if (notifiers.Length >= 1) this.notifier = new AggregateNotifier(notifiers);

			this.HasSettingsView = plugin.OfType<ISettings>().Any();
			this.HasNotifySource = plugin.OfType<IRequestNotify>().Any();
		}

		public void OpenSettings()
		{

		}

		public void TestNotifier()
		{
			if (this.HasNotifier)
			{
				this.notifier.Show(NotifyType.Other, "テスト", "これはテスト通知です。", App.ViewModelRoot.Activate, ex => this.ErrorMessage = ex.Message);
			}
		}
	}
}
