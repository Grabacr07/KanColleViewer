using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Models;
using Livet;
using Livet.EventListeners;
using System.Text;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class AdmiralViewModel : ViewModel
	{
		#region Model 変更通知プロパティ

		public Admiral Model => KanColleClient.Current.Homeport.Admiral;

		#endregion

		#region ToolTip 변경 통지 프로퍼티
		private string _ToolTip;

		public string ToolTip
		{
			get { return this._ToolTip; }
			set
			{
				if (this._ToolTip == value) return;
				this._ToolTip = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region ServerName 변경 통지 프로퍼티
		private string _ServerName;
		public string ServerName
		{
			get { return this._ServerName; }
			set
			{
				if (this._ServerName != value)
				{
					this._ServerName = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public static HQRecord Record;

		public AdmiralViewModel()
		{
			ServerName = "???";

			Record = new HQRecord();
			Record.Load();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport)
			{
				{ nameof(Homeport.Admiral), (sender, args) => this.Update() },
				{ nameof(Homeport.Admiral), (sender, args) => Record.Updated() },
			});

			var servers = new Dictionary<string, string>
			{
				{ "125.6.184.15", "(구) 구레 진수부" },
				{ "203.104.209.71", "요코스카 진수부" },
				{ "203.104.209.87", "구레 진수부" },
				{ "125.6.184.16", "사세보 진수부" },
				{ "125.6.187.205", "마이즈루 진수부" },
				{ "125.6.187.229", "오오미나토 경비부" },
				{ "125.6.187.253", "트럭 정박지" },
				{ "125.6.188.25", "링가 정박지" },
				{ "203.104.248.135", "라바울 기지" },
				{ "125.6.189.7", "쇼트랜드 정박지" },
				{ "125.6.189.39", "부인 기지" },
				{ "125.6.189.71", "타위타위 정박지" },
				{ "125.6.189.103", "팔라우 정박지" },
				{ "125.6.189.135", "브루나이 정박지" },
				{ "125.6.189.167", "카사트카만(히토캇푸만) 정박지" },
				{ "125.6.189.215", "파라무쉬르 정박지" },
				{ "125.6.189.247", "스쿠모만 정박지" },
				{ "203.104.209.23", "가노야 기지" },
				{ "203.104.209.39", "이와가와 기지" },
				{ "203.104.209.55", "사이키만 정박지" },
				{ "203.104.209.102", "하시라지마 정박지" }
			};

			KanColleClient.Current.Proxy.api_port
				.Subscribe(x =>
				{
					var server = x.Request.Headers.Host;
					if (servers.ContainsKey(server))
						this.ServerName = servers[server];
					else
						this.ServerName = server;
				});
		}

		private void Update()
		{
			StringBuilder tooltip = new StringBuilder();
			var res = Record.GetRecordPrevious();
			if (res != null)
			{
				int diff = Model.Experience - res.HQExp;
				tooltip.AppendFormat("이번: +{0} exp. / 전과 {1:n2}\r\n", diff, diff * 7 / 10000.0);
			}
			res = Record.GetRecordDay();
			if (res != null)
			{
				int diff = Model.Experience - res.HQExp;
				tooltip.AppendFormat("오늘: +{0} exp. / 전과 {1:n2}\r\n", diff, diff * 7 / 10000.0);
			}
			res = Record.GetRecordMonth();
			if (res != null)
			{
				int diff = Model.Experience - res.HQExp;
				tooltip.AppendFormat("이달: +{0} exp. / 전과 {1:n2}", diff, diff * 7 / 10000.0);
			}
			ToolTip = tooltip.ToString();
			this.RaisePropertyChanged(nameof(this.Model));
		}
	}
}
