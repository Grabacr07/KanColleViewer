using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
	public class AkashiTimer : TimerNotifier
	{
		#region CurrentTime 변경 통지 프로퍼티
		private TimeSpan _CurrentTime;

		public TimeSpan CurrentTime
		{
			get { return _CurrentTime; }
			private set
			{
				if(_CurrentTime != value)
				{
					_CurrentTime = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public AkashiTimer()
		{
			CurrentTime = new TimeSpan();
		}

		public void Update(NameValueCollection request)
		{
			int fleetId = int.Parse(request["api_id"]);
			int shipIdx = int.Parse(request["api_ship_idx"]);
			int shipId = int.Parse(request["api_ship_id"]);

			bool akashiCheck = false;

			var fleets = KanColleClient.Current.Homeport.Organization.Fleets;
			if(fleets[fleetId].Ships[0].Info.Id == 182 || fleets[fleetId].Ships[0].Info.Id == 187)
			{
				akashiCheck = true;
			}

			if (shipIdx != -1 && akashiCheck == true)
				CurrentTime = TimeSpan.FromSeconds(0);
		}

		public void Reset()
		{
			if (CurrentTime.Minutes >= 20)
				CurrentTime = TimeSpan.FromSeconds(0);
		}

		protected override void Tick()
		{
			base.Tick();

			CurrentTime = CurrentTime.Add(TimeSpan.FromSeconds(1));
		}
	}
}
