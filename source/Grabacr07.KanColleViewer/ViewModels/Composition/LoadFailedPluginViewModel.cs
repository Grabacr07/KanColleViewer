using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class LoadFailedPluginViewModel : ViewModel
	{
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

		#region Metadata 変更通知プロパティ

		private object _Metadata;

		public object Metadata
		{
			get { return this._Metadata; }
			set
			{
				if (this._Metadata != value)
				{
					this._Metadata = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public LoadFailedPluginViewModel(LoadFailedPluginData data)
		{
			this.Metadata = data.Metadata ?? new BlacklistedAssembly { Name = Path.GetFileName(data.FilePath) } as object;

			using (var reader = new StringReader(data.Message))
			{
				this.Message = reader.ReadLine();
				this.Exception = reader.ReadToEnd();
			}
		}
	}

	public class BlacklistedAssembly
	{
		public string Name { get; set; }
	}
}
