﻿using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Grabacr07.KanColleWrapper
{
	public class Translations
	{
		private XDocument ShipsXML;//0
		private XDocument ShipTypesXML;//1
		private XDocument EquipmentXML;//2
		private XDocument OperationsXML;//3
		private XDocument QuestsXML;//4
		private XDocument ExpeditionXML;//5
		private XDocument RemodelXml;//6
        private XDocument EquipmentTypesXML;//7
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

		public bool EnableTranslations { get; set; }
		public bool EnableAddUntranslated { get; set; }

		public string EquipmentVersion { get; set; }
		public string OperationsVersion { get; set; }
		public string QuestsVersion { get; set; }
		public string ExpeditionsVersion { get; set; }
		public string ShipsVersion { get; set; }
		public string ShipTypesVersion { get; set; }
		public string RemodelSlotsVersion { get; set; }
        public string EquipmentTypesVersion { get; set; }
		private void SaveXmls(int idx)
		{
			switch (idx)
			{
				case 0:
					ShipsXML.Save(Path.Combine(MainFolder, "Translations", "Ships.xml"));
					break;
				case 1:
					ShipTypesXML.Save(Path.Combine(MainFolder, "Translations", "ShipTypes.xml"));
					break;
				case 2:
					EquipmentXML.Save(Path.Combine(MainFolder, "Translations", "Equipment.xml"));
					break;
				case 3:
					OperationsXML.Save(Path.Combine(MainFolder, "Translations", "Operations.xml"));
					break;
				case 4:
					QuestsXML.Save(Path.Combine(MainFolder, "Translations", "Quests.xml"));
					break;
				case 5:
					ExpeditionXML.Save(Path.Combine(MainFolder, "Translations", "Expeditions.xml"));
					break;
				case 6://리모델링은 세이브가 없다
					break;
                case 7:
                    EquipmentTypesXML.Save(Path.Combine(MainFolder, "Translations", "EquipmentTypes.xml"));
                    break;
            }
			LoadXmls();
		}
		private void LoadXmls()
		{
			if (File.Exists(Path.Combine(MainFolder, "Translations", "Ships.xml"))) ShipsXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Ships.xml"));
			if (File.Exists(Path.Combine(MainFolder, "Translations", "ShipTypes.xml"))) ShipTypesXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "ShipTypes.xml"));
			if (File.Exists(Path.Combine(MainFolder, "Translations", "Equipment.xml"))) EquipmentXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Equipment.xml"));
			if (File.Exists(Path.Combine(MainFolder, "Translations", "Operations.xml"))) OperationsXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Operations.xml"));
			if (File.Exists(Path.Combine(MainFolder, "Translations", "Quests.xml"))) QuestsXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Quests.xml"));
			if (File.Exists(Path.Combine(MainFolder, "Translations", "Expeditions.xml"))) ExpeditionXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Expeditions.xml"));
			if (File.Exists(Path.Combine(MainFolder, "Translations", "RemodelSlots.xml"))) RemodelXml = XDocument.Load(Path.Combine(MainFolder, "Translations", "RemodelSlots.xml"));
            if (File.Exists(Path.Combine(MainFolder, "Translations", "EquipmentTypes.xml"))) EquipmentTypesXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "EquipmentTypes.xml"));

        }
        internal Translations()
		{
			try
			{
				LoadXmls();
				GetVersions();
			}
			catch { }
		}
		private void GetVersions()
		{
			if (ShipsXML != null)
				if (ShipsXML.Root.Attribute("Version") != null) ShipsVersion = ShipsXML.Root.Attribute("Version").Value;
				else ShipsVersion = "알 수 없음";
			else
				ShipsVersion = "없음";
			if (ShipTypesXML != null)
				if (ShipTypesXML.Root.Attribute("Version") != null) ShipTypesVersion = ShipTypesXML.Root.Attribute("Version").Value;
				else ShipTypesVersion = "알 수 없음";
			else
				ShipTypesVersion = "없음";
			if (EquipmentXML != null)
				if (EquipmentXML.Root.Attribute("Version") != null) EquipmentVersion = EquipmentXML.Root.Attribute("Version").Value;
				else EquipmentVersion = "알 수 없음";
			else
				EquipmentVersion = "없음";
			if (OperationsXML != null)
				if (OperationsXML.Root.Attribute("Version") != null) OperationsVersion = OperationsXML.Root.Attribute("Version").Value;
				else OperationsVersion = "알 수 없음";
			else
				OperationsVersion = "없음";
			if (QuestsXML != null)
				if (QuestsXML.Root.Attribute("Version") != null) QuestsVersion = QuestsXML.Root.Attribute("Version").Value;
				else QuestsVersion = "알 수 없음";
			if (ExpeditionXML != null)
				if (ExpeditionXML.Root.Attribute("Version") != null) ExpeditionsVersion = ExpeditionXML.Root.Attribute("Version").Value;
				else ExpeditionsVersion = "알 수 없음";
			else
				QuestsVersion = "없음";
			if (RemodelXml != null)
				if (RemodelXml.Root.Attribute("Version") != null) RemodelSlotsVersion = RemodelXml.Root.Attribute("Version").Value;
				else RemodelSlotsVersion = "알 수 없음";
			else
				RemodelSlotsVersion = "없음";
            if (EquipmentTypesXML != null)
                if (EquipmentTypesXML.Root.Attribute("Version") != null) EquipmentTypesVersion = EquipmentTypesXML.Root.Attribute("Version").Value;
                else EquipmentTypesVersion = "알 수 없음";
            else
                EquipmentTypesVersion = "없음";
		}

		private IEnumerable<XElement> GetTranslationList(TranslationType Type)
		{
			switch (Type)
			{
				case TranslationType.Ships:
					if (ShipsXML != null)
					{
						if (KanColleClient.Current.Updater.ShipsUpdate)
						{
							this.ShipsXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Ships.xml"));
							KanColleClient.Current.Updater.ShipsUpdate = false;
						}
						return ShipsXML.Descendants("Ship");
					}
					break;
				case TranslationType.ShipTypes:
					if (ShipTypesXML != null)
					{
						if (KanColleClient.Current.Updater.ShipTypesUpdate)
						{
							this.ShipTypesXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "ShipTypes.xml"));
							KanColleClient.Current.Updater.ShipTypesUpdate = false;
						}
						return ShipTypesXML.Descendants("Type");
					}
					break;
				case TranslationType.Equipment:
					if (EquipmentXML != null)
					{
						if (KanColleClient.Current.Updater.EquipUpdate)
						{
							this.EquipmentXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Equipment.xml"));
							KanColleClient.Current.Updater.EquipUpdate = false;
						}
						return EquipmentXML.Descendants("Item");
					}
					break;
                case TranslationType.EquipmentTypes:
                    if (EquipmentTypesXML != null)
                    {
                        if (KanColleClient.Current.Updater.EquipTypesUpdate)
                        {
                            this.EquipmentTypesXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "EquipmentTypes.xml"));
                            KanColleClient.Current.Updater.EquipTypesUpdate = false;
                        }
                        return EquipmentTypesXML.Descendants("Item");
                    }
                    break;
				case TranslationType.OperationMaps:
					if (OperationsXML != null)
					{
						if (KanColleClient.Current.Updater.OperationsUpdate)
						{
							this.OperationsXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Operations.xml"));
							KanColleClient.Current.Updater.OperationsUpdate = false;
						}
						return OperationsXML.Descendants("Map");
					}
					break;
				case TranslationType.OperationSortie:
					if (OperationsXML != null)
					{
						if (KanColleClient.Current.Updater.OperationsUpdate)
						{
							this.OperationsXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Operations.xml"));
							KanColleClient.Current.Updater.OperationsUpdate = false;
						}
						return OperationsXML.Descendants("Sortie");
					}
					break;
				case TranslationType.Quests:
				case TranslationType.QuestTitle:
				case TranslationType.QuestDetail:
					if (QuestsXML != null)
					{
						if (KanColleClient.Current.Updater.QuestUpdate)
						{
							this.QuestsXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Quests.xml"));
							KanColleClient.Current.Updater.QuestUpdate = false;
						}
						return QuestsXML.Descendants("Quest");
					}
					break;
				case TranslationType.Expeditions:
				case TranslationType.ExpeditionTitle:
				case TranslationType.ExpeditionDetail:
					if (ExpeditionXML != null)
					{
						if (KanColleClient.Current.Updater.ExpeditionUpdate)
						{
							this.ExpeditionXML = XDocument.Load(Path.Combine(MainFolder, "Translations", "Expeditions.xml"));
							KanColleClient.Current.Updater.ExpeditionUpdate = false;
						}
						return ExpeditionXML.Descendants("Expedition");
					}
					break;
			}
			return null;
		}
		public string GetExpeditionData(string ElementName, int ID)
		{
			IEnumerable<XElement> TranslationList = GetTranslationList(TranslationType.ExpeditionDetail);

			if (TranslationList == null || TranslationList.Count() < 1) return string.Empty;
			string IDElement = "ID";


			IEnumerable<XElement> FoundTranslation = TranslationList.Where(b => b.Element(IDElement).Value.Equals(ID.ToString()));//퀘스트 ID검색

			foreach (XElement el in FoundTranslation)
			{
				if (el.Element(ElementName) == null) return string.Empty;
				if (ID >= 0 && el.Element("ID") != null && Convert.ToInt32(el.Element("ID").Value) == ID)
					return el.Element(ElementName).Value;

			}

			return string.Empty;
		}
		public int GetExpeditionListCount()
		{
			IEnumerable<XElement> TranslationList = GetTranslationList(TranslationType.ExpeditionDetail);

			return TranslationList.Where(x => x.Element("TR-Name") != null).Where(x => x.Element("FlagLv") != null).Count();
		}
		public string GetTranslation(string JPString, TranslationType Type, bool IsLogReader, Object RawData = null, int ID = -1, bool IsRepeat = false)
		{
			if (!EnableTranslations)
				return JPString;
			if (IsRepeat)
				return JPString;

			try
			{
				IEnumerable<XElement> TranslationList = GetTranslationList(Type);

				if (TranslationList == null && RawData != null)
				{
					if (!IsLogReader) AddTranslation(RawData, Type);
					if (ID < 0 && !IsLogReader) return "[" + ID.ToString() + "] " + JPString;
					else return JPString;
				}

				string JPChildElement = "JP-Name";
				string TRChildElement = "TR-Name";
				string IDElement = "ID";

				if (Type == TranslationType.QuestDetail || Type == TranslationType.ExpeditionDetail)
				{
					JPChildElement = "JP-Detail";
					TRChildElement = "TR-Detail";
				}

				IEnumerable<XElement> OldFoundTranslation = TranslationList.Where(b => b.Element(JPChildElement).Value.Equals(JPString));//string 비교검색
				if (ID < 0)//일반적 번역
				{
					foreach (XElement el in OldFoundTranslation)
					{
						return el.Element(TRChildElement).Value;
					}
				}
				else//퀘스트,원정 번역
				{
					IEnumerable<XElement> FoundTranslation = TranslationList.Where(b => b.Element(IDElement).Value.Equals(ID.ToString()));//퀘스트 ID검색

					foreach (XElement el in FoundTranslation)
					{
#if DEBUG
						if (ID >= 0 && el.Element("ID") != null && Convert.ToInt32(el.Element("ID").Value) == ID)
							Debug.WriteLine(string.Format("Translation: {0,-20} {1,-20} {2}", JPString, el.Element(TRChildElement).Value, ID));
#endif
						if (ID >= 0 && el.Element("ID") != null && Convert.ToInt32(el.Element("ID").Value) == ID)
							return el.Element(TRChildElement).Value;

					}
					foreach (XElement el in OldFoundTranslation)
					{
						return el.Element(TRChildElement).Value;
					}
				}

#if DEBUG
				Debug.WriteLine(string.Format("Can't find Translation: {0,-20} {1}", JPString, ID));
#endif
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			if (RawData != null && !IsLogReader) AddTranslation(RawData, Type);
			if (ID < 0 && IsLogReader) return JPString;
			else
			{
				try
				{
					return GetTranslation(JPString, Type, false, RawData, ID, true);
				}
				catch
				{
					return "[" + ID.ToString() + "] " + JPString;
				}
			}
		}

		public void AddTranslation(Object RawData, TranslationType Type)
		{
			if (RawData == null || !EnableAddUntranslated)
				return;

			try
			{
				switch (Type)
				{
					case TranslationType.Ships:
						if (ShipsXML == null)
						{
							ShipsXML = new XDocument();
							ShipsXML.Add(new XElement("Ships"));
						}

						kcsapi_mst_ship ShipData = RawData as kcsapi_mst_ship;

						if (ShipData == null)
							return;

						ShipsXML.Root.Add(new XElement("Ship",
							new XElement("JP-Name", ShipData.api_name),
							new XElement("TR-Name", KanColleClient.Current.WebTranslator.RawTranslate(ShipData.api_name))
						));

						SaveXmls(0);
						break;

					case TranslationType.ShipTypes:
						if (ShipTypesXML == null)
						{
							ShipTypesXML = new XDocument();
							ShipTypesXML.Add(new XElement("ShipTypes"));
						}

						kcsapi_mst_stype TypeData = RawData as kcsapi_mst_stype;

						if (TypeData == null)
							return;

						ShipTypesXML.Root.Add(new XElement("Type",
							new XElement("ID", TypeData.api_id),
							new XElement("JP-Name", TypeData.api_name),
							new XElement("TR-Name", KanColleClient.Current.WebTranslator.RawTranslate(TypeData.api_name))
							));

						SaveXmls(1);
						break;

					case TranslationType.Equipment:
						if (EquipmentXML == null)
						{
							EquipmentXML = new XDocument();
							EquipmentXML.Add(new XElement("Equipment"));
						}

						kcsapi_mst_slotitem EqiupData = RawData as kcsapi_mst_slotitem;

						if (EqiupData == null)
							return;

						EquipmentXML.Root.Add(new XElement("Item",
							new XElement("JP-Name", EqiupData.api_name),
							new XElement("TR-Name", KanColleClient.Current.WebTranslator.RawTranslate(EqiupData.api_name))
							));

						SaveXmls(2);
						break;
                    case TranslationType.EquipmentTypes:
                        if (EquipmentTypesXML == null)
                        {
                            EquipmentTypesXML = new XDocument();
                            EquipmentTypesXML.Add(new XElement("EquipmentTypes"));
                        }

                        kcsapi_mst_slotitem_equiptype EqiupTypeData = RawData as kcsapi_mst_slotitem_equiptype;

                        if (EqiupTypeData == null)
                            return;

                        EquipmentTypesXML.Root.Add(new XElement("Item",
                            new XElement("JP-Name", EqiupTypeData.api_name),
                            new XElement("TR-Name", KanColleClient.Current.WebTranslator.RawTranslate(EqiupTypeData.api_name))
                            ));

                        SaveXmls(7);
                        break;

                    case TranslationType.OperationMaps:
					case TranslationType.OperationSortie:
						if (OperationsXML == null)
						{
							OperationsXML = new XDocument();
							OperationsXML.Add(new XElement("Operations"));
						}

						kcsapi_battleresult OperationsData = RawData as kcsapi_battleresult;

						if (OperationsData == null)
							return;

						if (Type == TranslationType.OperationMaps)
						{
							OperationsXML.Root.Add(new XElement("Map",
								new XElement("JP-Name", OperationsData.api_quest_name),
								new XElement("TR-Name", KanColleClient.Current.WebTranslator.RawTranslate(OperationsData.api_quest_name))
								));
						}
						else
						{
							OperationsXML.Root.Add(new XElement("Sortie",
								new XElement("JP-Name", OperationsData.api_enemy_info.api_deck_name),
								new XElement("TR-Name", KanColleClient.Current.WebTranslator.RawTranslate(OperationsData.api_enemy_info.api_deck_name))
								));
						}

						SaveXmls(3);
						break;

					case TranslationType.Quests:
					case TranslationType.QuestTitle:
					case TranslationType.QuestDetail:
						if (QuestsXML == null)
						{
							QuestsXML = new XDocument();
							QuestsXML.Add(new XElement("Quests"));
						}

						kcsapi_quest QuestData = RawData as kcsapi_quest;

						if (QuestData == null)
							return;

						IEnumerable<XElement> FoundTranslationID = QuestsXML.Descendants("Quest").Where(b => b.Element("ID").Value.Equals(QuestData.api_no));
						//이 부분에 의심이 가지만 확증은 없음
						if (Type == TranslationType.QuestTitle || Type == TranslationType.QuestDetail && FoundTranslationID != null && FoundTranslationID.Any())
						{
							foreach (XElement el in FoundTranslationID)
								el.Element("ID").Value = QuestData.api_no.ToString();

						}
						else
						{
							// The quest doesn't exist at all. Add it.
							QuestsXML.Root.Add(new XElement("Quest",
								new XElement("ID", QuestData.api_no),
								new XElement("JP-Name", QuestData.api_title),
								new XElement("TR-Name", "[" + QuestData.api_no.ToString() + "]" + KanColleClient.Current.WebTranslator.RawTranslate(QuestData.api_title)),
								new XElement("JP-Detail", QuestData.api_detail),
								new XElement("TR-Detail", "[" + QuestData.api_no.ToString() + "]" + KanColleClient.Current.WebTranslator.RawTranslate(QuestData.api_detail))
								));
						}

						SaveXmls(4);
						break;
					case TranslationType.Expeditions:
					case TranslationType.ExpeditionTitle:
					case TranslationType.ExpeditionDetail:
						if (ExpeditionXML == null)
						{
							ExpeditionXML = new XDocument();
							ExpeditionXML.Add(new XElement("Expeditions"));
						}

						kcsapi_mission MissionData = RawData as kcsapi_mission;

						if (MissionData == null)
							return;

						FoundTranslationID = ExpeditionXML.Descendants("Expedition").Where(b => b.Element("ID").Value.Equals(MissionData.api_id));
						if (Type == TranslationType.ExpeditionTitle || Type == TranslationType.ExpeditionDetail && FoundTranslationID != null && FoundTranslationID.Any())
						{
							foreach (XElement el in FoundTranslationID)
								el.Element("ID").Value = MissionData.api_id.ToString();

						}
						else
						{
							// The quest doesn't exist at all. Add it.
							ExpeditionXML.Root.Add(new XElement("Expedition",
							new XElement("ID", MissionData.api_id),
							new XElement("JP-Name", MissionData.api_name),
							new XElement("TR-Name", "[" + MissionData.api_id.ToString() + "]" + KanColleClient.Current.WebTranslator.RawTranslate(MissionData.api_name)),
							new XElement("JP-Detail", MissionData.api_details),
							new XElement("TR-Detail", "[" + MissionData.api_id.ToString() + "]" + KanColleClient.Current.WebTranslator.RawTranslate(MissionData.api_details))
							));
						}

						SaveXmls(5);
						break;
				}
			}
			catch { }
		}

	}
}