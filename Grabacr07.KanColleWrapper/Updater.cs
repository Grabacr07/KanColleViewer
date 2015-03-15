using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Grabacr07.KanColleWrapper
{
	public class Updater
	{
		public bool EquipUpdate { get; set; }
		public bool OperationsUpdate { get; set; }
		public bool QuestUpdate { get; set; }
		public bool ShipsUpdate { get; set; }
		public bool ShipTypesUpdate { get; set; }
		public bool ExpeditionUpdate { get; set; }
		public bool RemodelUpdate { get; set; }
		/// <summary>
		/// 업데이트 상태를 구별한다.
		/// bool값을 조정하며 이는 업데이트 후 바로 퀘스트 로드가 적용되지 않는 문제점을 자체 해결하기 위해 도입한것임.
		/// </summary>
		/// <param name="ReturnVal">-1,0,1을 받는다. 사실상 별 의미 없는거같기도</param>
		/// <param name="type">업데이트 된 항목이 정확히 무엇인지 구별한다</param>
		/// <returns></returns>
		public void UpdateState(int ReturnVal, TranslationType type)
		{
			if (ReturnVal == 1)
			{
				switch (type)
				{
					case TranslationType.Equipment:
						this.EquipUpdate = true;
						break;
					case TranslationType.Operations:
						this.OperationsUpdate = true;
						break;
					case TranslationType.Quests:
						this.QuestUpdate = true;
						break;
					case TranslationType.Ships:
						this.ShipsUpdate = true;
						break;
					case TranslationType.ShipTypes:
						this.ShipTypesUpdate = true;
						break;
					case TranslationType.Expeditions:
						this.ExpeditionUpdate = true;
						break;
					case TranslationType.RemodelSlots:
						this.RemodelUpdate = true;
						break;
				}
			}
		}

		private XDocument VersionXML;

		/// <summary>
		/// Loads the version XML file from a remote URL. This houses all current online version info.
		/// </summary>
		/// <param name="UpdateURL">String URL to the version XML file.</param>
		/// <returns>True: Successful, False: Failed</returns>
		public bool LoadVersion(string UpdateURL)
		{
			try
			{
				VersionXML = XDocument.Load(UpdateURL);

				if (VersionXML == null)
					return false;
			}
			catch
			{
				// Couldn't download xml file?
				return false;
			}

			return true;
		}

		private int TranslationFileWizard(string MainFolder, string Xmlname, TranslationType type)
		{
			int ReturnValue;
			try
			{
				if (File.Exists(Path.Combine(MainFolder, "Translations", Xmlname)))
				{
					if (File.Exists(Path.Combine(MainFolder, "Translations", "Old", Xmlname + ".old")))
						File.Delete(Path.Combine(MainFolder, "Translations", "Old", Xmlname + ".old"));
					File.Move(Path.Combine(MainFolder, "Translations", Xmlname), Path.Combine(MainFolder, "Translations", "Old", Xmlname + ".old"));
				}
				File.Move(Path.Combine(MainFolder, "Translations", "tmp", Xmlname), Path.Combine(MainFolder, "Translations", Xmlname));
				ReturnValue = 1;
				UpdateState(ReturnValue, type);
			}
			catch
			{
				ReturnValue = -1;
			}
			return ReturnValue;
		}

		/// <summary>
		/// Updates any translation files that differ from that found online.
		/// </summary>
		/// <param name="BaseTranslationURL">String URL folder that contains all the translation XML files.</param>
		/// <param name="Culture">Language version to download</param>
		/// <param name="TranslationsRef">Link to the translation engine to obtain current translation versions.</param>
		/// <returns>Returns a state code depending on how it ran. [-1: Error, 0: Nothing to update, 1: Update Successful]</returns>
		public int UpdateTranslations(string BaseTranslationURL, Translations TranslationsRef)
		{
			using (WebClient Client = new WebClient())
			{
				int ReturnValue = 0;
				string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

				try
				{
					if (!Directory.Exists(Path.Combine(MainFolder, "Translations"))) Directory.CreateDirectory(Path.Combine(MainFolder, "Translations"));
					if (!Directory.Exists(Path.Combine(MainFolder, "Translations", "tmp"))) Directory.CreateDirectory(Path.Combine(MainFolder, "Translations", "tmp"));
					if (!Directory.Exists(Path.Combine(MainFolder, "Translations", "Old")))
						Directory.CreateDirectory(Path.Combine(MainFolder, "Translations", "Old"));

					// In every one of these we download it to a temp folder, check if the file works, then move it over.
					if (IsOnlineVersionGreater(TranslationType.Equipment, TranslationsRef.EquipmentVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "Equipment.xml", Path.Combine(MainFolder, "Translations", "tmp", "Equipment.xml"));
						ReturnValue = TranslationFileWizard(MainFolder, "Equipment.xml", TranslationType.Equipment);
					}

					if (IsOnlineVersionGreater(TranslationType.Operations, TranslationsRef.OperationsVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "Operations.xml", Path.Combine(MainFolder, "Translations", "tmp", "Operations.xml"));
						ReturnValue = TranslationFileWizard(MainFolder, "Operations.xml", TranslationType.Operations);
					}

					if (IsOnlineVersionGreater(TranslationType.Quests, TranslationsRef.QuestsVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "Quests.xml", Path.Combine(MainFolder, "Translations", "tmp", "Quests.xml"));
						ReturnValue = TranslationFileWizard(MainFolder, "Quests.xml", TranslationType.Quests);
					}
					if (IsOnlineVersionGreater(TranslationType.Expeditions, TranslationsRef.ExpeditionsVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "Expeditions.xml", Path.Combine(MainFolder, "Translations", "tmp", "Expeditions.xml"));
						ReturnValue = TranslationFileWizard(MainFolder, "Expeditions.xml", TranslationType.Expeditions);
					}

					if (IsOnlineVersionGreater(TranslationType.Ships, TranslationsRef.ShipsVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "Ships.xml", Path.Combine(MainFolder, "Translations", "tmp", "Ships.xml"));
						ReturnValue = TranslationFileWizard(MainFolder, "Ships.xml", TranslationType.Ships);
					}

					if (IsOnlineVersionGreater(TranslationType.ShipTypes, TranslationsRef.ShipTypesVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "ShipTypes.xml", Path.Combine(MainFolder, "Translations", "tmp", "ShipTypes.xml"));
						ReturnValue = TranslationFileWizard(MainFolder, "ShipTypes.xml", TranslationType.ShipTypes);
					}
					if (IsOnlineVersionGreater(TranslationType.RemodelSlots, TranslationsRef.RemodelSlotsVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "RemodelSlots.xml", Path.Combine(MainFolder, "Translations", "tmp", "RemodelSlots.xml"));
						ReturnValue = TranslationFileWizard(MainFolder, "RemodelSlots.xml", TranslationType.RemodelSlots);
					}

				}
				catch
				{
					// Failed to download files.
					return -1;
				}
				if (Directory.Exists(Path.Combine(MainFolder, "Translations", "tmp")))
					Directory.Delete(Path.Combine(MainFolder, "Translations", "tmp"), true);

				return ReturnValue;
			}
		}

		/// <summary>
		/// Uses the downloaded Version XML document to return a specific version number as a string.
		/// </summary>
		/// <param name="Type">Translation file type. Can also be for the App itself.</param>
		/// <param name="bGetURL">If true, returns the URL of the online file instead of the version.</param>
		/// <returns>String value of either the Version or URL to the file.</returns>
		public string GetOnlineVersion(TranslationType Type, bool bGetURL = false)
		{
			if (VersionXML == null)
				return "";

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName = !bGetURL ? "Version" : "URL";

			switch (Type)
			{
				case TranslationType.App:
					return Versions.Where(x => x.Element("Name").Value.Equals("App")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.Equipment:
					return Versions.Where(x => x.Element("Name").Value.Equals("Equipment")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.Operations:
				case TranslationType.OperationSortie:
				case TranslationType.OperationMaps:
					return Versions.Where(x => x.Element("Name").Value.Equals("Operations")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.Quests:
				case TranslationType.QuestDetail:
				case TranslationType.QuestTitle:
					return Versions.Where(x => x.Element("Name").Value.Equals("Quests")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.Expeditions:
				case TranslationType.ExpeditionTitle:
				case TranslationType.ExpeditionDetail:
					return Versions.Where(x => x.Element("Name").Value.Equals("Expeditions")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.Ships:
					return Versions.Where(x => x.Element("Name").Value.Equals("Ships")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.ShipTypes:
					return Versions.Where(x => x.Element("Name").Value.Equals("ShipTypes")).FirstOrDefault().Element(ElementName).Value;
				case TranslationType.RemodelSlots:
					return Versions.Where(x => x.Element("Name").Value.Equals("RemodelSlots")).FirstOrDefault().Element(ElementName).Value;
			}
			return "";
		}

		/// <summary>
		/// Conditional function to determine whether the supplied version is greater than the one found online.
		/// </summary>
		/// <param name="Type">Translation file type. Can also be for the App itself.</param>
		/// <param name="LocalVersionString">Version string of the local file to check against</param>
		/// <returns></returns>
		public bool IsOnlineVersionGreater(TranslationType Type, string LocalVersionString)
		{
			if (VersionXML == null)
				return true;

			IEnumerable<XElement> Versions = VersionXML.Root.Descendants("Item");
			string ElementName = "Version";
			if (LocalVersionString == "알 수 없음") return false;
			else if (LocalVersionString == "없음") return false;
			Version LocalVersion = new Version(LocalVersionString);


			switch (Type)
			{
				case TranslationType.App:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("App")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.Equipment:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Equipment")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.Operations:
				case TranslationType.OperationSortie:
				case TranslationType.OperationMaps:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Operations")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.Quests:
				case TranslationType.QuestDetail:
				case TranslationType.QuestTitle:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Quests")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.Expeditions:
				case TranslationType.ExpeditionTitle:
				case TranslationType.ExpeditionDetail:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Expeditions")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.Ships:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("Ships")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.ShipTypes:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("ShipTypes")).FirstOrDefault().Element(ElementName).Value)) < 0;
				case TranslationType.RemodelSlots:
					return LocalVersion.CompareTo(new Version(Versions.Where(x => x.Element("Name").Value.Equals("RemodelSlots")).FirstOrDefault().Element(ElementName).Value)) < 0;
			}

			return false;
		}

	}
}