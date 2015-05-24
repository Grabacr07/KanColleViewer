using System;
using System.Reactive.Linq;
using System.Windows;
using BattleInfoPlugin.Properties;
using Grabacr07.KanColleViewer;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using Livet;

namespace BattleInfoPlugin.Models.Notifiers
{
	public class BattleEndNotifier : NotificationObject
	{
		private static readonly Settings settings = Settings.Default;

		#region IsEnabled変更通知プロパティ
		public bool IsEnabled
		{
			get
			{ return settings.IsEnabledBattleEndNotify; }
			set
			{
				if (settings.IsEnabledBattleEndNotify == value)
					return;
				settings.IsEnabledBattleEndNotify = value;
				settings.Save();
				this.RaisePropertyChanged();
			}
		}
		#endregion


		public BattleEndNotifier()
		{
			settings.Reload();

			var proxy = KanColleClient.Current.Proxy;
			proxy.api_req_combined_battle_battleresult
				.Subscribe(_ => this.Notify());
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_practice/battle_result")
				.Subscribe(_ => this.Notify(true));
			proxy.api_req_sortie_battleresult
				.Subscribe(_ => this.Notify());
			proxy.api_port.Subscribe(_ => this.Clear());
		}
		private void Clear()
		{
			Settings.Default.FirstIsCritical = false;
			Settings.Default.SecondIsCritical = false;
			Grabacr07.KanColleViewer.Models.NotifierHost.ClearCriticalStatus();
		}
		private void Notify(bool IsPractice = false)
		{
			var isActive = DispatcherHelper.UIDispatcher.Invoke(() => Application.Current.MainWindow.IsActive);
			if (this.IsEnabled && !isActive)
				PluginHost.Instance.GetNotifier().Show(
					NotifyType.Other,
					"전투종료",
					"전투가 종료되었습니다!",
					() => App.ViewModelRoot.Activate());

			if (Settings.Default.FirstIsCritical || Settings.Default.SecondIsCritical)
				if(!IsPractice)
					Grabacr07.KanColleViewer.Models.NotifierHost.CriticalNotify();
		}
	}
}
