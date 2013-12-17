using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class Quests : NotificationObject
	{
		private readonly ConcurrentDictionary<int, Quest> quests;

		public IReadOnlyCollection<Quest> All
		{
			get { return this.quests.Select(x => x.Value).OrderBy(x => x.Id).ToList(); }
		}

		#region Current 変更通知プロパティ

		private IReadOnlyCollection<Quest> _Current;

		/// <summary>
		/// 現在遂行中の任務のリストを取得します。未取得の任務がある場合、リスト内に null が含まれることに注意してください。
		/// </summary>
		public IReadOnlyCollection<Quest> Current
		{
			get { return this._Current; }
			set
			{
				if (!Equals(this._Current, value))
				{
					this._Current = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		internal Quests(KanColleProxy proxy)
		{
			this.quests = new ConcurrentDictionary<int, Quest>();

			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "")
				.TryParse<kcsapi_questlist>()
				.Subscribe(this.Update);
		}

		private void Update(kcsapi_questlist questlist)
		{
			questlist.api_list.Select(x => new Quest(x))
				.ForEach(x => this.quests.AddOrUpdate(x.Id, x, (_, y) => y));

			var current = this.quests.Where(x => x.Value.State == QuestState.TakeOn || x.Value.State == QuestState.Accomplished)
				.Select(x => x.Value)
				.OrderBy(x => x.Id)
				.ToList();

			// 遂行中の任務数に満たない場合、未取得分として null で埋める
			while (current.Count < questlist.api_exec_count - 1) current.Add(null);
			this.Current = current;

			this.RaisePropertyChanged("All");
		}
	}
}
