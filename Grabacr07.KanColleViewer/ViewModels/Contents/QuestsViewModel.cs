using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class QuestsViewModel : TabItemViewModel
	{
		#region Current 変更通知プロパティ

		private QuestViewModel[] _Current;

		public QuestViewModel[] Current
		{
			get { return this._Current; }
			set
			{
				if (this._Current != value)
				{
					this._Current = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Quests 変更通知プロパティ

		private QuestViewModel[] _Quests;

		public QuestViewModel[] Quests
		{
			get { return this._Quests; }
			set
			{
				if (this._Quests != value)
				{
					this._Quests = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsUntaken 変更通知プロパティ

		private bool _IsUntaken;

		public bool IsUntaken
		{
			get { return this._IsUntaken; }
			set
			{
				if (this._IsUntaken != value)
				{
					this._IsUntaken = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public QuestsViewModel()
		{
			this.Name = Properties.Resources.ViewModels_Quests;

			this.IsUntaken = KanColleClient.Current.Homeport.Quests.IsUntaken;
			this.Quests = KanColleClient.Current.Homeport.Quests.All.Select(x => new QuestViewModel(x)).ToArray();
			this.Current = KanColleClient.Current.Homeport.Quests.Current.Select(x => new QuestViewModel(x)).ToArray();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Quests)
			{
				{
					() => KanColleClient.Current.Homeport.Quests.IsUntaken,
					(sender, args) => this.IsUntaken = KanColleClient.Current.Homeport.Quests.IsUntaken
				},
				{
					() => KanColleClient.Current.Homeport.Quests.All,
					(sender, args) => this.Quests = KanColleClient.Current.Homeport.Quests.All.Select(x => new QuestViewModel(x)).ToArray()
				},
				{
					() => KanColleClient.Current.Homeport.Quests.Current,
					(sender, args) => this.Current = KanColleClient.Current.Homeport.Quests.Current.Select(x => new QuestViewModel(x)).ToArray()
				},
			});
		}
	}
}
