using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Grabacr07.KanColleWrapper.Models.Translations
{
	// ReSharper disable InconsistentNaming
	[DataContract]
	[Serializable]
	public class EquipmentTranslationSet : BaseTranslationSet
	{
		[DataMember] public List<SimpleStringTranslationItem> equipment { get; set; }
	}
}