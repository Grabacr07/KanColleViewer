using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;
using Fiddler;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class Quests : NotificationObject
	{
		private readonly List<ConcurrentDictionary<int, Quest>> questPages;

		public IReadOnlyCollection<Quest> All
		{
			get
			{
				return this.questPages.Where(x => x != null)
					.SelectMany(x => x.Select(kvp => kvp.Value))
					.Distinct(x => x.Id)
					.OrderBy(x => x.Id)
					.ToList();
			}
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


		internal Quests(KanColleProxy proxy)
		{
			this.questPages = new List<ConcurrentDictionary<int, Quest>>();
			this.IsUntaken = true;
			this.Current = new List<Quest>();

			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/questlist")
				.Select(Serialize)
				.Where(x => x != null)
				.Subscribe(this.Update);
		}

		private static kcsapi_questlist Serialize(Session session)
		{
			try
			{
				var djson = DynamicJson.Parse(session.GetResponseAsJson());
				var questlist = new kcsapi_questlist
				{
					api_count = Convert.ToInt32(djson.api_data.api_count),
					api_disp_page = Convert.ToInt32(djson.api_data.api_disp_page),
					api_page_count = Convert.ToInt32(djson.api_data.api_page_count),
					api_exec_count = Convert.ToInt32(djson.api_data.api_exec_count),
				};

				var list = new List<kcsapi_quest>();
				var serializer = new DataContractJsonSerializer(typeof(kcsapi_quest));
				foreach (var x in (object[])djson.api_data.api_list)
				{
					try
					{
						list.Add(serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(x.ToString()))) as kcsapi_quest);
					}
					catch (SerializationException ex)
					{
						// 最後のページで任務数が 5 に満たないとき、api_list が -1 で埋められるというクソ API のせい
						Debug.WriteLine(ex.Message);
					}
				}

				questlist.api_list = list.ToArray();

				return questlist;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				return null;
			}
		}

		private void Update(kcsapi_questlist questlist)
		{
			this.IsUntaken = false;

			// キャッシュしてるページの数が、取得したページ数 (api_page_count) より大きいとき
			// 取得したページ数と同じ数になるようにキャッシュしてるページを減らす
			if (this.questPages.Count > questlist.api_page_count)
			{
				while (this.questPages.Count > questlist.api_page_count) this.questPages.RemoveAt(this.questPages.Count - 1);
			}

			// 小さいときは、キャッシュしたページ数と同じ数になるようにページを増やす
			else if (this.questPages.Count < questlist.api_page_count)
			{
				while (this.questPages.Count < questlist.api_page_count) this.questPages.Add(null);
			}

			this.questPages[questlist.api_disp_page - 1]  = new ConcurrentDictionary<int, Quest>();
			questlist.api_list.Select(x => new Quest(x))
				.ForEach(x => this.questPages[questlist.api_disp_page - 1].AddOrUpdate(x.Id, x, (_, __) => x));

			var current = this.All.Where(x => x.State == QuestState.TakeOn || x.State == QuestState.Accomplished)
				.OrderBy(x => x.Id)
				.ToList();

			// 遂行中の任務数に満たない場合、未取得分として null で埋める
			while (current.Count < questlist.api_exec_count) current.Add(null);
			this.Current = current;

			this.RaisePropertyChanged("All");
		}
	}
}
