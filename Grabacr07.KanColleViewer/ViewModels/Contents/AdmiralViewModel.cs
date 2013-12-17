using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class AdmiralViewModel : ViewModel
	{
		#region Model 変更通知プロパティ

		public Admiral Model
		{
			get { return KanColleClient.Current.Homeport.Admiral; }
		}

		#endregion

		public AdmiralViewModel()
		{
			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport)
			{
				{ "Admiral", (sender, args) => this.Update() },
			});
		}

		private void Update()
		{
			this.RaisePropertyChanged("Model");
		}
	}
}
