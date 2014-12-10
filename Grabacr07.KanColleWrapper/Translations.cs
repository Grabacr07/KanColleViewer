using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Grabacr07.KanColleWrapper
{
	public class Translations : NotificationObject
	{
		private XDocument ShipsXML;
		private XDocument ShipTypesXML;
		private XDocument EquipmentXML;
		private XDocument OperationsXML;
		private XDocument QuestsXML;
		private XDocument ExpeditionXML;
		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";

		public bool EnableTranslations { get; set; }
		public bool EnableAddUntranslated { get; set; }

		#region EquipmentVersion 変更通知プロパティ

		private string _EquipmentVersion;

		public string EquipmentVersion
		{
			get { return _EquipmentVersion; }
			set
			{
				if (_EquipmentVersion != value)
				{
					_EquipmentVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region OperationsVersion 変更通知プロパティ

		private string _OperationsVersion;

		public string OperationsVersion
		{
			get { return _OperationsVersion; }
			set
			{
				if (_OperationsVersion != value)
				{
					_OperationsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region QuestsVersion 変更通知プロパティ

		private string _QuestsVersion;

		public string QuestsVersion
		{
			get { return _QuestsVersion; }
			set
			{
				if (_QuestsVersion != value)
				{
					_QuestsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ExpeditionsVersion 変更通知プロパティ

		private string _ExpeditionsVersion;

		public string ExpeditionsVersion
		{
			get { return _ExpeditionsVersion; }
			set
			{
				if (_ExpeditionsVersion != value)
				{
					_ExpeditionsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipsVersion 変更通知プロパティ

		private string _ShipsVersion;

		public string ShipsVersion
		{
			get { return _ShipsVersion; }
			set
			{
				if (_ShipsVersion != value)
				{
					_ShipsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipTypesVersion 変更通知プロパティ

		private string _ShipTypesVersion;

		public string ShipTypesVersion
		{
			get { return _ShipTypesVersion; }
			set
			{
				if (_ShipTypesVersion != value)
				{
					_ShipTypesVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		internal Translations()
		{
			try
			{
				if (File.Exists(MainFolder + "Translations\\Ships.xml")) ShipsXML = XDocument.Load(MainFolder + "Translations\\Ships.xml");
				if (File.Exists(MainFolder + "Translations\\ShipTypes.xml")) ShipTypesXML = XDocument.Load(MainFolder + "Translations\\ShipTypes.xml");
				if (File.Exists(MainFolder + "Translations\\Equipment.xml")) EquipmentXML = XDocument.Load(MainFolder + "Translations\\Equipment.xml");
				if (File.Exists(MainFolder + "Translations\\Operations.xml")) OperationsXML = XDocument.Load(MainFolder + "Translations\\Operations.xml");
				if (File.Exists(MainFolder + "Translations\\Quests.xml")) QuestsXML = XDocument.Load(MainFolder + "Translations\\Quests.xml");
				if (File.Exists(MainFolder + "Translations\\Expeditions.xml")) ExpeditionXML = XDocument.Load(MainFolder + "Translations\\Expeditions.xml");
					

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
				if (EquipmentXML.Root.Attribute("Version")!=null) EquipmentVersion = EquipmentXML.Root.Attribute("Version").Value;
					else EquipmentVersion="알 수 없음";
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
							this.ShipsXML = XDocument.Load(MainFolder + "Translations\\Ships.xml");
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
							this.ShipTypesXML = XDocument.Load(MainFolder + "Translations\\ShipTypes.xml");
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
							this.EquipmentXML = XDocument.Load(MainFolder + "Translations\\Equipment.xml");
							KanColleClient.Current.Updater.EquipUpdate = false;
						}
						return EquipmentXML.Descendants("Item");
					}
					break;
				case TranslationType.OperationMaps:
					if (OperationsXML != null)
					{
						if (KanColleClient.Current.Updater.OperationsUpdate)
						{
							this.OperationsXML = XDocument.Load(MainFolder + "Translations\\Operations.xml");
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
							this.OperationsXML = XDocument.Load(MainFolder + "Translations\\Operations.xml");
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
							this.QuestsXML = XDocument.Load(MainFolder + "Translations\\Quests.xml");
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
							this.ExpeditionXML = XDocument.Load(MainFolder + "Translations\\Expeditions.xml");
							KanColleClient.Current.Updater.ExpeditionUpdate = false;
						}
						return ExpeditionXML.Descendants("Expedition");
					}
					break;
			}
			return null;
		}
		public string GetQuestTranslation(string JPString, TranslationType Type, Object RawData, int ID)
		{
			if (!EnableTranslations)
				return JPString;

			try
			{
				IEnumerable<XElement> TranslationList = GetTranslationList(Type);

				if (TranslationList == null)
				{
					AddTranslation(RawData, Type);
					return "[" + ID.ToString() + "] " + JPString;
				}

				string JPChildElement = "JP-Name";
				string TRChildElement = "TR-Name";
				string IDElement = "ID";

				if (Type == TranslationType.QuestDetail || Type == TranslationType.ExpeditionDetail)
				{
					JPChildElement = "JP-Detail";
					TRChildElement = "TR-Detail";
				}

				IEnumerable<XElement> FoundTranslation = TranslationList.Where(b => b.Element(IDElement).Value.Equals(ID.ToString()));
				IEnumerable<XElement> OldFoundTranslation = TranslationList.Where(b => b.Element(JPChildElement).Value.Equals(JPString));

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
#if DEBUG
				Debug.WriteLine(string.Format("Can't find Translation: {0,-20} {1}", JPString, ID));
#endif
			}
			catch { }

			AddTranslation(RawData, Type);

			return "["+ID.ToString()+"] "+ JPString;
		}
		public string GetTranslation(string JPString, TranslationType Type, Object RawData, int ID = -1)
		{
			if (!EnableTranslations)
				return JPString;

			try
			{
				IEnumerable<XElement> TranslationList = GetTranslationList(Type);

				if (TranslationList == null)
				{
					AddTranslation(RawData, Type);
					return JPString;
				}

				string JPChildElement = "JP-Name";
				string TRChildElement = "TR-Name";

				if (Type == TranslationType.QuestDetail)
				{
					JPChildElement = "JP-Detail";
					TRChildElement = "TR-Detail";
				}

				IEnumerable<XElement> FoundTranslation = TranslationList.Where(b => b.Element(JPChildElement).Value.Equals(JPString));

				foreach (XElement el in FoundTranslation)
				{
#if DEBUG
					if (ID >= 0 && el.Element("ID") != null && Convert.ToInt32(el.Element("ID").Value) == ID)
						Debug.WriteLine(string.Format("Translation: {0,-20} {1,-20} {2}", JPString, el.Element(TRChildElement).Value, ID));
#endif
					if (ID >= 0 && el.Element("ID") != null && Convert.ToInt32(el.Element("ID").Value) == ID)
						return el.Element(TRChildElement).Value;
					else if (ID < 0)
						return el.Element(TRChildElement).Value;

				}
#if DEBUG
				Debug.WriteLine(string.Format("Can't find Translation: {0,-20} {1}", JPString, ID));
#endif
			}
			catch { }

			AddTranslation(RawData, Type);

			return JPString;
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
							new XElement("TR-Name", ShipData.api_name)
						));

						ShipsXML.Save(MainFolder + "Translations\\Ships.xml");
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
							new XElement("TR-Name", TypeData.api_name)
							));

						ShipTypesXML.Save(MainFolder + "Translations\\ShipTypes.xml");
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
							new XElement("TR-Name", EqiupData.api_name)
							));

						EquipmentXML.Save(MainFolder + "Translations\\Equipment.xml");
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
								new XElement("TR-Name", OperationsData.api_quest_name)
								));
						}
						else
						{
							OperationsXML.Root.Add(new XElement("Sortie",
								new XElement("JP-Name", OperationsData.api_enemy_info.api_deck_name),
								new XElement("TR-Name", OperationsData.api_enemy_info.api_deck_name)
								));
						}

						OperationsXML.Save(MainFolder + "Translations\\Operations.xml");
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
								new XElement("TR-Name", "["+QuestData.api_no.ToString()+"]"+ QuestData.api_title),
								new XElement("JP-Detail", QuestData.api_detail),
								new XElement("TR-Detail", "[" + QuestData.api_no.ToString() + "]" + QuestData.api_detail)
								));
						}

						QuestsXML.Save(MainFolder + "Translations\\Quests.xml");
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
								new XElement("TR-Name", "[" + MissionData.api_id.ToString() + "]" + MissionData.api_name),
								new XElement("JP-Detail", MissionData.api_details),
								new XElement("TR-Detail", "[" + MissionData.api_id.ToString() + "]" + MissionData.api_details)
								));
						}

						ExpeditionXML.Save(MainFolder + "Translations\\Expeditions.xml");
						break;
				}
			}
			catch { }
		}

	}
}