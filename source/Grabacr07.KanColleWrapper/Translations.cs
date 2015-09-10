using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			/// <summary>
			/// Local storage: file name.
			/// </summary>
			public string Filename { get; set; }
			/// <summary>
			/// API: array name.
			/// </summary>
			public string ArrayName { get; set; }
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
			{ TranslationType.Ships, new TranslationModel{Document = null, Filename = "Ships.xml", ArrayName = "ship"} },
			{ TranslationType.ShipTypes, new TranslationModel{Document = null, Filename = "ShipTypes.xml", ArrayName = "shiptype"} },
			{ TranslationType.Equipment, new TranslationModel{Document = null, Filename = "Equipment.xml", ArrayName = "item"} },
			{ TranslationType.Quests, new TranslationModel{Document = null, Filename = "Quests.xml", ArrayName = "quest"} },
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

		internal Translations() : this("en-US") { }
		/// <summary>
		/// The constructor performs initial loading of translation files.
		/// </summary>
		/// <param name="culture">The culture to use for translations. Defaults to "en-US".</param>
		internal Translations(string culture)
		{
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
				if (File.Exists(TranslationData[type].FilePath)) TranslationData[type].Document = XDocument.Load(TranslationData[type].FilePath);
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
			CurrentCulture = culture;

			if (!EnableTranslations || CurrentCulture == "ja-JP")
			{
				foreach (var translationData in TranslationData) translationData.Value.Document = null;
				return;
			}

			this.ReloadTranslations(TranslationType.Equipment, TranslationType.Quests, TranslationType.Ships, TranslationType.ShipTypes /*, TranslationType.Operations, TranslationType.Expeditions*/);
		}

		/// <summary>
		/// Obtains an XML set of translations for the requested resource type.
		/// </summary>
		/// <param name="type">Resource type.</param>
		/// <returns></returns>
		private IEnumerable<XElement> GetTranslationSet(TranslationType type)
		{
			var localType = type;
			switch (type)
			{
				case TranslationType.QuestTitle:
				case TranslationType.QuestDetail:
					localType = TranslationType.Quests;
					break;
				case TranslationType.OperationMaps:
				case TranslationType.OperationSortie:
					localType = TranslationType.Operations;
					break;
			}
			return (!TranslationData.ContainsKey(localType) || (TranslationData[localType].Document == null)) ? null : TranslationData[localType].Document.Descendants(TranslationData[localType].ArrayName);
		} 

		/// <summary>
		/// Looks up a translated string for a resource.
		/// </summary>
		/// <param name="type">Resource type.</param>
		/// <param name="rawData">Raw API data; provides the necessary data (ID, names, etc.) for a look-up.</param>
		/// <returns></returns>
		public string Lookup(TranslationType type, object rawData)
		{
			string lookupField = "name_ja";
			string resultField = "name";
			string lookupData;

			// Determine look-up fields and queries
			switch (type)
			{
				case TranslationType.Ships:
					lookupData = (rawData as kcsapi_mst_ship).api_name;
					break;
				case TranslationType.ShipTypes:
					lookupField = "id";
					lookupData = (rawData as kcsapi_mst_stype).api_id.ToString();
					break;
				case TranslationType.Equipment:
					lookupField = "id";
					lookupData = (rawData as kcsapi_mst_slotitem).api_sortno.ToString();
					break;
				case TranslationType.QuestTitle:
					lookupField = "id";
					resultField = "title";
					lookupData = (rawData as kcsapi_quest).api_no.ToString();
					break;
				case TranslationType.QuestDetail:
					lookupField = "id";
					resultField = "description";
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

			Debug.WriteLine("Translation look-up of type {0} in {1} for {2} results in {3}.", type, lookupField, lookupData, this.GetTranslationSet(type)?.FirstOrDefault(b => b.Element(lookupField).Value.Equals(lookupData))?.Element(resultField)?.Value ?? "NO MATCH");
			return this.GetTranslationSet(type)?.FirstOrDefault(b => b.Element(lookupField).Value.Equals(lookupData))?.Element(resultField)?.Value;
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
	}

	static class TranslationsExtensions
	{
		public static bool In<T>(this T item, params T[] list)
		{
			return list.Contains(item);
		}
	}
}
