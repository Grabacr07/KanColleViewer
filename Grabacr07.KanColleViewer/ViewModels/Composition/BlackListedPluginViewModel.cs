using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class BlackListedPluginViewModel : ViewModel
	{
		#region Name 変更通知プロパティ

		private string _Name;

		public string Name
		{
			get { return this._Name; }
			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

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

		#region Exception 変更通知プロパティ

		private string _Exception;

		public string Exception
		{
			get { return this._Exception; }
			set
			{
				if (this._Exception != value)
				{
					this._Exception = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public BlackListedPluginViewModel(BlacklistedPluginData data)
		{
			this.Name = Path.GetFileName(data.FilePath);

			using (var reader = new StringReader(data.Exception))
			{
				this.Message = reader.ReadLine();
				this.Exception = reader.ReadToEnd();
			}
		}
	}
}
