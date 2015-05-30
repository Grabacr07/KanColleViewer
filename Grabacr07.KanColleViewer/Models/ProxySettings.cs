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
		#region IsEnabled 変更通知プロパティ

		private bool _IsEnabled;

		public bool IsEnabled
		{
			get { return this._IsEnabled; }
			set
			{
				if (this._IsEnabled != value)
				{
					this._IsEnabled = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsEnabledOnSSL 変更通知プロパティ

		private bool _IsEnabledOnSSL;

		public bool IsEnabledOnSSL
		{
			get { return this._IsEnabledOnSSL; }
			set
			{
				if (this._IsEnabledOnSSL != value)
				{
					this._IsEnabledOnSSL = value;
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

		#region IsAuthRequired 変更通知プロパティ

		private bool _IsAuthRequired;

		public bool IsAuthRequired
		{
			get { return this._IsAuthRequired; }
			set
			{
				if (this._IsAuthRequired != value)
				{
					this._IsAuthRequired = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Username 変更通知プロパティ

		private string _Username;

		public string Username
		{
			get { return this._Username; }
			set
			{
				if (this._Username != value)
				{
					this._Username = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Password 変更通知プロパティ

		private const string passwordForEncryption = "4629A16F-C815-4307-B367-9C16FAC0A52F";

		[XmlIgnore]
		public string Password
		{
			get { return Helper.DecryptString(this.ProtectedPassword, passwordForEncryption); }
			set { this.ProtectedPassword = Helper.EncryptString(value, passwordForEncryption); }
		}

		#endregion

		#region ProtectedPassword 変更通知プロパティ

		private string _ProtectedPassword;

		public string ProtectedPassword
		{
			get { return this._ProtectedPassword; }
			set
			{
				if (this._ProtectedPassword != value)
				{
					this._ProtectedPassword = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
	}
}
