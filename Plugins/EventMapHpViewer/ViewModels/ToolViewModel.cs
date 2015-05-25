using System.Linq;
using EventMapHpViewer.Models;
using Livet;
using Livet.EventListeners;

namespace EventMapHpViewer.ViewModels
{
    public class ToolViewModel : ViewModel
    {
        private readonly MapInfoProxy mapInfoProxy;

        public ToolViewModel(MapInfoProxy proxy)
        {
            this.mapInfoProxy = proxy;

            if (this.mapInfoProxy == null) return;

            this.CompositeDisposable.Add(new PropertyChangedEventListener(this.mapInfoProxy)
            {
                {
                    () => this.mapInfoProxy.Maps, (sender, args) =>
                    {
                        this.Maps = this.mapInfoProxy.Maps.MapList
                            .OrderBy(x => x.Id)
                            .Select(x => new MapViewModel(x))
                            .Where(x => !x.IsCleared)
                            .ToArray();
                        this.IsNoMap = !this.Maps.Any();
                    }
                }
            });
        }

        #region Maps変更通知プロパティ
        private MapViewModel[] _Maps;

        public MapViewModel[] Maps
        {
            get
            { return this._Maps; }
            set
            { 
                if (this._Maps == value)
                    return;
                this._Maps = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region IsNoMap変更通知プロパティ
        private bool _IsNoMap;

        public bool IsNoMap
        {
            get
            { return this._IsNoMap; }
            set
            { 
                if (this._IsNoMap == value)
                    return;
                this._IsNoMap = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

    }
}
