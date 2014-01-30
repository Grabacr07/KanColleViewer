using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleViewer.Models
{
	/// <summary>
	/// 主にメイン ウィンドウのステータス バーに表示する
	/// </summary>
	public class StatusService : NotificationObject
	{
		#region static members

		private static readonly StatusService current = new StatusService();
		public static StatusService Current
		{
			get { return current; }
		}

		#endregion

		private readonly Subject<string> notifier;
		private string persisitentMessage = "";
		private string notificationMessage;

		#region Message 変更通知プロパティ

		/// <summary>
		/// 現在のステータスを示す文字列を取得します。
		/// </summary>
		public string Message
		{
			get { return this.notificationMessage ?? this.persisitentMessage; }
			set
			{
				if (this.persisitentMessage != value)
				{
					this.persisitentMessage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private StatusService()
		{
			this.notifier = new Subject<string>();
			this.notifier
				.Do(x =>
				{
					this.notificationMessage = x;
					this.RaiseMessagePropertyChanged();
				})
				.Throttle(TimeSpan.FromMilliseconds(5000))
				.Subscribe(_ =>
				{
					this.notificationMessage = null;
					this.RaiseMessagePropertyChanged();
				});
		}

		public void Set(string message)
		{
			this.Message = message;
		}

		public void Notify(string message)
		{
			this.notifier.OnNext(message);
		}

		private void RaiseMessagePropertyChanged()
		{
			this.RaisePropertyChanged("Message");
		}
	}
}
