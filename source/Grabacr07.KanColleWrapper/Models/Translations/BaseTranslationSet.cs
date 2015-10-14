using System;
using System.Runtime.Serialization;

namespace Grabacr07.KanColleWrapper.Models.Translations
{
	// ReSharper disable InconsistentNaming
	[DataContract]
	[Serializable]
	public class BaseTranslationSet
	{
		[DataMember] public string version { get; set; }
	}
}