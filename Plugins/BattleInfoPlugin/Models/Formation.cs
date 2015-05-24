using System.Runtime.Serialization;

namespace BattleInfoPlugin.Models
{
	[DataContract]
	public enum Formation
	{
		[EnumMember]
		불명 = -1,
		[EnumMember]
		없음 = 0,
		[EnumMember]
		단종진 = 1,
		[EnumMember]
		복종진 = 2,
		[EnumMember]
		윤형진 = 3,
		[EnumMember]
		제형진 = 4,
		[EnumMember]
		단횡진 = 5,
		[EnumMember]
		대잠진형 = 11,
		[EnumMember]
		전방진형 = 12,
		[EnumMember]
		윤형진형 = 13,
		[EnumMember]
		전투진형 = 14,
		なし,
	}
}