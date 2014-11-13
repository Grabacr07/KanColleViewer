using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlyncLightSDK;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.Plugins
{
    [Export(typeof(IPlugin))]
    [ExportMetadata("Title", "KanColleBlync")]
    [ExportMetadata("Description", "Blynclight を光らせます。")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("Author", "@Grabacr07")]
    public class KanColleBlync : IPlugin
    {
        private readonly BlyncLightController controller = new BlyncLightController();
        private int deviceCounts;
        private BlyncDevice[] blyncDevices;

        private readonly LivetCompositeDisposable compositDisposable = new LivetCompositeDisposable();
        private FleetMonitor[] fleets;

        public bool CanBlync
        {
            get { return this.deviceCounts > 0; }
        }


        public KanColleBlync()
        {
            this.deviceCounts = this.controller.InitBlyncDevices();
            this.blyncDevices = Enumerable.Range(0, deviceCounts)
                .Select(x => new BlyncDevice { Index = x, IsSelected = true })
                .ToArray();
            // 選択の UI を作るまではとりあえず全デバイスを選択 (= 全デバイスを光らせる)

            this.compositDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current)
            {
                { nameof(Organization.Fleets), (sender, e) => this.ChangeFleets() },
                { nameof(KanColleClient.IsInSortie), (sender, e) => this.UpdateBlync() },
            });
            this.ChangeFleets();
        }


        public object GetSettingsView()
        {
            return null;
        }

        public void Dispose()
        {
            this.controller.CloseDevices(this.deviceCounts);
        }


        private void ChangeFleets()
        {
            if (this.fleets != null)
            {
                foreach (var fleet in this.fleets) fleet.Dispose();
            }

            this.fleets = KanColleClient.Current.Homeport.Organization.Fleets
                .Select(x => new FleetMonitor(x.Value, this.UpdateBlync))
                .ToArray();
        }

        private void UpdateBlync()
        {
            if (!this.CanBlync || this.fleets == null) return;

            // 出撃中に大破した艦がいたときの Blync
            if (this.fleets.Any(x => x.IsSortieBlyncRequested))
            {
                this.BlyncCore(BlyncLightController.Color.Red, BlyncLightController.Speed.Fast);
                return;
            }

            // 出撃中の Blync
            if (KanColleClient.Current.IsInSortie)
            {
                this.BlyncCore(BlyncLightController.Color.Green, null);
                return;
            }

            // 遠征帰投の Blync
            if (this.fleets.Any(x => x.IsExpeditionBlyncRequested))
            {
                this.BlyncCore(BlyncLightController.Color.Cyan, BlyncLightController.Speed.Slow);
                return;
            }
        }


        private void BlyncCore(BlyncLightController.Color color, BlyncLightController.Speed? speed)
        {
            if (speed.HasValue)
            {
                foreach (var device in this.blyncDevices)
                {
                    this.controller.Blink(color, speed.Value, device.Index);
                }
            }
            else
            {
                foreach (var device in this.blyncDevices)
                {
                    this.controller.Display(color, device.Index);
                }
            }
        }
    }

    public class BlyncDevice
    {
        public int Index { get; internal set; }
        public bool IsSelected { get; set; }
    }


    public class FleetMonitor : IDisposable
    {
        private readonly LivetCompositeDisposable compositDisposable = new LivetCompositeDisposable();
        private Fleet target;
        private Action updateBlync;

        public bool IsExpeditionBlyncRequested { get; private set; }
        public bool IsSortieBlyncRequested { get; private set; }


        public FleetMonitor(Fleet target, Action updateBlync)
        {
            this.target = target;
            this.updateBlync = updateBlync;

            this.compositDisposable.Add(new PropertyChangedEventListener(target.Expedition)
            {
                { nameof(Expedition.Remaining), (sender, e) => this.ChangeExpeditionRemaining() },
            });
            this.compositDisposable.Add(new PropertyChangedEventListener(target)
            {
                { nameof(Fleet.State), (sender, e) => this.ChangeCondition() },
                { nameof(Fleet.IsWounded), (sender, e) => this.ChangeCondition() },
            });
        }

        private void ChangeExpeditionRemaining()
        {
            if (Settings.Current.NotifyExpeditionReturned)
            {
                var remaining = target.Expedition.Remaining;
                if (remaining.HasValue && remaining.Value.TotalSeconds < KanColleClient.Current.Settings.NotificationShorteningTime)
                {
                    this.IsExpeditionBlyncRequested = true;
                    this.updateBlync();
                    return;
                }
            }

            if (this.IsExpeditionBlyncRequested)
            {
                this.IsExpeditionBlyncRequested = false;
                this.updateBlync();
            }
        }

        private void ChangeCondition()
        {
            if (this.target.State == FleetState.Sortie && this.target.IsWounded)
            {
                this.IsSortieBlyncRequested = true;
                this.updateBlync();
                return;
            }

            if (this.IsSortieBlyncRequested)
            {
                this.IsSortieBlyncRequested = false;
                this.updateBlync();
            }
        }

        public void Dispose()
        {
            this.compositDisposable.Dispose();
        }
    }


    public enum BlyncType
    {
        None,
        ReturnedExpedition,
        DamagedSortie,
    }
}
