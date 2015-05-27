using System;
using BattleInfoPlugin.Models;
using BattleInfoPlugin.Models.Notifiers;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;

namespace BattleInfoPlugin.ViewModels
{
    public class ToolViewModel : ViewModel
    {
        private readonly BattleEndNotifier notifier;

        public BattleData Data { get; set; }

        public string UpdatedTime
        {
            get
            {
                return this.Data != null && this.Data.UpdatedTime != default(DateTimeOffset)
                    ? this.Data.UpdatedTime.ToString("yyyy/MM/dd HH:mm:ss")
                    : "No Data";
            }
        }

		public string Cell
		{
			get
			{
				if (this.Data != null)
				{
					return this.Data.Cell.ToString();
				}
				else return "";
			}
		}
		public string RankResult
		{
			get
			{
				if (this.Data != null)
				{
					return this.Data.RankResult.ToString();
				}
				else return "없음";
			}
		}
        public string BattleSituation
        {
            get
            {
                return this.Data != null && this.Data.BattleSituation != Models.BattleSituation.없음
                    ? this.Data.BattleSituation.ToString()
                    : "";
            }
        }
        public string FriendAirSupremacy
        {
            get
            {
                return this.Data != null && this.Data.FriendAirSupremacy != AirSupremacy.항공전없음
                    ? this.Data.FriendAirSupremacy.ToString()
                    : "";
            }
        }


        #region FirstFleet変更通知プロパティ
        private FleetViewModel _FirstFleet;

        public FleetViewModel FirstFleet
        {
            get
            { return this._FirstFleet; }
            set
            { 
                if (this._FirstFleet == value)
                    return;
                this._FirstFleet = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region SecondFleet変更通知プロパティ
        private FleetViewModel _SecondFleet;

        public FleetViewModel SecondFleet
        {
            get
            { return this._SecondFleet; }
            set
            { 
                if (this._SecondFleet == value)
                    return;
                this._SecondFleet = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region Enemies変更通知プロパティ
        private FleetViewModel _Enemies;

        public FleetViewModel Enemies
        {
            get
            { return this._Enemies; }
            set
            { 
                if (this._Enemies == value)
                    return;
                this._Enemies = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region IsNotifierEnabled変更通知プロパティ
        // ここ以外で変更しないのでModel変更通知は受け取らない雑対応
        public bool IsNotifierEnabled
        {
            get
            { return this.notifier.IsEnabled; }
            set
            {
                if (this.notifier.IsEnabled == value)
                    return;
                this.notifier.IsEnabled = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        public ToolViewModel(BattleData data, BattleEndNotifier notifier)
        {
            this.FirstFleet = new FleetViewModel("아군함대");
            this.SecondFleet = new FleetViewModel("호위함대");
            this.Enemies = new FleetViewModel("적함대");

            this.notifier = notifier;

            this.Data = data;

            this.CompositeDisposable.Add(new PropertyChangedEventListener(this.Data)
            {
                {
                    () => this.Data.UpdatedTime,
                    (_, __) => this.RaisePropertyChanged(() => this.UpdatedTime)
                },
				{
					()=>this.Data.Cell,
					(_,__)=>this.RaisePropertyChanged(()=>this.Cell)
				},
				{
					()=>this.Data.RankResult,
					(_,__)=>this.RaisePropertyChanged(()=>this.RankResult)
				},
                {
                    () => this.Data.BattleSituation,
                    (_, __) => this.RaisePropertyChanged(() => this.BattleSituation)
                },
                {
                    () => this.Data.FriendAirSupremacy,
                    (_, __) => this.RaisePropertyChanged(() => this.FriendAirSupremacy)
                },
                {
                    () => this.Data.FirstFleet,
                    (_, __) => this.FirstFleet.Fleet = this.Data.FirstFleet
                },
                {
                    () => this.Data.SecondFleet,
                    (_, __) => this.SecondFleet.Fleet = this.Data.SecondFleet
                },
                {
                    () => this.Data.Enemies,
                    (_, __) => this.Enemies.Fleet = this.Data.Enemies
                },
            });
        }

        public void OpenEnemyWindow()
        {
            var message = new TransitionMessage("Show/EnemyWindow")
            {
                TransitionViewModel = new EnemyWindowViewModel(this.Data.GetMapEnemies(), this.Data.GetCellTypes())
            };
            this.Messenger.RaiseAsync(message);
        }
    }
}
