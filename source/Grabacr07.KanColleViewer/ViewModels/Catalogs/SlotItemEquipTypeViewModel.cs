﻿using Grabacr07.KanColleWrapper.Models;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class SlotItemEquipTypeViewModel : ViewModel
	{
		public Action SelectionChangedAction { get; set; }

		#region Id 変更通知プロパティ

		private SlotItemIconType _Id;
		public SlotItemIconType Type
		{
			get { return this._Id; }
			set
			{
				if (this._Id != value)
				{
					this._Id = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region DisplayName 変更通知プロパティ

		private string _DisplayName;
		public string DisplayName
		{
			get { return this._DisplayName; }
			set
			{
				if (this._DisplayName != value)
				{
					this._DisplayName = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsSelected 変更通知プロパティ

		private bool _IsSelected;
		public bool IsSelected
		{
			get { return this._IsSelected; }
			set
			{
				if (this._IsSelected != value)
				{
					this._IsSelected = value;
					this.RaisePropertyChanged();
					SelectionChangedAction?.Invoke();
				}
			}
		}

		#endregion

		public static Dictionary<SlotItemIconType, string> IconNameTable { get; }
		public static Dictionary<SlotItemIconType, SlotItemIconType> IconAliasNamable { get; }

		static SlotItemEquipTypeViewModel()
		{
			IconNameTable = new Dictionary<SlotItemIconType, string>
			{
				{ SlotItemIconType.Unknown, "*분류안됨" },
				{ SlotItemIconType.MainCanonLight, "소구경주포" },
				{ SlotItemIconType.MainCanonMedium, "중구경주포" },
				{ SlotItemIconType.MainCanonHeavy, "대구경주포" },
				{ SlotItemIconType.SecondaryCanon, "부포" },
				{ SlotItemIconType.Torpedo, "어뢰" },
				{ SlotItemIconType.Fighter, "함상전투기" },
				{ SlotItemIconType.DiveBomber, "함상폭격기" },
				{ SlotItemIconType.TorpedoBomber, "함상공격기" },
				{ SlotItemIconType.ReconPlane, "함상정찰기" },
				{ SlotItemIconType.ReconSeaplane, "수상기" },
				{ SlotItemIconType.Rader, "전탐" },
				{ SlotItemIconType.AAShell, "삼식탄" },
				{ SlotItemIconType.APShell, "철갑탄" },
				{ SlotItemIconType.DamageControl, "다메콘" },
				{ SlotItemIconType.AAGun, "기총" },
				{ SlotItemIconType.HighAngleGun, "고각포" },
				{ SlotItemIconType.ASW, "폭뢰" },
				{ SlotItemIconType.Soner, "소나" },
				{ SlotItemIconType.EngineImprovement, "기관부강화" },
				{ SlotItemIconType.LandingCraft, "상륙정" },
				{ SlotItemIconType.Autogyro, "오토자이로" },
				{ SlotItemIconType.ArtillerySpotter, "지휘연락기" },
				{ SlotItemIconType.AntiTorpedoBulge, "벌지" },
				{ SlotItemIconType.Searchlight, "탐조등" },
				{ SlotItemIconType.DrumCanister, "드럼통" },
				{ SlotItemIconType.Facility, "함정수리시설" },
				{ SlotItemIconType.Flare, "조명탄" },
				{ SlotItemIconType.FleetCommandFacility, "사령부시설" },
				{ SlotItemIconType.MaintenancePersonnel, "함재기정비원" },
				{ SlotItemIconType.AntiAircraftFireDirector, "고사장치" },
				{ SlotItemIconType.RocketLauncher, "로켓발사기" },
				{ SlotItemIconType.SurfaceShipPersonnel, "숙련견시원" },
				{ SlotItemIconType.FlyingBoat, "대형비행정" },
				{ SlotItemIconType.CombatRations, "전투식량" },
				{ SlotItemIconType.OffshoreResupply, "해상보급" },
				{ SlotItemIconType.AmphibiousLandingCraft, "내화정" },
				{ SlotItemIconType.LandBasedAttacker, "육상공격기" },
				{ SlotItemIconType.InterceptorFighter, "국지전투기" },
				{ SlotItemIconType.JetPowerededBomber1, "분식폭격전투기" },
				{ SlotItemIconType.JetPowerededBomber2, "분식폭격전투기" },
				{ SlotItemIconType.TransportEquipment, "운송자재" },
				{ SlotItemIconType.SubmarineEquipment, "잠수함장비" },
				{ SlotItemIconType.SeaplaneFighter, "수상전투기" },
				{ SlotItemIconType.LandBasedFighter, "육군전투기" },
			};
			IconAliasNamable = new Dictionary<SlotItemIconType, SlotItemIconType>
			{
				{ SlotItemIconType.JetPowerededBomber2, SlotItemIconType.JetPowerededBomber1 }
			};
		}

		public SlotItemEquipTypeViewModel(SlotItemIconType stype)
		{
			this.Type = stype;
			this.DisplayName = (IconNameTable?.ContainsKey(stype) ?? false)
				? IconNameTable[stype]
				: IconNameTable[SlotItemIconType.Unknown];
		}

		public void Set(bool selected)
		{
			this._IsSelected = selected;
			this.RaisePropertyChanged(nameof(this.IsSelected));
		}
	}
}
