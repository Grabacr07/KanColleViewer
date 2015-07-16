using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class ProxyBootstrapperViewModel : WindowViewModel
	{
		public ProxyBootstrapper Bootstrapper { get; }

		#region Message 変更通知プロパティ

		private string _Message;

		public string Message
		{
			get { return this._Message; }
			set
			{
				if (this._Message != value)
				{
					this._Message = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsEditable 変更通知プロパティ

		private bool _IsEditable = true;

		public bool IsEditable
		{
			get { return this._IsEditable; }
			set
			{
				if (this._IsEditable != value)
				{
					this._IsEditable = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Status 変更通知プロパティ

		private string _Status;

		public string Status
		{
			get { return this._Status; }
			set
			{
				if (this._Status != value)
				{
					this._Status = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.HasStatus));
				}
			}
		}

		public bool HasStatus => !string.IsNullOrEmpty(this.Status);

		#endregion

		public ProxyBootstrapperViewModel(ProxyBootstrapper bootstrapper)
		{
			this.DialogResult = false;
			this.Bootstrapper = bootstrapper;
			this.UpdateMessage();
		}

		public async void Retry()
		{
			this.IsEditable = false;
			this.Status = "再試行中...";

			// ToDo: async void なので刺されそう
			await Task.WhenAll(Task.Run(() => this.Bootstrapper.Try()), Task.Delay(TimeSpan.FromMilliseconds(1500)));

			this.IsEditable = true;
			this.Status = "";

			if (this.Bootstrapper.Result == ProxyBootstrapResult.Success)
			{
				this.DialogResult = true;
				this.Close();
			}
			else
			{
				this.UpdateMessage();
			}
		}

		public void Cancel()
		{
			this.DialogResult = false;
			this.Close();
		}

		private void UpdateMessage()
		{
			#region messages (const)
			// いつか多言語リソースに移す… いつか…
			const string message10048 = @"既にポート {0} で通信を待ち受けているアプリケーションが存在するため、開始に失敗しました。
アプリケーションを終了するか、以下で待ち受けポートを変更できます。";
			const string messageUnexpectedException = @"だめだった :;(∩´﹏`∩);:
{0}";
			#endregion

			if (this.Bootstrapper.Result == ProxyBootstrapResult.WsaEAddrInUse)
			{
				this.Message = string.Format(message10048, this.Bootstrapper.ListeningPort);
			}
			else if (this.Bootstrapper.Result == ProxyBootstrapResult.UnexpectedException)
			{
				this.Message = string.Format(messageUnexpectedException, this.Bootstrapper.Exception.Message);
			}
		}
	}
}
