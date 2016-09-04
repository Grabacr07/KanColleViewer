using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;

using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Models.QuestTracker;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class QuestsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.Quests + (this._Badge > 0 ? " (" + this._Badge + ")" : ""); }
			protected set { throw new NotImplementedException(); }
		}

        public QuestViewModel[] Current => this._Quests.Where(x => x.State != QuestState.None).ToArray();

		#region Quests 変更通知プロパティ

		private QuestViewModel[] _Quests;

		public QuestViewModel[] Quests
		{
            get
            {
                IEnumerable<QuestViewModel> temp;
                switch (SelectedItem.Key)
                {
                    case "All": return ComputeQuestPage(this._Quests);
                    case "Current":
                        temp = this._Quests.Where(x => x.State != QuestState.None);
                        break;
                    case "Daily":
                        temp = this._Quests.Where(x => x.Type == QuestType.Daily);
                        break;
                    case "Weekly":
                        temp = this._Quests.Where(x => x.Type == QuestType.Weekly);
                        break;
                    case "Monthly":
                        temp = this._Quests.Where(x => x.Type == QuestType.Monthly);
                        break;
                    case "Once":
                        temp = this._Quests.Where(x => x.Type == QuestType.OneTime);
                        break;
                    case "Others":
                        temp = this._Quests.Where(x => x.Type == QuestType.Other);
                        break;
                    default:
                        temp = null;
                        break;
                }
                if (temp == null) return new QuestViewModel[0];
                return ComputeQuestPage(temp.ToArray());
            }
            set
            {
				if (this._Quests != value)
				{
					this._Quests = value;
					this.RaisePropertyChanged();
                    this.RaisePropertyChanged("Current");
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


        private int _Badge = 0;

        public IList<KeyNameTabItemViewModel> TabItems { get; set; }

        private KeyNameTabItemViewModel _SelectedItem;
        public KeyNameTabItemViewModel SelectedItem
        {
            get { return this._SelectedItem; }
            set
            {
                if (this._SelectedItem != value)
                {
                    this._SelectedItem = value;
                    this.RaisePropertyChanged("Quests");
                    this.RaisePropertyChanged("SelectedItem");
                }
            }
        }


        private TrackManager questTracker { get; set; }

        public QuestsViewModel()
		{
            this.TabItems = new List<KeyNameTabItemViewModel>
                {
                    new KeyNameTabItemViewModel("All", "전체"),
                    new KeyNameTabItemViewModel("Current", "진행중"),
                    new KeyNameTabItemViewModel("Daily", "일일"),
                    new KeyNameTabItemViewModel("Weekly", "주간"),
                    new KeyNameTabItemViewModel("Monthly", "월간"),
                    new KeyNameTabItemViewModel("Once", "일회성"),
                    new KeyNameTabItemViewModel("Others", "그 외")
                };
            this.SelectedItem = this.TabItems.FirstOrDefault(x => x.Name == "진행중");


            var quests = KanColleClient.Current.Homeport.Quests;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(quests)
			{
				{ nameof(quests.All), (sender, args) => this.UpdateQuest(quests) },
			});
            
            // set Quest Tarcker
            questTracker = new TrackManager();
            questTracker.QuestsEventChanged += (s, e) => this.UpdateQuest(quests);

            this.UpdateQuest(quests);
		}

        private void UpdateQuest(Quests quests)
        {
            var viewList = quests.All
                .Select(x => new QuestViewModel(x)).Distinct(x => x.Id)
                .ToList();

            questTracker.AllQuests.ForEach(x =>
            {
                foreach (var y in viewList)
                {
                    if (y.Id == x.Id)
                    {
                        y.QuestProgressValue = x.GetProgress();
                        y.QuestProgressText = x.GetProgressText();
                    }
                }
            });

            this.Quests = viewList.ToArray();
            this.IsEmpty = quests.IsEmpty;
            this.IsUntaken = quests.IsUntaken;

            this._Badge = questTracker.AllQuests.Where(x => quests.All.Any(y => y.Id == x.Id)).Count(x => x.GetProgress() == 100);
            this.RaisePropertyChanged("Name");
        }

        private QuestViewModel[] ComputeQuestPage(QuestViewModel[] inp)
        {
            if (inp.Length == 0) return inp;

            inp = inp
                .Select(x => { x.LastOnPage = false; return x; })
                .OrderBy(x => x.Page)
                .ThenBy(x => x.Id)
                .ToArray();

            int[] pages = inp.Select(x => x.Page)
                .Distinct()
                .ToArray();

            foreach (var page in pages)
                inp.Where(x => x.Page == page)
                    .Last().LastOnPage = true;

            return inp.ToArray();
        }

    }
}
