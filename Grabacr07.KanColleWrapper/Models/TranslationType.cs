
namespace Grabacr07.KanColleWrapper.Models
{
	public enum TranslationType
	{
		/// <summary>
		/// Application Translation... Not really. Used for updates.
		/// </summary>
		App = 0,

		/// <summary>
		/// Equipment translation list
		/// </summary>
		Equipment = 1,

		/// <summary>
		/// Map and enemy fleet translation lists
		/// </summary>
		Operations = 2,

		/// <summary>
		/// Quest translation list
		/// </summary>
		Quests = 3,

		/// <summary>
		/// Ship name translation list
		/// </summary>
		Ships = 4,

		/// <summary>
		/// Ship type translation list
		/// </summary>
		ShipTypes = 5,

		/// <summary>
		/// Operation map translations only
		/// </summary>
		OperationMaps = 6,

		/// <summary>
		/// Operation enemy fleet translations only
		/// </summary>
		OperationSortie = 7,

		/// <summary>
		/// Quest detail translations only
		/// </summary>
		QuestDetail = 8,

		/// <summary>
		/// Quest title translations only
		/// </summary>
		QuestTitle = 9,
		/// <summary>
		/// Expedition list translations only
		/// </summary>
		Expeditions = 10,
		/// <summary>
		/// Expedition title translations only
		/// </summary>
		ExpeditionTitle = 11,
		/// <summary>
		/// Expedition detail translations only
		/// </summary>
		ExpeditionDetail = 12
	}
}