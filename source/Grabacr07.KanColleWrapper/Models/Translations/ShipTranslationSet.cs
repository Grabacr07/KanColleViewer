using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Grabacr07.KanColleWrapper.Models.Translations
{
	// ReSharper disable InconsistentNaming
	[DataContract]
	[Serializable]
	public class ShipTranslationSet : BaseTranslationSet
	{
		[DataMember] public List<SimpleStringTranslationItem> ships { get; set; }
	}
}