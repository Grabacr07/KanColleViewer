using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleViewer.QuestTracker.Models.EventArgs;

namespace Grabacr07.KanColleViewer.QuestTracker.Models
{
	public class SlotItemTracker
	{
		internal event EventHandler<BaseEventArgs> CreateItemEvent;
		internal event EventHandler<DestroyItemEventArgs> DestoryItemEvent;

		private readonly Homeport homeport;

		public int SlotItemsCount => this.SlotItems.Count;
		private MemberTable<SlotItem> SlotItems;

		internal SlotItemTracker(Homeport parent, KanColleProxy proxy)
		{
			this.homeport = parent;

			this.SlotItems = new MemberTable<SlotItem>();

			proxy.api_get_member_slot_item.TryParse<kcsapi_slotitem[]>().Subscribe(x => this.Update(x.Data));
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data));
			proxy.api_req_kousyou_destroyitem2.TryParse<kcsapi_destroyitem2>().Subscribe(this.DestroyItem);

			proxy.api_req_kousyou_remodel_slot.TryParse<kcsapi_remodel_slot>().Subscribe(x =>
			{
				this.RemoveFromRemodel(x.Data);
				this.RemodelSlotItem(x.Data);
			});
		}


		internal void Update(kcsapi_slotitem[] source)
		{
			this.SlotItems = new MemberTable<SlotItem>(source.Select(x => new SlotItem(x)));
			foreach (var ship in this.homeport.Organization.Ships.Values) ship.UpdateSlots();
		}

		internal void RemoveFromRemodel(kcsapi_remodel_slot source)
		{
			if (source.api_use_slot_id != null)
			{
				foreach (var id in source.api_use_slot_id)
					this.SlotItems.Remove(id);
			}
		}

		private void CreateItem(kcsapi_createitem source)
		{
			if (source.api_create_flag == 1 && source.api_slot_item != null)
			{
				CreateItemEvent?.Invoke(this, new BaseEventArgs(source.api_create_flag != 0));

				this.SlotItems.Add(new SlotItem(source.api_slot_item));
			}
		}

		private void DestroyItem(SvData<kcsapi_destroyitem2> data)
		{
			if (data == null || !data.IsSuccess) return;

			try
			{
				DestoryItemEvent?.Invoke(this, new DestroyItemEventArgs(data.Request, data.Data));

				foreach (var x in data.Request["api_slotitem_ids"].Split(',').Select(int.Parse))
					this.SlotItems.Remove(x);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("装備の破棄に失敗しました: {0}", ex);
			}
		}

		private void RemodelSlotItem(kcsapi_remodel_slot source)
		{
			if (source.api_after_slot == null) return;

			this.SlotItems[source.api_after_slot.api_id]
				?.Remodel(source.api_after_slot.api_level, source.api_after_slot.api_slotitem_id);
		}

		private class MemberTable<TValue> : IReadOnlyDictionary<int, TValue> where TValue : class, IIdentifiable
		{
			private readonly IDictionary<int, TValue> dictionary;

			/// <summary>
			/// テーブルから指定した ID の要素を取得します。ID が存在しない場合は null を返します。
			/// </summary>
			public TValue this[int key] => this.dictionary.ContainsKey(key) ? this.dictionary[key] : null;


			public MemberTable() : this(new List<TValue>()) { }

			public MemberTable(IEnumerable<TValue> source)
			{
				this.dictionary = source.ToDictionary(x => x.Id);
			}


			internal void Add(TValue value)
			{
				this.dictionary.Add(value.Id, value);
			}

			internal void Remove(TValue value)
			{
				this.dictionary.Remove(value.Id);
			}

			internal void Remove(int id)
			{
				this.dictionary.Remove(id);
			}


			#region IReadOnlyDictionary<TK, TV> members

			public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
			{
				return this.dictionary.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public int Count => this.dictionary.Count;

			public bool ContainsKey(int key)
			{
				return this.dictionary.ContainsKey(key);
			}

			public bool TryGetValue(int key, out TValue value)
			{
				return this.dictionary.TryGetValue(key, out value);
			}

			public IEnumerable<int> Keys => this.dictionary.Keys;

			public IEnumerable<TValue> Values => this.dictionary.Values;

			#endregion
		}

		private class SlotItem : RawDataWrapper<kcsapi_slotitem>, IIdentifiable
		{
			public int Id => this.RawData.api_id;

			public SlotItemInfo Info { get; private set; }

			public string NameWithLevel => $"{this.Info.Name}{(this.Level >= 1 ? (" " + this.LevelText) : "")}{(this.Proficiency >= 1 ? (" " + this.ProficiencyText) : "")}";

			public int Level => this.RawData.api_level;
			public string LevelText => this.Level >= 10 ? "★max" : this.Level >= 1 ? ("★+" + this.Level) : "";

			public int Proficiency => this.RawData.api_alv;
			public string ProficiencyText => this.Proficiency >= 1 ? (" (숙련도 " + this.Proficiency + ")") : "";

			internal SlotItem(kcsapi_slotitem rawData)
				: base(rawData)
			{
				this.Info = KanColleClient.Current.Master.SlotItems[this.RawData.api_slotitem_id] ?? SlotItemInfo.Dummy;
			}


			public void Remodel(int level, int masterId)
			{
				this.RawData.api_level = level;
				this.Info = KanColleClient.Current.Master.SlotItems[masterId] ?? SlotItemInfo.Dummy;

				this.RaisePropertyChanged(nameof(this.Info));
				this.RaisePropertyChanged(nameof(this.Level));
			}

			public override string ToString()
			{
				return $"ID = {this.Id}, Name = \"{this.Info.Name}\", Level = {this.Level}, Proficiency = {this.Proficiency}";
			}


			public static SlotItem Dummy { get; } = new SlotItem(new kcsapi_slotitem { api_slotitem_id = -1, });
		}
	}
}
