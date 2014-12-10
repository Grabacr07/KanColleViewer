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
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
		/// <summary>
		/// 업데이트 상태를 구별한다.
		/// bool값을 조정하며 이는 업데이트 후 바로 퀘스트 로드가 적용되지 않는 문제점을 자체 해결하기 위해 도입한것임.
		/// 
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
				XDocument TestXML;
				int ReturnValue = 0;

				try
				{
					if (!Directory.Exists(MainFolder + "Translations")) Directory.CreateDirectory(MainFolder + "Translations");
					if (!Directory.Exists(MainFolder + "Translations\\tmp\\")) Directory.CreateDirectory(MainFolder + "Translations\\tmp\\");

					// In every one of these we download it to a temp folder, check if the file works, then move it over.
					if (IsOnlineVersionGreater(TranslationType.Equipment, TranslationsRef.EquipmentVersion))
					{
						Client.DownloadFile(BaseTranslationURL+ "Equipment.xml", MainFolder + "Translations\\tmp\\Equipment.xml");

						try
						{
							TestXML = XDocument.Load(MainFolder + "Translations\\tmp\\Equipment.xml");
							if (File.Exists(MainFolder + "Translations\\Equipment.xml"))
								File.Delete(MainFolder + "Translations\\Equipment.xml");
							File.Move(MainFolder + "Translations\\tmp\\Equipment.xml", MainFolder + "Translations\\Equipment.xml");
							ReturnValue = 1;
							UpdateState(ReturnValue, TranslationType.Equipment);
						}
						catch
						{
							ReturnValue = -1;
						}
					}

					if (IsOnlineVersionGreater(TranslationType.Operations, TranslationsRef.OperationsVersion))
					{
						Client.DownloadFile(BaseTranslationURL+ "Operations.xml", MainFolder + "Translations\\tmp\\Operations.xml");

						try
						{
							TestXML = XDocument.Load(MainFolder + "Translations\\tmp\\Operations.xml");
							if (File.Exists(MainFolder + "Translations\\Operations.xml"))
								File.Delete(MainFolder + "Translations\\Operations.xml");
							File.Move(MainFolder + "Translations\\tmp\\Operations.xml", MainFolder + "Translations\\Operations.xml");
							ReturnValue = 1;
							UpdateState(ReturnValue, TranslationType.Operations);
						}
						catch
						{
							ReturnValue = -1;
						}
					}

					if (IsOnlineVersionGreater(TranslationType.Quests, TranslationsRef.QuestsVersion))
					{
						Client.DownloadFile(BaseTranslationURL+ "Quests.xml", MainFolder + "Translations\\tmp\\Quests.xml");

						try
						{
							TestXML = XDocument.Load(MainFolder + "Translations\\tmp\\Quests.xml");
							if (File.Exists(MainFolder + "Translations\\Quests.xml"))
								File.Delete(MainFolder + "Translations\\Quests.xml");
							File.Move(MainFolder + "Translations\\tmp\\Quests.xml", MainFolder + "Translations\\Quests.xml");
							ReturnValue = 1;
							UpdateState(ReturnValue,TranslationType.Quests);
						}
						catch
						{
							ReturnValue = -1;
						}
					}
					if (IsOnlineVersionGreater(TranslationType.Expeditions, TranslationsRef.ExpeditionsVersion))
					{
						Client.DownloadFile(BaseTranslationURL + "Expeditions.xml", MainFolder + "Translations\\tmp\\Expeditions.xml");

						try
						{
							TestXML = XDocument.Load(MainFolder + "Translations\\tmp\\Expeditions.xml");
							if (File.Exists(MainFolder + "Translations\\Expeditions.xml"))
								File.Delete(MainFolder + "Translations\\Expeditions.xml");
							File.Move(MainFolder + "Translations\\tmp\\Expeditions.xml", MainFolder + "Translations\\Expeditions.xml");
							ReturnValue = 1;
							UpdateState(ReturnValue, TranslationType.Expeditions);
						}
						catch
						{
							ReturnValue = -1;
						}
					}

					if (IsOnlineVersionGreater(TranslationType.Ships, TranslationsRef.ShipsVersion))
					{
						Client.DownloadFile(BaseTranslationURL+ "Ships.xml", MainFolder + "Translations\\tmp\\Ships.xml");

						try
						{
							TestXML = XDocument.Load(MainFolder + "Translations\\tmp\\Ships.xml");
							if (File.Exists(MainFolder + "Translations\\Ships.xml"))
								File.Delete(MainFolder + "Translations\\Ships.xml");
							File.Move(MainFolder + "Translations\\tmp\\Ships.xml", MainFolder + "Translations\\Ships.xml");
							ReturnValue = 1;
							UpdateState(ReturnValue,TranslationType.Ships);
						}
						catch
						{
							ReturnValue = -1;
						}
					}

					if (IsOnlineVersionGreater(TranslationType.ShipTypes, TranslationsRef.ShipTypesVersion))
					{
						Client.DownloadFile(BaseTranslationURL+ "ShipTypes.xml", MainFolder + "Translations\\tmp\\ShipTypes.xml");

						try
						{
							TestXML = XDocument.Load(MainFolder + "Translations\\tmp\\ShipTypes.xml");
							if (File.Exists(MainFolder + "Translations\\ShipTypes.xml"))
								File.Delete(MainFolder + "Translations\\ShipTypes.xml");
							File.Move(MainFolder + "Translations\\tmp\\ShipTypes.xml", MainFolder + "Translations\\ShipTypes.xml");
							ReturnValue = 1;
							UpdateState(ReturnValue,TranslationType.ShipTypes);
						}
						catch
						{
							ReturnValue = -1;
						}
					}

				}
				catch
				{
					// Failed to download files.
					return -1;
				}

				if (Directory.Exists(MainFolder + "Translations\\tmp\\")) Directory.Delete(MainFolder + "Translations\\tmp\\");

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

			}

			return false;
		}

	}
}