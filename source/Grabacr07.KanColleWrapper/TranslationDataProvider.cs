using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Models.Translations;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// Provides translation data storage capabilities
	/// </summary>
	public static class TranslationDataProvider
	{
		/// <summary>
		/// Currently selected culture.
		/// </summary>
		private static string currentCulture;

		public static string CurrentCulture
		{
			get { return currentCulture; }
			private set
			{
				currentCulture = value ?? CultureInfo.CurrentCulture.Name;
				currentCulture = (currentCulture.StartsWith("en")) ? "en" : currentCulture;
				currentCulture = (currentCulture.StartsWith("ja")) ? "ja" : currentCulture;
				// If culture is set to "(auto)", check if we support the translations for the current system-wide culture.
				if ((value == null) && !IsCultureSupported(CultureInfo.CurrentCulture.Name)) currentCulture = "en";
			}
		}

		private static string translationsPath;

		private static TranslationDatabase translationSets = new TranslationDatabase();

		private static bool EnableSubmission => KanColleClient.Current?.Settings?.EnableAutosubmission ?? false;

		public static event EventHandler<ProcessUnknownEventArgs> ProcessUnknown;

		static TranslationDataProvider()
		{
			translationsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Smooth and Flat", "KanColleViewer", "Translations");
			CurrentCulture = KanColleClient.Current?.Settings?.Culture;
		}

		private static string SerialisationPath(TranslationProviderType type, string culture)
			=> Path.Combine(translationsPath, culture, type.ToString() + ".xml");

		private static string SerialisationPath(TranslationProviderType type)
			=> SerialisationPath(type, CurrentCulture);

		/// <summary>
		/// Changes culture and loads or re-loads translation files.
		/// </summary>
		/// <param name="culture">The culture to use for translations.</param>
		/// <param name="firstRun">If true, forces loading local cache and skips calling ChangeCulture() in Updater and Translations.</param>
		public static void ChangeCulture(string culture, bool firstRun = false)
		{
			if (culture == null) culture = CultureInfo.CurrentCulture.Name;
			culture = (culture.StartsWith("en")) ? "en" : culture;
			culture = (culture.StartsWith("ja")) ? "ja" : culture;
			if (culture == CurrentCulture && !firstRun)
				return;

			Debug.WriteLine("TranslationDataProvider: got <" + culture + "> in a culture change request.");

			CurrentCulture = culture;
			LoadLocalTranslations();
			if (firstRun) return;

			KanColleClient.Current.Translations.ChangeCulture();
			KanColleClient.Current.Updater.ChangeCulture();
		}

		/// <summary>
		/// Parses JSON received from the server
		/// </summary>
		/// <param name="type">Translation provider</param>
		/// <param name="culture">Culture</param>
		/// <param name="jsonBytes">JSON data</param>
		/// <returns></returns>
		private static bool LoadJson(TranslationProviderType type, string culture, byte[] jsonBytes)
		{
			Debug.WriteLine("TranslationDataProvider: Provider {0}: JSON parsing was requested for culture <{1}>.", type, culture);

			var accessor = Tuple.Create(type, culture);

			switch (type)
			{
				case TranslationProviderType.Ships:
					ShipTranslationSet ships;
					if (!TryParse(jsonBytes, out ships)) return false;
					translationSets[accessor] = ships;
					return true;

				case TranslationProviderType.Quests:
					QuestTranslationSet quests;
					if (!TryParse(jsonBytes, out quests)) return false;
					translationSets[accessor] = quests;
					return true;

				case TranslationProviderType.ShipTypes:
					ShipTypeTranslationSet shiptypes;
					if (!TryParse(jsonBytes, out shiptypes)) return false;
					translationSets[accessor] = shiptypes;
					return true;

				case TranslationProviderType.Equipment:
					EquipmentTranslationSet equipment;
					if (!TryParse(jsonBytes, out equipment)) return false;
					translationSets[accessor] = equipment;
					return true;
			}
			return false;
		}

		public static bool LoadJson(TranslationProviderType type, byte[] jsonBytes)
			=> LoadJson(type, CurrentCulture, jsonBytes);

		private static string Lookup(TranslationType type, string culture, object rawData)
		{
			string lookupData;

			// Determine look-up fields and queries
			switch (type)
			{
				case TranslationType.Ships:
					lookupData = (rawData as kcsapi_mst_ship).api_name;
					break;
				case TranslationType.ShipTypes:
					lookupData = (rawData as kcsapi_mst_stype).api_id.ToString();
					break;
				case TranslationType.Equipment:
					lookupData = (rawData as kcsapi_mst_slotitem).api_name;
					break;
				case TranslationType.QuestTitle:
				case TranslationType.QuestDetail:
					lookupData = (rawData as kcsapi_quest).api_no.ToString();
					break;
				//case TranslationType.OperationMaps:
				//	lookupData = (rawData as kcsapi_battleresult).api_quest_name;
				//	break;
				//case TranslationType.OperationSortie:
				//	lookupData = (rawData as kcsapi_battleresult).api_enemy_info.api_deck_name;
				//	break;
				default:
					// Unsupported resource type requested.
					return null;
			}

			var result = LookupInsideProvider(type, lookupData);
			if ((result == null) && EnableSubmission)
			{
				ProcessUnknown?.Invoke(null, new ProcessUnknownEventArgs(TypeToProviderType(type), culture, rawData));
			}
			return result;
		}

		public static string Lookup(TranslationType type, object rawData)
			=> Lookup(type, CurrentCulture, rawData);

		private static string LookupInsideProvider(TranslationType type, string culture, string key)
		{
			var accessor = Tuple.Create(TypeToProviderType(type), culture);

			if (!translationSets.ContainsKey(accessor)) return null;
			if (translationSets[accessor] == null) return null;

			string result = null;

			switch (type)
			{
				case TranslationType.Equipment:
					result = (translationSets[accessor] as EquipmentTranslationSet)?.equipment?.FirstOrDefault(x => x.name_ja == key)?.name;
					break;
				case TranslationType.Ships:
					result = (translationSets[accessor] as ShipTranslationSet)?.ships?.FirstOrDefault(x => x.name_ja == key)?.name;
					break;
				case TranslationType.ShipTypes:
					result = (translationSets[accessor] as ShipTypeTranslationSet)?.shiptypes?.FirstOrDefault(x => x.id.ToString() == key)?.name;
					break;
				case TranslationType.QuestDetail:
					result = (translationSets[accessor] as QuestTranslationSet)?.quests?.FirstOrDefault(x => x.id.ToString() == key)?.description;
					break;
				case TranslationType.QuestTitle:
					result = (translationSets[accessor] as QuestTranslationSet)?.quests?.FirstOrDefault(x => x.id.ToString() == key)?.title;
					break;
				//case TranslationType.OperationMaps:
				//	return (translationSets[accessor] as OperationTranslationSet).operations.FirstOrDefault(x => x.name_ja == key).name;
				//case TranslationType.OperationSortie:
				//	return (translationSets[accessor] as OperationTranslationSet).operations.FirstOrDefault(x => x.name_ja == key).name;
				//	break;
				//case TranslationType.ExpeditionTitle:
				//	break;
				//case TranslationType.ExpeditionDetail:
				//	break;
			}
			Debug.WriteLine("{0} ({1}): lookup for {2} resulted in {3}", type, culture, key , result ?? "no match");

			return !string.IsNullOrEmpty(result) ? result: null;
		}

		public static string LookupInsideProvider(TranslationType type, string key)
			=> LookupInsideProvider(type, CurrentCulture, key);

		private static string Version(TranslationProviderType type, string culture)
		{
			var accessor = Tuple.Create(type, culture);
			if (!translationSets.ContainsKey(accessor)) return null;
			return translationSets[accessor]?.version;
		}

		public static string Version(TranslationProviderType type)
			=> Version(type, CurrentCulture);

		/// <summary>
		/// Serialise data to local storage
		/// </summary>
		/// <param name="type"></param>
		/// <param name="culture"></param>
		private static void SaveXml(TranslationProviderType type, string culture)
		{
			if (culture.StartsWith("ja")) return;

			Directory.CreateDirectory(Path.Combine(translationsPath, culture));

			using (StreamWriter dataWriter = new StreamWriter(SerialisationPath(type, culture)))
			{
				XmlSerializer dataXmlSerializer;
				var accessor = Tuple.Create(type, culture);

				switch (type)
				{
					case TranslationProviderType.Ships:
						dataXmlSerializer = new XmlSerializer(typeof(ShipTranslationSet));
						dataXmlSerializer.Serialize(dataWriter, translationSets[accessor] as ShipTranslationSet);
						break;
					case TranslationProviderType.ShipTypes:
						dataXmlSerializer = new XmlSerializer(typeof(ShipTypeTranslationSet));
						dataXmlSerializer.Serialize(dataWriter, translationSets[accessor] as ShipTypeTranslationSet);
						break;
					case TranslationProviderType.Equipment:
						dataXmlSerializer = new XmlSerializer(typeof(EquipmentTranslationSet));
						dataXmlSerializer.Serialize(dataWriter, translationSets[accessor] as EquipmentTranslationSet);
						break;
					case TranslationProviderType.Quests:
						dataXmlSerializer = new XmlSerializer(typeof(QuestTranslationSet));
						dataXmlSerializer.Serialize(dataWriter, translationSets[accessor] as QuestTranslationSet);
						break;
					//case TranslationProviderType.Operations:
					//	break;
					//case TranslationProviderType.Expeditions:
					//	break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}
		}

		public static void SaveXml(TranslationProviderType type)
			=> SaveXml(type, CurrentCulture);

		private static bool LoadXml(TranslationProviderType type, string culture)
		{
			if (!File.Exists(SerialisationPath(type, culture)) || culture.StartsWith("ja"))
				return false;

			using (FileStream dataReader = new FileStream(SerialisationPath(type, culture), FileMode.Open))
			{
				XmlSerializer dataXmlSerializer;

				switch (type)
				{
					case TranslationProviderType.Ships:
						dataXmlSerializer = new XmlSerializer(typeof(ShipTranslationSet));
						break;
					case TranslationProviderType.ShipTypes:
						dataXmlSerializer = new XmlSerializer(typeof(ShipTypeTranslationSet));
						break;
					case TranslationProviderType.Equipment:
						dataXmlSerializer = new XmlSerializer(typeof(EquipmentTranslationSet));
						break;
					case TranslationProviderType.Quests:
						dataXmlSerializer = new XmlSerializer(typeof(QuestTranslationSet));
						break;
					//case TranslationProviderType.Operations:
					//	break;
					//case TranslationProviderType.Expeditions:
					//	break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}

				translationSets.Update(type, culture, dataXmlSerializer.Deserialize(dataReader));
				return true;
			}
		}

		public static bool LoadXml(TranslationProviderType type)
			=> LoadXml(type, CurrentCulture);

		/// <summary>
		/// Deserialises translations from local storage. Should only be called during culture change requests.
		/// </summary>
		private static void LoadLocalTranslations(string culture)
		{
			if (culture.StartsWith("ja")) return;

			foreach (TranslationProviderType type in Enum.GetValues(typeof(TranslationProviderType)))
			{
				if (type == TranslationProviderType.App) continue;

				LoadXml(type, culture);
			}
		}

		public static void LoadLocalTranslations()
			=> LoadLocalTranslations(CurrentCulture);

		/// <summary>
		/// Are translations for the given culture supported?
		/// Currently a stub.
		/// TODO: Updater should provide a list of supported cultures.
		/// </summary>
		/// <param name="culture">Culture to check translation availability for</param>
		/// <returns>True if translations are supported</returns>
		public static bool IsCultureSupported(string culture)
		{
			return culture.StartsWith("ja") || (culture == "zh-CN") || (culture == "ko-KR") || culture.StartsWith("en");
		}

		public class TranslationDatabase : Dictionary<Tuple<TranslationProviderType, string>, BaseTranslationSet>
		{
			public void Update(TranslationProviderType type, string culture, object set)
			{
				var accessor = Tuple.Create(type, culture);
				switch (type)
				{
					case TranslationProviderType.Ships:
						this[accessor] = (ShipTranslationSet)set;
						break;
					case TranslationProviderType.ShipTypes:
						this[accessor] = (ShipTypeTranslationSet)set;
						break;
					case TranslationProviderType.Equipment:
						this[accessor] = (EquipmentTranslationSet)set;
						break;
					case TranslationProviderType.Quests:
						this[accessor] = (QuestTranslationSet)set;
						break;
					//case TranslationProviderType.Operations:
					//	break;
					//case TranslationProviderType.Expeditions:
					//	break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}
		}

		public static TranslationProviderType? StringToTranslationProviderType(string typeString)
		{
			switch (typeString)
			{
				case "app":
					return TranslationProviderType.App;
				case "ships":
					return TranslationProviderType.Ships;
				case "shiptypes":
					return TranslationProviderType.ShipTypes;
				case "equipment":
					return TranslationProviderType.Equipment;
				case "quests":
					return TranslationProviderType.Quests;
				case "operations":
					return TranslationProviderType.Operations;
				case "expeditions":
					return TranslationProviderType.Expeditions;
				//default:
				//	throw new NotImplementedException("Unknown provider type");
			}
			return null;
		}

		public static TranslationProviderType TypeToProviderType(TranslationType type)
		{
			switch (type)
			{
				case TranslationType.App:
					return TranslationProviderType.App;
				case TranslationType.Equipment:
					return TranslationProviderType.Equipment;
				case TranslationType.Ships:
					return TranslationProviderType.Ships;
				case TranslationType.ShipTypes:
					return TranslationProviderType.ShipTypes;
				case TranslationType.Operations:
				case TranslationType.OperationMaps:
				case TranslationType.OperationSortie:
					return TranslationProviderType.Operations;
				case TranslationType.Quests:
				case TranslationType.QuestDetail:
				case TranslationType.QuestTitle:
					return TranslationProviderType.Quests;
				case TranslationType.Expeditions:
				case TranslationType.ExpeditionTitle:
				case TranslationType.ExpeditionDetail:
					return TranslationProviderType.Expeditions;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		public static T Parse<T>(byte[] bytes)
		{
			var serialiser = new DataContractJsonSerializer(typeof(T));
			using (var stream = new MemoryStream(bytes))
			{
				return (T)serialiser.ReadObject(stream);
			}
		}

		public static bool TryParse<T>(byte[] bytes, out T result)
		{
			try
			{
				result = Parse<T>(bytes);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				result = default(T);
				return false;
			}
			return true;
		}
	}
}