using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Grabacr07.KanColleWrapper;
using Livet;

namespace Grabacr07.KanColleViewer.Models
{
	[Serializable]
	public class ProxySettings : NotificationObject, IProxySettings
	{
		public ProxySettings()
		{
			this._SettingType = ProxySettingType.SystemProxy;
			this._Host = null;
			this._Port = 80;
		}

		#region IsEnabled 変更通知プロパティ

		private ProxySettingType _SettingType;

		public ProxySettingType SettingType
		{
			get { return this._SettingType; }
			set
			{
				if (this._SettingType != value)
				{
					this._SettingType = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Host 変更通知プロパティ

		private string _Host;

		[XmlElement(ElementName = "ProxyHost")]
		public string Host
		{
			get { return this._Host; }
			set
			{
				if (this._Host != value)
				{
					this._Host = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Port 変更通知プロパティ

		private ushort _Port;

		public ushort Port
		{
			get { return this._Port; }
			set
			{
				if (this._Port != value)
				{
					this._Port = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
	}
}
