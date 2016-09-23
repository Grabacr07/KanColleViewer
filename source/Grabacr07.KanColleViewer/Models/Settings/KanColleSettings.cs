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
		/// 建造中の艦の名前を表示するかどうかを示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> CanDisplayBuildingShipName { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

		/// <summary>
		/// GPU렌더링을 활성화합니다
		/// </summary>
		public static SerializableProperty<bool> GPURenderEnable { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

		public static SerializableProperty<bool> AutoTranslateEnable { get; }
		= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

		/// <summary>
		/// 업데이트 알림을 활성화합니다
		/// </summary>
		public static SerializableProperty<bool> EnableUpdateNotification { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

		/// <summary>
		/// 
		/// </summary>
		public static SerializableProperty<bool> EnableUpdateTransOnStart { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

		/// <summary>
		/// 번역을 활성화합니다
		/// </summary>
		public static SerializableProperty<bool> EnableTranslations { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

		/// <summary>
		/// 번역이 되지 않은 텍스트가 있는 경우 해당 데이터를 xml에 추가 입력합니다
		/// </summary>
		public static SerializableProperty<bool> EnableAddUntranslated { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

		/// <summary>
		/// 索敵計算に使用するロジックを識別する文字列の設定値を取得します。
		/// </summary>
		public static SerializableProperty<string> ViewRangeCalcType { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Roaming, new ViewRangeType4().Id);

		/// <summary>
		/// 索敵計算に第1艦隊を含めるかどうかの設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> IsViewRangeCalcIncludeFirstFleet { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

		/// <summary>
		/// 索敵計算に第2艦隊を含めるかどうかの設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> IsViewRangeCalcIncludeSecondFleet { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

		/// <summary>
		/// 칸코레 윈도우가 음소거인 경우 알림도 음소거합니다.
		/// </summary>
		public static SerializableProperty<bool> NotifyMuteOnMute { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

		/// <summary>
		/// 建造完了時に通知するかどうかを示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> NotifyBuildingCompleted { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

		/// <summary>
		/// 遠征帰投時に通知するかどうかを示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> NotifyExpeditionReturned { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

		/// <summary>
		/// 入渠完了時に通知するかどうかを示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> NotifyRepairingCompleted { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

		/// <summary>
		/// 入渠完了と遠征帰還の通知における、通知短縮時間 (秒) を表す設定値を取得します。
		/// </summary>
		public static SerializableProperty<int> NotificationShorteningTime { get; }
			= new SerializableProperty<int>(GetKey(), Providers.Roaming, 40);

		/// <summary>
		/// 艦隊のコンディション値が出撃可能となったときに通知するかどうかを示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> NotifyFleetRejuvenated { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

		/// <summary>
		/// 艦隊が再出撃可能と判断する基準となるコンディション値を表す設定値を取得します。
		/// </summary>
		public static SerializableProperty<int> ReSortieCondition { get; }
			= new SerializableProperty<int>(GetKey(), Providers.Roaming, 49);

		/// <summary>
		/// 画面に表示する資材 (1 つめ) を表す設定値を取得します。
		/// </summary>
		public static SerializableProperty<string> DisplayMaterial1 { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Roaming, nameof(Materials.InstantRepairMaterials));

		/// <summary>
		/// 画面に表示する資材 (2 つめ) を表す設定値を取得します。
		/// </summary>
		public static SerializableProperty<string> DisplayMaterial2 { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Roaming, nameof(Materials.InstantBuildMaterials));

        /// <summary>
        /// 画面に表示する資材 (3 つめ) を表す設定値を取得します。
        /// </summary>
        public static SerializableProperty<string> DisplayMaterial3 { get; }
            = new SerializableProperty<string>(GetKey(), Providers.Roaming, nameof(Materials.Fuel));

        /// <summary>
        /// 画面に表示する資材 (4 つめ) を表す設定値を取得します。
        /// </summary>
        public static SerializableProperty<string> DisplayMaterial4 { get; }
            = new SerializableProperty<string>(GetKey(), Providers.Roaming, nameof(Materials.Ammunition));

        /// <summary>
        /// 画面に表示する資材 (4 つめ) を表す設定値を取得します。
        /// </summary>
        public static SerializableProperty<bool> DisplayMaterialExtended { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

        /// <summary>
        /// 艦隊ステータスにおいて、旗艦が工作艦でないことを確認するかどうかを表す設定値を取得します。
        /// </summary>
        public static SerializableProperty<bool> CheckFlagshipIsNotRepairShip { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

		/// <summary>
		/// "아카시 수리 타이머를 사용" 설정의 체크 여부를 가져옵니다.
		/// </summary>
		public static SerializableProperty<bool> UseRepairTimer { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

        /// <summary>
        /// 아카시 수리 타이머가 20분을 넘었을 때 알림을 표시합니다.
        /// </summary>
        public static SerializableProperty<bool> AkashiTwentyMinute { get; }
            = new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

        /// <summary>
        /// 艦隊の編成が変更されたときに、その艦隊を自動的に選択状態にするかどうかを表す設定値を取得します。
        /// </summary>
        public static SerializableProperty<bool> AutoFleetSelectWhenShipsChanged { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false); 

		/// <summary>
		/// 艦隊が出撃・帰投したときに、その艦隊を自動的に選択状態にするかどうかを表す設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> AutoFleetSelectWhenSortie { get; } 
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, true);

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

		#endregion


		private static string GetKey([CallerMemberName] string propertyName = "")
		{
			return nameof(KanColleSettings) + "." + propertyName;
		}
	}
}
