using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class DropLogCatalogViewModel : WindowViewModel
    {
        #region LogCollection 変更通知プロパティ

        private LogItemCollection _logCollection;

        public LogItemCollection LogCollection
		{
            get { return this._logCollection; }
			set
			{
                if (this._logCollection != value)
				{
                    this._logCollection = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

        #region IsReloading 変更通知プロパティ

        private bool _IsReloading;

        public bool IsReloading
        {
            get { return this._IsReloading; }
            set
            {
                if (this._IsReloading != value)
                {
                    this._IsReloading = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region SelectorBuildItem 変更通知プロパティ

        private bool _selectorBuildItem;

        public bool SelectorBuildItem
        {
            get { return this._selectorBuildItem; }
            set
            {
                if (this._selectorBuildItem != value)
                {
                    this._selectorBuildItem = value;
                    if (value)
                        this.CurrentLogType = Logger.LogType.BuildItem;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region SelectorBuildShip 変更通知プロパティ

        private bool _selectorBuildShip;

        public bool SelectorBuildShip
        {
            get { return this._selectorBuildShip; }
            set
            {
                if (this._selectorBuildShip != value)
                {
                    this._selectorBuildShip = value;
                    if (value)
                        this.CurrentLogType = Logger.LogType.BuildShip;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region SelectorShipDrop 変更通知プロパティ

        private bool _selectorShipDrop = true;

        public bool SelectorShipDrop
        {
            get { return this._selectorShipDrop; }
            set
            {
                if (this._selectorShipDrop != value)
                {
                    this._selectorShipDrop = value;
                    if (value)
                        this.CurrentLogType = Logger.LogType.ShipDrop;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentLogType

	    private Logger.LogType _currentLogType;

        private Logger.LogType CurrentLogType
	    {
            get { return this._currentLogType; }
	        set
            {
                if (this._currentLogType != value)
                {
                    this._currentLogType = value;

                    _watcher.Filter = Logger.LogParameters[value].FileName;

                    this.Update();
                }
	        }
	    }

        #endregion

	    private FileSystemWatcher _watcher;

        public DropLogCatalogViewModel()
		{
            this.Title = "ログ一覧";

            this.SelectorShipDrop = true;
            this._currentLogType = Logger.LogType.ShipDrop;

			this.Update();

            try
            {

                this._watcher = new FileSystemWatcher(Logger.LogFolder)
                                {
                                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName,
                                    Filter = Logger.LogParameters[Logger.LogType.ShipDrop].FileName,
                                    EnableRaisingEvents = true
                                };

                this._watcher.Changed += (sender, e) => { this.Update(); };
                this._watcher.Created += (sender, e) => { this.Update(); };
                this._watcher.Deleted += (sender, e) => { this.Update(); };
                this._watcher.Renamed += (sender, e) => { this.Update(); };
            }
            catch (Exception)
            {
                if (this._watcher != null)
                    this._watcher.EnableRaisingEvents = false;
            }
		}

		public async void Update()
		{
		    this.IsReloading = true;
            this.LogCollection = await this.UpdateCore();
		    this.IsReloading = false;
		}

        private Task<LogItemCollection> UpdateCore()
        {
            LogItemCollection items = new LogItemCollection();

            return Task.Factory.StartNew(() =>
                                         {
                                             try
                                             {
                                                 string file = Path.Combine(Logger.LogFolder,
                                                                            Logger.LogParameters[this.CurrentLogType].FileName);
                                                 if (!File.Exists(file))
                                                     return items;

                                                 IEnumerable<string> lines = File.ReadLines(file);

                                                 lines.Take(1).First().Split(',').ForEach(col => items.Columns.Add(col));

                                                 lines.Skip(1).Reverse().Take(200).ForEach(line =>
                                                                                           {
                                                                                               string[] elements = line.Split(',');

                                                                                               if (elements.Length < 5)
                                                                                                   return;

                                                                                               items.Rows.Add(elements
                                                                                                                  .Take(items.Columns.Count)
                                                                                                                  .ToArray());
                                                                                           });

                                                 return items;
                                             }
                                             catch (Exception)
                                             {
                                                 return items;
                                             }
                                         });
	    }
	}
}
