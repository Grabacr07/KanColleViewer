using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace BattleInfoPlugin.Models
{
    public class ShipData : NotificationObject
    {

        #region Name変更通知プロパティ
        private string _Name;

        public string Name
        {
            get
            { return this._Name; }
            set
            { 
                if (this._Name == value)
                    return;
                this._Name = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region AdditionalName変更通知プロパティ
        private string _AdditionalName;

        public string AdditionalName
        {
            get
            { return this._AdditionalName; }
            set
            { 
                if (this._AdditionalName == value)
                    return;
                this._AdditionalName = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region TypeName変更通知プロパティ
        private string _TypeName;

        public string TypeName
        {
            get
            { return this._TypeName; }
            set
            { 
                if (this._TypeName == value)
                    return;
                this._TypeName = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Situation変更通知プロパティ
        private ShipSituation _Situation;

        public ShipSituation Situation
        {
            get
            { return _Situation; }
            set
            { 
                if (_Situation == value)
                    return;
                _Situation = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region MaxHP変更通知プロパティ
        private int _MaxHP;
        public int MaxHP
        {
            get { return this._MaxHP; }
            set
            {
                if (this._MaxHP == value)
                    return;
                this._MaxHP = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(() => this.HP);
            }
        }
        #endregion

        #region NowHP変更通知プロパティ
        private int _NowHP;
        public int NowHP
        {
            get { return this._NowHP; }
            set
            {
                if (this._NowHP == value)
                    return;
                this._NowHP = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(() => this.HP);
            }
        }
        #endregion

        #region Firepower 変更通知プロパティ

        private int _Firepower;

        /// <summary>
        /// 火力ステータス値を取得します。
        /// </summary>
        public int Firepower
        {
            get { return this._Firepower; }
            set
            {
                this._Firepower = value;
                this.RaisePropertyChanged();

            }
        }

        #endregion

        #region Torpedo 変更通知プロパティ

        private int _Torpedo;

        /// <summary>
        /// 雷装ステータス値を取得します。
        /// </summary>
        public int Torpedo
        {
            get { return this._Torpedo; }
            set
            {
                this._Torpedo = value;
                this.RaisePropertyChanged();

            }
        }

        #endregion

        #region AA 変更通知プロパティ

        private int _AA;

        /// <summary>
        /// 対空ステータス値を取得します。
        /// </summary>
        public int AA
        {
            get { return this._AA; }
            set
            {
                this._AA = value;
                this.RaisePropertyChanged();
            }

        }

        #endregion

        #region Armer 変更通知プロパティ

        private int _Armer;

        /// <summary>
        /// 装甲ステータス値を取得します。
        /// </summary>
        public int Armer
        {
            get { return this._Armer; }
            set
            {
                this._Armer = value;
                this.RaisePropertyChanged();

            }
        }

        #endregion

        #region Luck 変更通知プロパティ

        private int _Luck;

        /// <summary>
        /// 運のステータス値を取得します。
        /// </summary>
        public int Luck
        {
            get { return this._Luck; }
            set
            {
                this._Luck = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Slots変更通知プロパティ
        private IEnumerable<ShipSlotData> _Slots;

        public IEnumerable<ShipSlotData> Slots
        {
            get
            { return this._Slots; }
            set
            { 
                if (this._Slots == value)
                    return;
                this._Slots = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public int SlotsFirepower { get { return this.Slots.Sum(x => x.Firepower); } }
        public int SlotsTorpedo { get { return this.Slots.Sum(x => x.Torpedo); } }
        public int SlotsAA { get { return this.Slots.Sum(x => x.AA); } }
        public int SlotsArmer { get { return this.Slots.Sum(x => x.Armer); } }
        public int SlotsASW { get { return this.Slots.Sum(x => x.ASW); } }
        public int SlotsHit { get { return this.Slots.Sum(x => x.Hit); } }
        public int SlotsEvade { get { return this.Slots.Sum(x => x.Evade); } }

        public int SumFirepower { get { return this.Firepower + this.SlotsFirepower; } }
        public int SumTorpedo { get { return this.Torpedo + this.SlotsTorpedo; } }
        public int SumAA { get { return this.AA + this.SlotsAA; } }
        public int SumArmer { get { return this.Armer + this.SlotsArmer; } }

        public LimitedValue HP
        {
            get { return new LimitedValue(this.NowHP, this.MaxHP, 0); }
        }

        public AttackType DayAttackType
        {
            get
            {
                return this.HasScout() && this.Count(Type2.주포) == 2 && this.Count(Type2.철갑탄) == 1 ? AttackType.주주컷인
                    : this.HasScout() && this.Count(Type2.주포) == 1 && this.Count(Type2.부포) == 1 && this.Count(Type2.철갑탄) == 1 ? AttackType.주철컷인
                    : this.HasScout() && this.Count(Type2.주포) == 1 && this.Count(Type2.부포) == 1 && this.Count(Type2.전탐) == 1 ? AttackType.주전컷인
                    : this.HasScout() && this.Count(Type2.주포) >= 1 && this.Count(Type2.부포) >= 1 ? AttackType.주부컷인
                    : this.HasScout() && this.Count(Type2.주포) >= 2 ? AttackType.연격
                    : AttackType.통상;
            }
        }

        public AttackType NightAttackType
        {
            get
            {
                return this.Count(Type2.어뢰) >= 2 ? AttackType.뇌격컷인
                    : this.Count(Type2.주포) >= 3 ? AttackType.주주주컷인
                    : this.Count(Type2.주포) == 2 && this.Count(Type2.부포) >= 1 ? AttackType.주부컷인
                    : this.Count(Type2.주포) == 2 && this.Count(Type2.부포) == 0 && this.Count(Type2.어뢰) == 1 ? AttackType.주뢰컷인
                    : this.Count(Type2.주포) == 1 && this.Count(Type2.어뢰) == 1 ? AttackType.주뢰컷인
                    : this.Count(Type2.주포) == 2 && this.Count(Type2.부포) == 0 && this.Count(Type2.어뢰) == 0 ? AttackType.연격
                    : this.Count(Type2.주포) == 1 && this.Count(Type2.부포) >= 1 && this.Count(Type2.어뢰) == 0 ? AttackType.연격
                    : this.Count(Type2.주포) == 0 && this.Count(Type2.부포) >= 2 && this.Count(Type2.어뢰) <= 1 ? AttackType.연격
                    : AttackType.통상;
            }
        }

        public ShipData()
        {
            this._Name = "？？？";
            this._AdditionalName = "";
            this._TypeName = "？？？";
            this._Situation = ShipSituation.None;
            this._Slots = new ShipSlotData[0];
        }
    }

    public static class ShipDataExtensions
    {
        public static int Count(this ShipData data, Type2 type2)
        {
            return data.Slots.Count(x => x.Type2 == type2);
        }

        public static bool HasScout(this ShipData data)
        {
            return data.Slots
                .Where(x => x.Source.Type == SlotItemType.水上偵察機
                            || x.Source.Type == SlotItemType.水上爆撃機)
                .Any(x => 0 < x.Current);
        }
    }

    public class MembersShipData : ShipData
    {

        #region Source変更通知プロパティ
        private Ship _Source;

        public Ship Source
        {
            get
            { return this._Source; }
            set
            { 
                if (this._Source == value)
                    return;
                this._Source = value;
                this.RaisePropertyChanged();
                this.UpdateFromSource();
            }
        }
        #endregion


        public MembersShipData()
        {
        }

        public MembersShipData(Ship ship) : this()
        {
            this.Source = ship;
        }

        private void UpdateFromSource()
        {
            this.Name = this.Source.Info.Name;
            this.TypeName = this.Source.Info.ShipType.Name;
            this.Situation = this.Source.Situation;
            this.NowHP = this.Source.HP.Current;
            this.MaxHP = this.Source.HP.Maximum;
            this.Slots = this.Source.Slots
                .Where(s => s!= null)
                .Where(s => s.Equipped)
                .Select(s => new ShipSlotData(s)).ToArray();

            this.Firepower = this.Source.Firepower.Current;
            this.Torpedo = this.Source.Torpedo.Current;
            this.AA = this.Source.AA.Current;
            this.Armer = this.Source.Armer.Current;
            this.Luck = this.Source.Luck.Current;
        }
    }

    public class MastersShipData : ShipData
    {

        #region Source変更通知プロパティ
        private ShipInfo _Source;

        public ShipInfo Source
        {
            get
            { return this._Source; }
            set
            { 
                if (this._Source == value)
                    return;
                this._Source = value;
                this.RaisePropertyChanged();
                this.UpdateFromSource();
            }
        }
        #endregion


        public MastersShipData()
        {
        }

        public MastersShipData(ShipInfo info) : this()
        {
            this.Source = info;
        }

        private void UpdateFromSource()
        {
            this.Name = this.Source.Name;
            var isEnemyID = 500 < this.Source.Id && this.Source.Id < 901;
            var m = BattleInfoPlugin.RawStart2.api_mst_ship.Single(x => x.api_id == this.Source.Id);
            this.AdditionalName = isEnemyID ? m.api_yomi : "";
            this.TypeName = this.Source.ShipType.Name;
            this.NowHP = this.Source.HP;
            this.MaxHP = this.Source.HP;
            this.Firepower = this.Source.MaxFirepower;
            this.Torpedo = this.Source.MaxTorpedo;
            this.AA = this.Source.MaxAA;
            this.Armer = this.Source.MaxArmer;
            this.Luck = m.api_luck[0];
        }
    }
}
