using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ExpeditionsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Properties.Resources.Expedition; }
			protected set { throw new NotImplementedException(); }
		}

		public FleetsViewModel Fleets { get; }

		public ExpeditionsViewModel(FleetsViewModel fleets)
		{
            this.Fleets = fleets;

            this.Fleets.CompositeDisposable.Add(new PropertyChangedEventListener(this.Fleets)
            {
                { nameof(this.Fleets.Fleets), (sender, args)=>
                {
                    foreach(var x in this.Fleets.Fleets)
                    {
                        var fleetId = x.Id - 1;
                        if(fleetId == 0) continue; // 1 함대

                        x.PropertyChanged += (s, e) =>
                            {
                                if(e.PropertyName == nameof(x.ExpeditionId))
                                {
                                    switch(fleetId) {
                                        case 1:
                                            Models.Settings.ExpeditionSettings.ExpeditionId1.Value = x.ExpeditionId;
                                            break;
                                        case 2:
                                            Models.Settings.ExpeditionSettings.ExpeditionId2.Value = x.ExpeditionId;
                                            break;
                                        case 3:
                                            Models.Settings.ExpeditionSettings.ExpeditionId3.Value = x.ExpeditionId;
                                            break;
                                    }
                                    Properties.Settings.Default.Save();
                                }
                            };

                        if (!x.Expedition.IsInExecution)
                        {
                            switch(fleetId) {
                                case 1:
                                    x.ExpeditionId = Models.Settings.ExpeditionSettings.ExpeditionId1;
                                    break;
                                case 2:
                                    x.ExpeditionId = Models.Settings.ExpeditionSettings.ExpeditionId2;
                                    break;
                                case 3:
                                    x.ExpeditionId = Models.Settings.ExpeditionSettings.ExpeditionId3;
                                    break;
                            }
                        }
                    }
                } }
            });
		}
	}
}
