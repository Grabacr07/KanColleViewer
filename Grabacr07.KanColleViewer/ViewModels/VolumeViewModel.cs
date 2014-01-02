using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class VolumeViewModel : ViewModel
	{
		private Volume volume;

		#region IsMute 変更通知プロパティ

		private bool _IsMute;

		public bool IsMute
		{
			get { return this._IsMute; }
			set
			{
				if (this._IsMute != value)
				{
					this._IsMute = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public VolumeViewModel()
		{
			this.CreateVolumeInstanceIfNull();
		}

		public void ToggleMute()
		{
			if (this.CreateVolumeInstanceIfNull())
			{
				this.volume.ToggleMute();
			}
		}

		private bool CreateVolumeInstanceIfNull()
		{
			if (this.volume == null)
			{
				try
				{
					this.volume = Volume.GetInstance();
					this.CompositeDisposable.Add(new PropertyChangedEventListener(this.volume)
					{
						{ "IsMute", (sender, args) => this.IsMute = this.volume.IsMute },
					});
					this.IsMute = this.volume.IsMute;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
					return false;
				}
			}

			return true;
		}
	}
}
