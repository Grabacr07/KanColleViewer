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
using Nekoxy;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.Web;

namespace Grabacr07.KanColleWrapper
{
	public class Quests : Notifier
	{
		#region All 変更通知プロパティ

		private IReadOnlyCollection<Quest> _All;

		public IReadOnlyCollection<Quest> All
		{
			get { return this._All; }
			set
			{
				if (!Equals(this._All, value))
				{
					this._All = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

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


		internal Quests(KanColleProxy proxy)
		{
			this.IsUntaken = true;
			this.All = this.Current = new List<Quest>();

			proxy.api_get_member_questlist
				.Where(x => HttpUtility.ParseQueryString(x.Request.BodyAsString)["api_tab_id"] == "0")
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
					api_completed_kind = Convert.ToInt32(djson.api_data.api_completed_kind),
					api_exec_count = Convert.ToInt32(djson.api_data.api_exec_count),
					api_exec_type = Convert.ToInt32(djson.api_data.api_exec_type),
				};

				if (djson.api_data.api_list != null)
				{
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
				}

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

			if (questlist.api_list == null)
			{
				this.IsEmpty = true;
				this.All = this.Current = new List<Quest>();
			}
			else
			{
				this.IsEmpty = false;

				this.All = questlist.api_list.Select(x => new Quest(x)).ToList();

				var current = this.All.Where(x => x.State == QuestState.TakeOn || x.State == QuestState.Accomplished)
					.OrderBy(x => x.Id)
					.ToList();

				// 遂行中の任務数に満たない場合、未取得分として null で埋める
				while (current.Count < questlist.api_exec_count) current.Add(null);
				this.Current = current;
			}
		}
	}
}
