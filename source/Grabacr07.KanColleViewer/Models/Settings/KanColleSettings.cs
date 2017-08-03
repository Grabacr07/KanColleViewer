using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	/// <summary>
	/// 艦これの動作に関連する設定を表す静的プロパティを公開します。
	/// </summary>
	public class KanColleSettings : IKanColleClientSettings
	{
		/// <summary>
		/// 건조중인 칸무스의 이름을 스포일러할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> CanDisplayBuildingShipName { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// GPU 렌더링을 활성화할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> GPURenderEnable { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 번역이 존재하지 않을 때 구글 번역을 이용해서 자동으로 번역할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> AutoTranslateEnable { get; }
		= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 업데이트 알림을 활성화할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> EnableUpdateNotification { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 플레이어 시작할 때 번역 파일을 자동으로 업데이트 할 지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> EnableUpdateTransOnStart { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 번역을 활성화할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> EnableTranslations { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 번역이 되지 않은 텍스트가 있는 경우 해당 데이터를 xml에 추가 입력합니다
		/// </summary>
		public static SerializableProperty<bool> EnableAddUntranslated { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 색적 계산에 사용하는 공식을 설정합니다.
		/// </summary>
		public static SerializableProperty<string> ViewRangeCalcType { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer, new ViewRangeType4().Id);

		/// <summary>
		/// 색적 계산에 1함대를 포함할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> IsViewRangeCalcIncludeFirstFleet { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 색적 계산에 2함대를 포함할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> IsViewRangeCalcIncludeSecondFleet { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 칸코레 윈도우가 음소거인 경우 알림도 음소거할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> NotifyMuteOnMute { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 건조 완료시에 알림을 표시할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> NotifyBuildingCompleted { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 원정 완료시에 알림을 표시할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> NotifyExpeditionReturned { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 입거 완료시에 알림을 표시할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> NotifyRepairingCompleted { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 입거와 원정이 끝났다고 판단하는 기준 시간을 설정합니다. (초)
		/// </summary>
		public static SerializableProperty<int> NotificationShorteningTime { get; }
			= new SerializableProperty<int>(GetKey(), Providers.Viewer, 40);

		/// <summary>
		/// 함대의 컨디션이 출격 가능한 수준까지 회복되면 알림을 표시할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> NotifyFleetRejuvenated { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 함대가 출격 가능한 컨디션인지 판단할 때의 기준 컨디션 값을 설정합니다.
		/// </summary>
		public static SerializableProperty<int> ReSortieCondition { get; }
			= new SerializableProperty<int>(GetKey(), Providers.Viewer, 49);

		/// <summary>
		/// 화면에 표시하는 자원 첫번째의 설정입니다.
		/// </summary>
		public static SerializableProperty<string> DisplayMaterial1 { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer, nameof(Materials.InstantRepairMaterials));

		/// <summary>
		/// 화면에 표시하는 자원 두번째의 설정입니다.
		/// </summary>
		public static SerializableProperty<string> DisplayMaterial2 { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer, nameof(Materials.InstantBuildMaterials));

		/// <summary>
		/// 화면에 표시하는 자원 세번째의 설정입니다.
		/// </summary>
		public static SerializableProperty<string> DisplayMaterial3 { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer, nameof(Materials.Fuel));

		/// <summary>
		/// 화면에 표시하는 자원 네번째의 설정입니다.
		/// </summary>
		public static SerializableProperty<string> DisplayMaterial4 { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Viewer, nameof(Materials.Ammunition));

		/// <summary>
		/// 화면에 표시할 자원을 4개까지 확장하는지 여부에 대한 설정.
		/// </summary>
		public static SerializableProperty<bool> DisplayMaterialExtended { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 함재기의 슬롯수를 표시할지, 격추수를 표시할지 여부에 대한 설정.
		/// </summary>
		public static SerializableProperty<bool> ShowLostAirplane { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 함재기를 장착하지 않아도 슬롯수를 표시할지 여부에 대한 설정.
		/// </summary>
		public static SerializableProperty<bool> ShowAirplaneAlways { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 공작함이 기함일 때 출격가능으로 판정하지 않을지 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> CheckFlagshipIsNotRepairShip { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// "아카시 수리 타이머를 사용" 설정의 체크 여부를 가져옵니다.
		/// </summary>
		public static SerializableProperty<bool> UseRepairTimer { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 아카시 수리 타이머가 20분을 넘었을 때 알림을 표시합니다.
		/// </summary>
		public static SerializableProperty<bool> AkashiTwentyMinute { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 함대의 편성이 변경되었을 때, 그 함대를 자동으로 선택할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> AutoFleetSelectWhenShipsChanged { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false); 

		/// <summary>
		/// 함대가 출격/귀환했을 때, 그 함대를 자동으로 선택할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> AutoFleetSelectWhenSortie { get; } 
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 모항 정보에 큰 폰트를 사용할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> HomeportBigFont { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 임무 진행도 추적 사용 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> UseQuestTracker { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 임무 진행도 추적 사용 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<FlashQuality> FlashElementQuality { get; }
			= new SerializableProperty<FlashQuality>(GetKey(), Providers.Viewer, FlashQuality.High);

		/// <summary>
		/// 자동 메모리 최적화를 사용할지 여부를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> UseMemoryOptimize { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 자동 메모리 최적화의 주기를 설정합니다. (초)
		/// </summary>
		public static SerializableProperty<int> MemoryOptimizePeriod { get; }
			= new SerializableProperty<int>(GetKey(), Providers.Viewer, 60);

		/// <summary>
		/// 모항 정보의 제독 정보를 간단하게 표시할지를 설정합니다. (이름, 직급, 사령부 레벨만 표시, 나머지 정보는 툴팁에)
		/// </summary>
		public static SerializableProperty<bool> AdmiralSummaryView { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 모항 정보의 자원을 수치만 표시할지를 설정합니다. (드롭다운에는 표시됨)
		/// </summary>
		public static SerializableProperty<bool> MaterialValueOnly { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 원정의 기대자원량을 표시할지를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> ExpeditionExpectResult { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 원정 함대의 키라 상황을 표시합니다.
		/// </summary>
		public static SerializableProperty<bool> ExpeditionConditions { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 완료된 임무 갯수를 뱃지로 탭에 표시할지를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> ShowQuestBadge { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 완료된 원정 갯수를 뱃지로 탭에 표시할지를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> ShowExpeditionBadge { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 함대 탭에서 연합함대를 합쳐서 표시할지를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> MergeCombinedFleet { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 상태표시줄에서 함대 상태를 표시할지를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> StatusbarFleetStatus { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 함대 보급량 미리보기를 표시할지를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> UseSupplyPreview { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, true);

		/// <summary>
		/// 전체 탭이 아닌 일간, 주간 등의 탭에서도 갱신되도록 합니다.
		/// </summary>
		public static SerializableProperty<bool> QuestOnAllTabs { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);

		/// <summary>
		/// 모항 정보에 원정 진행바를 표시할지를 설정합니다.
		/// </summary>
		public static SerializableProperty<bool> AdmiralExpeditionBars { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Viewer, false);


		#region instance members

		public event PropertyChangedEventHandler PropertyChanged;

		public KanColleSettings()
		{
			NotificationShorteningTime.Subscribe(_ => this.RaisePropertyChanged(nameof(NotificationShorteningTime)));
			ReSortieCondition.Subscribe(_ => this.RaisePropertyChanged(nameof(ReSortieCondition)));
			ViewRangeCalcType.Subscribe(_ => this.RaisePropertyChanged(nameof(ViewRangeCalcType)));
			IsViewRangeCalcIncludeFirstFleet.Subscribe(_ => this.RaisePropertyChanged(nameof(IsViewRangeCalcIncludeFirstFleet)));
			IsViewRangeCalcIncludeSecondFleet.Subscribe(_ => this.RaisePropertyChanged(nameof(IsViewRangeCalcIncludeSecondFleet)));
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region IKanColleClientSettings members

		int IKanColleClientSettings.NotificationShorteningTime => NotificationShorteningTime.Value;

		int IKanColleClientSettings.ReSortieCondition => ReSortieCondition.Value;

		string IKanColleClientSettings.ViewRangeCalcType => ViewRangeCalcType.Value;

		bool IKanColleClientSettings.IsViewRangeCalcIncludeFirstFleet => IsViewRangeCalcIncludeFirstFleet.Value;

		bool IKanColleClientSettings.IsViewRangeCalcIncludeSecondFleet => IsViewRangeCalcIncludeSecondFleet.Value;

		bool IKanColleClientSettings.AutoTranslateEnable => AutoTranslateEnable.Value;

		bool IKanColleClientSettings.CheckFlagshipIsRepairShip => CheckFlagshipIsNotRepairShip.Value;

		bool IKanColleClientSettings.QuestOnAllTabs => QuestOnAllTabs.Value;

		#endregion


		private static string GetKey([CallerMemberName] string propertyName = "")
		{
			return nameof(KanColleSettings) + "." + propertyName;
		}
	}
}
