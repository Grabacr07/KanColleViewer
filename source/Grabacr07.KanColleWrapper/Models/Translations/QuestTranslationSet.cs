using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Grabacr07.KanColleWrapper.Models.Translations
{
	// ReSharper disable InconsistentNaming
	[DataContract]
	[Serializable]
	public class QuestTranslationSet : BaseTranslationSet
	{
		[DataMember] public List<QuestTranslationItem> quests { get; set; }
	}

	public class QuestTranslationItem
	{
		public int id { get; set; }
		public string title { get; set; }
		public string title_ja { get; set; }
		public string description { get; set; }
		public string description_ja { get; set; }
	}
}