using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class Translations : NotificationObject
	{
		/// <summary>
		/// Provides documents and storage information for different resource sets.
		/// </summary>
		private class TranslationModel
		{
			/// <summary>
			/// The complete XML document for the resource.
			/// </summary>
			public XDocument Document { get; set; }
			public ITranslationContainer Table { get; set; }
			/// <summary>
			/// Local storage: file name.
			/// </summary>
			public string Filename { get; set; }
			/// <summary>
			/// Returns the full path to the resource file.
			/// </summary>
			public string FilePath => Path.Combine(TranslationsPath, CurrentCulture, this.Filename);
		}

		/// <summary>
		/// Pre-initialised set of resource storage data.
		/// </summary>
		private Dictionary<TranslationType, TranslationModel> TranslationData = new Dictionary<TranslationType, TranslationModel>()
		{
			{ TranslationType.Ships, new TranslationModel{Document = null, Filename = "Ships.xml", Table = new TranslationContainer(TranslationType.Ships, "ship", "name_ja", "name")} },
			{ TranslationType.ShipTypes, new TranslationModel{Document = null, Filename = "ShipTypes.xml", Table = new TranslationContainer(TranslationType.ShipTypes, "shiptype", "id", "name")} },
			{ TranslationType.Equipment, new TranslationModel{Document = null, Filename = "Equipment.xml", Table = new TranslationContainer(TranslationType.Equipment, "item", "name_ja", "name")} },
			{ TranslationType.QuestTitle, new TranslationModel{Document = null, Filename = "Quests.xml", Table = new TranslationContainer(TranslationType.QuestTitle, "quest", "id", "title")} },
			{ TranslationType.QuestDetail, new TranslationModel{Document = null, Filename = "Quests.xml", Table = new TranslationContainer(TranslationType.QuestDetail, "quest", "id", "description")} },
			// { TranslationType.Operations, new TranslationModel{Document = null, Filename = "Operations.xml", ArrayName = "operation"} },
			// { TranslationType.Expeditions, new TranslationModel{Document = null, Filename = "Expeditions.xml", ArrayName = "expedition"} },
		};

		/// <summary>
		/// Culture selected for translations.
		/// </summary>
		private static string CurrentCulture;

		/// <summary>
		/// Path to translation files.
		/// Default is Translations in the current directory.
		/// </summary>
		private static string TranslationsPath = "Translations";

		/// <summary>
		/// Settings, Enable translations: allows the display of translated resources.
		/// </summary>
		public bool EnableTranslations { get; private set; }

		internal Translations() : this("en") { }
		/// <summary>
		/// The constructor performs initial loading of translation files.
		/// </summary>
		/// <param name="culture">The culture to use for translations. Defaults to "en-US".</param>
		internal Translations(string culture)
		{
			Debug.WriteLine(this.GetType().Name + ": Constructor initialising with <" + culture + ">.");
			this.EnableTranslations = true;
			this.ChangeCulture(culture);
		}

		/// <summary>
		/// Reloads translations for a given resource type.
		/// </summary>
		/// <param name="type">Resource type.</param>
		private void ReloadTranslations(TranslationType type)
		{
			// Create directories if they do not yet exist
			Directory.CreateDirectory(Path.Combine(TranslationsPath, CurrentCulture));

			if (!TranslationData.ContainsKey(type)) return;

			try
			{
				TranslationData[type].Document = null;
				TranslationData[type].Table.Clear();
				if (File.Exists(TranslationData[type].FilePath)) TranslationData[type].Document = XDocument.Load(TranslationData[type].FilePath);
				if ((TranslationData[type].Table != null) && (TranslationData[type].Document != null))
					TranslationData[type].Table.Load(TranslationData[type].Document);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Reloads translations for multiple resource types.
		/// </summary>
		/// <param name="types">Array containing resource types to reload.</param>
		private void ReloadTranslations(params TranslationType[] types)
		{
			foreach (var type in types) this.ReloadTranslations(type);
		}

		/// <summary>
		/// Changes culture and loads or re-loads translation files.
		/// </summary>
		/// <param name="culture">The culture to use for translations.</param>
		public void ChangeCulture(string culture)
		{
			Debug.WriteLine("Got <" + culture + "> in a culture change request.");

			if ((culture == null) || (culture == CurrentCulture))
				return;

			CurrentCulture = culture;

			if (!EnableTranslations || CurrentCulture.StartsWith("ja"))
			{
				foreach (var translationData in TranslationData) translationData.Value.Document = null;
				this.RaisePropertyChanged();
				return;
			}

			this.ReloadTranslations(TranslationType.Equipment, TranslationType.QuestTitle, TranslationType.QuestDetail, TranslationType.Ships, TranslationType.ShipTypes /*, TranslationType.Operations, TranslationType.Expeditions*/);
			this.RaisePropertyChanged();
		}

		/// <summary>
		/// Looks up a translated string for a resource.
		/// </summary>
		/// <param name="type">Resource type.</param>
		/// <param name="rawData">Raw API data; provides the necessary data (ID, names, etc.) for a look-up.</param>
		/// <returns></returns>
		public string Lookup(TranslationType type, object rawData)
		{
			if (!EnableTranslations || CurrentCulture.StartsWith("ja"))
			{
				Debug.WriteLine("Lookup called for {0} but translations are disabled or culture is Japanese.", type);
				return null;
			}

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

			return TranslationData[type].Table.Lookup(lookupData);
		}

		/// <summary>
		/// Provides support for resource versioning.
		/// </summary>
		/// <param name="type">Resource type.</param>
		/// <returns>Version of the resource requested.</returns>
		public string Version(TranslationType type)
		{
			if (TranslationData.ContainsKey(type)) return TranslationData[type]?.Document?.Root?.Attribute("Version")?.Value ?? "0";
			throw new System.NotImplementedException("Versioning for this resource type is not supported.");
		}

		interface ITranslationContainer
		{
			TranslationType Type { get; }
			string Lookup(object key);
			void Load(XDocument document);
			void Clear();
		}

		class TranslationContainer : ITranslationContainer
		{
			public TranslationType Type { get; private set; }
			public string Key { get; private set; }
			public string Field { get; private set; }
			public string Element { get; private set; }

			private Dictionary<string, string> tableDictionary;

			public TranslationContainer(TranslationType type, string element, string key, string field)
			{
				Type = type;
				Key = key;
				Field = field;
				Element = element;
			}

			public void Load(XDocument document)
			{
				tableDictionary = null;
				tableDictionary = new Dictionary<string, string>();

				foreach (var el in document.Descendants(Element))
				{
					if ((el.Element(Key) != null) && (el.Element(Field) != null))
						tableDictionary.Add(el.Element(Key).Value, el.Element(Field).Value);
				}

				Debug.WriteLine("Loaded translations for {0}: {1} element(s).", Type, tableDictionary.Count);
			}

			public string Lookup(object key)
			{
				var lookup = key as string;
				Debug.WriteLine("Matching {0}: {1}.", lookup, tableDictionary.ContainsKey(lookup) ? tableDictionary[lookup] : "no match");
				if ((lookup !=null) && (tableDictionary?.ContainsKey(lookup) ?? false)) return tableDictionary[lookup];
				return null;
			}

			public void Clear()
			{
				tableDictionary?.Clear();
			}
		}
	}

	static class TranslationsExtensions
	{
		public static bool In<T>(this T item, params T[] list)
		{
			return list.Contains(item);
		}
	}
}
