using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class NetworkSettingsViewModel : ViewModel
	{
		#region ProxyType 変更通知プロパティ

		private ProxyType _ProxyType;

		public ProxyType ProxyType
		{
			get { return this._ProxyType; }
			set
			{
				if (this._ProxyType != value)
				{
					this._ProxyType = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region SpecificProxyHost 変更通知プロパティ

		private string _SpecificProxyHost;

		public string SpecificProxyHost
		{
			get { return this._SpecificProxyHost; }
			set
			{
				if (this._SpecificProxyHost != value)
				{
					this._SpecificProxyHost = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region SpecificProxyPort 変更通知プロパティ

		private ushort _SpecificProxyPort;

		public ushort SpecificProxyPort
		{
			get { return this._SpecificProxyPort; }
			set
			{
				if (this._SpecificProxyPort != value)
				{
					this._SpecificProxyPort = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion
		
		public void Initialize()
		{
			this.Cancel();
			NetworkSettings.Proxy.Type.Subscribe(x => this.ProxyType = x).AddTo(this);
			NetworkSettings.Proxy.Host.Subscribe(x => this.SpecificProxyHost = x).AddTo(this);
			NetworkSettings.Proxy.Port.Subscribe(x => this.SpecificProxyPort = x).AddTo(this);
		}

		public void Apply()
		{
			NetworkSettings.Proxy.Type.Value = this.ProxyType;
			NetworkSettings.Proxy.Host.Value = this.SpecificProxyHost;
			NetworkSettings.Proxy.Port.Value = this.SpecificProxyPort;

			KanColleClient.Current.Proxy.UpstreamProxySettings = new NetworkSettings.Proxy();
		}

		public void Cancel()
		{
			this.ProxyType = NetworkSettings.Proxy.Type;
			this.SpecificProxyHost = NetworkSettings.Proxy.Host;
			this.SpecificProxyPort = NetworkSettings.Proxy.Port;
		}
	}
}
