using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class MaterialsViewModel : ViewModel
	{
		#region Model 変更通知プロパティ

		public Materials Model
		{
			get { return KanColleClient.Current.Homeport.Materials; }
		}

		#endregion

		public MaterialsViewModel()
		{
			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport)
			{
				{"Materials", (sender, args) => this.RaisePropertyChanged("Model") },
			});
		}
	}
}
