using Grabacr07.KanColleWrapper.Models.Raw;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Grabacr07.KanColleWrapper.Models
{
	public class EventMapHPChecker
	{
		public bool EnableEventMapInfo { get; set; }
		public List<Maplists> Lists = new List<Maplists>();
		private StringBuilder SeaList= new StringBuilder();
		//public delegate void MapListEventHandler();

		//public event MapListEventHandler EventMapEnable;
		
		public EventMapHPChecker(KanColleProxy proxy)
		{
			proxy.api_get_member_mapinfo.TryParse<kcsapi_mapinfo[]>().Subscribe(x => this.EventMapList(x.Data));
		}

		private void EventMapList(kcsapi_mapinfo[] list)
		{
			if (!KanColleClient.Current.EventMapHPChecker.EnableEventMapInfo) return;
			int j = 1;

			for (int i = 0; i < list.Length; i++)
			{
				if (list[i].api_eventmap != null)
				{
					Maplists t = new Maplists { 
						MaxHp = list[i].api_eventmap.api_max_maphp,
						NowHp = list[i].api_eventmap.api_now_maphp,
						Num=j
					};
					this.Lists.Add(t);
					j++;
				}
			}
			if (this.Lists.Count >= 1)
			{
				for (int i = 0; i < Lists.Count; i++)
				{
					if ((double)Lists[i].NowHp / (double)Lists[i].MaxHp == 1) SeaList.Append("E-" + Lists[i].Num.ToString() + "해역 HP: 100%" + "\r");
					else if (((double)Lists[i].NowHp / (double)Lists[i].MaxHp) > 0) SeaList.Append("E-" + Lists[i].Num.ToString() + "해역 HP:" + Lists[i].NowHp.ToString() + "/" + Lists[i].MaxHp.ToString() + "\r");
				}
				if(SeaList.Length!=0)MessageBox.Show(SeaList.ToString(),"이벤트 해역정보");
				//if (SeaList.Length == 0) KanColleClient.Current.EventMapHPChecker.EnableEventMapInfo=false;
			}
			Lists.Clear();
			SeaList.Clear();
		}
	}
}
