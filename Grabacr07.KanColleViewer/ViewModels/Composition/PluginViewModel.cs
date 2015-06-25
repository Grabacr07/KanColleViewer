using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Views.Settings;
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

		public IEnumerable<RichText> Functions
		{
			get
			{
				var settings = this.Plugin.OfType<ISettings>().FirstOrDefault();
				if (settings != null) yield return new SettingsText { Function = settings, };

				if (this.notifier != null) yield return new NotifierText { Function = this.notifier, TestMethod = this.TestNotifier, };

				var requestNotify = this.Plugin.OfType<IRequestNotify>().FirstOrDefault();
				if (requestNotify != null) yield return new RequestNotifyText { Function = requestNotify, };

				var tool = this.Plugin.OfType<ITool>().FirstOrDefault();
				if (tool != null) yield return new ToolText { Function = tool, };
			}
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
		}

		public void OpenSettings()
		{

		}

		public void TestNotifier()
		{
			if (this.notifier != null)
			{
				this.notifier.Show(NotifyType.Other, "テスト", "これはテスト通知です。", App.ViewModelRoot.Activate, ex => this.ErrorMessage = ex.Message);
			}
		}
	}
}
