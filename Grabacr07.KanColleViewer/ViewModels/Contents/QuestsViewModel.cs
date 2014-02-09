using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class QuestsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.Quests; }
			protected set { throw new NotImplementedException(); }
		}
	
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

		#region IsEmpty 変更通知プロパティ

		private bool _IsEmpty;

		public bool IsEmpty
		{
			get { return this._IsEmpty; }
			set
			{
				if (this._IsEmpty != value)
				{
					this._IsEmpty = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public QuestsViewModel()
		{
			var quests = KanColleClient.Current.Homeport.Quests;

			this.IsUntaken = quests.IsUntaken;
			this.Quests = quests.All.Select(x => new QuestViewModel(x)).ToArray();
			this.Current = quests.Current.Select(x => new QuestViewModel(x)).ToArray();
			this.IsEmpty = quests.IsEmpty;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(quests)
			{
				{ () => quests.IsUntaken, (sender, args) => this.IsUntaken = quests.IsUntaken },
				{ () => quests.All, (sender, args) => this.Quests = quests.All.Select(x => new QuestViewModel(x)).ToArray() },
				{ () => quests.Current, (sender, args) => this.Current = quests.Current.Select(x => new QuestViewModel(x)).ToArray() },
				{ () => quests.IsEmpty, (sender, args) => this.IsEmpty = quests.IsEmpty }
			});
		}
	}
}
