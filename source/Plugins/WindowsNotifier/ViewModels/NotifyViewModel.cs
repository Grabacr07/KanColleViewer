using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Plugins.Properties;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Plugins.ViewModels
{
	public class NotifyViewModel : ViewModel
	{
		private static readonly Settings settings = Settings.Default;
		public float CustomVolume
		{
			get { return settings.CustomVolume; }
			set
			{
				if (settings.CustomVolume == value) return;
				settings.CustomVolume = value;
				settings.Save();
				this.RaisePropertyChanged();
			}
		}

		public bool UseModern
		{
			get { return settings.UseModern; }
			set
			{
				if (settings.UseModern == value) return;
				settings.UseModern = value;
				settings.Save();
				this.RaisePropertyChanged();
			}
		}

		public void Critical()
		{
			var notification = Notification.Create(
								"",
								"대파알림",
								"대파알림 테스트",
								() => WindowService.Current.MainWindow.Activate());

			NotifyService.Current.Notify(notification);
		}
		public void Dockyard()
		{

			var notification = Notification.Create(
								"",
								"건조완료",
								"테스트",
								() => WindowService.Current.MainWindow.Activate());

			NotifyService.Current.Notify(notification);
		}
		public void expedition()
		{

			var notification = Notification.Create(
								"",
								"원정완료",
								"테스트",
								() => WindowService.Current.MainWindow.Activate());

			NotifyService.Current.Notify(notification);
		}
		public void Rejuvenated()
		{

			var notification = Notification.Create(
								"",
								"피로회복완료",
								"테스트",
								() => WindowService.Current.MainWindow.Activate());

			NotifyService.Current.Notify(notification);
		}
		public void repair()
		{

			var notification = Notification.Create(
								"",
								"정비완료",
								"테스트",
								() => WindowService.Current.MainWindow.Activate());

			NotifyService.Current.Notify(notification);
		}
	}
}
