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

		#region SpecificHttpProxyHost 変更通知プロパティ

		private string _SpecificHttpProxyHost;

		public string SpecificHttpProxyHost
		{
			get { return this._SpecificHttpProxyHost; }
			set
			{
				if (this._SpecificHttpProxyHost != value)
				{
					this._SpecificHttpProxyHost = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region SpecificHttpProxyPort 変更通知プロパティ

		private string _SpecificHttpProxyPort;

		public string SpecificHttpProxyPort
		{
			get { return this._SpecificHttpProxyPort; }
			set
			{
				if (this._SpecificHttpProxyPort != value)
				{
					this._SpecificHttpProxyPort = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region IsUseHttpProxyForAllProtocols 変更通知プロパティ

		private bool _IsUseHttpProxyForAllProtocols;

		public bool IsUseHttpProxyForAllProtocols
		{
			get { return this._IsUseHttpProxyForAllProtocols; }
			set
			{
				if (this._IsUseHttpProxyForAllProtocols != value)
				{
					this._IsUseHttpProxyForAllProtocols = value;
					this.RaisePropertyChanged();
				}
				this.IsUseHttpProxyForAllProtocolsChanged();
			}
		}

		#endregion

		#region SpecificHttpsProxyHost 変更通知プロパティ

		private string _SpecificHttpsProxyHost;

		public string SpecificHttpsProxyHost
		{
			get { return this._SpecificHttpsProxyHost; }
			set
			{
				if (this._SpecificHttpsProxyHost != value)
				{
					this._SpecificHttpsProxyHost = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region SpecificHttpsProxyPort 変更通知プロパティ

		private string _SpecificHttpsProxyPort;

		public string SpecificHttpsProxyPort
		{
			get { return this._SpecificHttpsProxyPort; }
			set
			{
				if (this._SpecificHttpsProxyPort != value)
				{
					this._SpecificHttpsProxyPort = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region SpecificFtpProxyHost 変更通知プロパティ

		private string _SpecificFtpProxyHost;

		public string SpecificFtpProxyHost
		{
			get { return this._SpecificFtpProxyHost; }
			set
			{
				if (this._SpecificFtpProxyHost != value)
				{
					this._SpecificFtpProxyHost = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region SpecificFtpProxyPort 変更通知プロパティ

		private string _SpecificFtpProxyPort;

		public string SpecificFtpProxyPort
		{
			get { return this._SpecificFtpProxyPort; }
			set
			{
				if (this._SpecificFtpProxyPort != value)
				{
					this._SpecificFtpProxyPort = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region SpecificSocksProxyHost 変更通知プロパティ

		private string _SpecificSocksProxyHost;

		public string SpecificSocksProxyHost
		{
			get { return this._SpecificSocksProxyHost; }
			set
			{
				if (this._SpecificSocksProxyHost != value)
				{
					this._SpecificSocksProxyHost = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		#region SpecificSocksProxyPort 変更通知プロパティ

		private string _SpecificSocksProxyPort;

		public string SpecificSocksProxyPort
		{
			get { return this._SpecificSocksProxyPort; }
			set
			{
				if (this._SpecificSocksProxyPort != value)
				{
					this._SpecificSocksProxyPort = value;
					this.RaisePropertyChanged();

				}
			}
		}

		#endregion

		public void Initialize()
		{
			this.RevertToSavedSettings();
			NetworkSettings.Proxy.Type.Subscribe(x => this.ProxyType = x).AddTo(this);
			NetworkSettings.Proxy.Host.Subscribe(x => this.SpecificHttpProxyHost = x).AddTo(this);
			NetworkSettings.Proxy.Port.Subscribe(x => this.SpecificHttpProxyPort = x).AddTo(this);
			NetworkSettings.Proxy.HttpsHost.Subscribe(x => this.SpecificHttpsProxyHost = x).AddTo(this);
			NetworkSettings.Proxy.HttpsPort.Subscribe(x => this.SpecificHttpsProxyPort = x).AddTo(this);
			NetworkSettings.Proxy.FtpHost.Subscribe(x => this.SpecificFtpProxyHost = x).AddTo(this);
			NetworkSettings.Proxy.FtpPort.Subscribe(x => this.SpecificFtpProxyPort = x).AddTo(this);
			NetworkSettings.Proxy.SocksHost.Subscribe(x => this.SpecificSocksProxyHost = x).AddTo(this);
			NetworkSettings.Proxy.SocksPort.Subscribe(x => this.SpecificSocksProxyPort = x).AddTo(this);
			NetworkSettings.Proxy.IsUseHttpProxyForAllProtocols.Subscribe(x => this.IsUseHttpProxyForAllProtocols = x).AddTo(this);
		}

		public void Apply()
		{
			ushort port;
			NetworkSettings.Proxy.Type.Value = this.ProxyType;
			NetworkSettings.Proxy.Host.Value = this.SpecificHttpProxyHost;
			NetworkSettings.Proxy.Port.Value = ushort.TryParse(this.SpecificHttpProxyPort, out port) ? port : NetworkSettings.Proxy.Port.Default;
			NetworkSettings.Proxy.HttpsHost.Value = this.SpecificHttpsProxyHost;
			NetworkSettings.Proxy.HttpsPort.Value = ushort.TryParse(this.SpecificHttpsProxyPort, out port) ? port : NetworkSettings.Proxy.HttpsPort.Default;
			NetworkSettings.Proxy.FtpHost.Value = this.SpecificFtpProxyHost;
			NetworkSettings.Proxy.FtpPort.Value = ushort.TryParse(this.SpecificFtpProxyPort, out port) ? port : NetworkSettings.Proxy.FtpPort.Default;
			NetworkSettings.Proxy.SocksHost.Value = this.SpecificSocksProxyHost;
			NetworkSettings.Proxy.SocksPort.Value = ushort.TryParse(this.SpecificSocksProxyPort, out port) ? port : NetworkSettings.Proxy.SocksPort.Default;
			NetworkSettings.Proxy.IsUseHttpProxyForAllProtocols.Value = this.IsUseHttpProxyForAllProtocols;
			this.RevertToSavedSettings();    // Parse 結果書き戻し

			KanColleClient.Current.Proxy.UpstreamProxySettings = new NetworkSettings.Proxy();
		}

		public void Cancel()
		{
			this.RevertToSavedSettings();
		}

		private void RevertToSavedSettings()
		{
			this.ProxyType = NetworkSettings.Proxy.Type;
			this.SpecificHttpProxyHost = NetworkSettings.Proxy.Host;
			this.SpecificHttpProxyPort = NetworkSettings.Proxy.Port.Value.ToString();
			this.SpecificHttpsProxyHost = NetworkSettings.Proxy.HttpsHost;
			this.SpecificHttpsProxyPort = NetworkSettings.Proxy.HttpsPort.Value.ToString();
			this.SpecificFtpProxyHost = NetworkSettings.Proxy.FtpHost;
			this.SpecificFtpProxyPort = NetworkSettings.Proxy.FtpPort.Value.ToString();
			this.SpecificSocksProxyHost = NetworkSettings.Proxy.SocksHost;
			this.SpecificSocksProxyPort = NetworkSettings.Proxy.SocksPort.Value.ToString();
			this.IsUseHttpProxyForAllProtocols = NetworkSettings.Proxy.IsUseHttpProxyForAllProtocols;
		}

		private void IsUseHttpProxyForAllProtocolsChanged()
		{
			if (this.IsUseHttpProxyForAllProtocols)
			{
				this.SpecificHttpsProxyHost = this.SpecificHttpProxyHost;
				this.SpecificHttpsProxyPort = this.SpecificHttpProxyPort;
				this.SpecificFtpProxyHost = this.SpecificHttpProxyHost;
				this.SpecificFtpProxyPort = this.SpecificHttpProxyPort;
				this.SpecificSocksProxyHost = NetworkSettings.Proxy.SocksHost.Default;
				this.SpecificSocksProxyPort = NetworkSettings.Proxy.SocksPort.Default.ToString();
			}
			else
			{
				this.SpecificHttpsProxyHost = NetworkSettings.Proxy.HttpsHost;
				this.SpecificHttpsProxyPort = NetworkSettings.Proxy.HttpsPort.Value.ToString();
				this.SpecificFtpProxyHost = NetworkSettings.Proxy.FtpHost;
				this.SpecificFtpProxyPort = NetworkSettings.Proxy.FtpPort.Value.ToString();
				this.SpecificSocksProxyHost = NetworkSettings.Proxy.SocksHost;
				this.SpecificSocksProxyPort = NetworkSettings.Proxy.SocksPort.Value.ToString();
			}
		}
	}
}
