using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

		public static HQRecord Record;

		public AdmiralViewModel()
		{
			Record = new HQRecord();
			Record.Load();
			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport)
			{
				{ nameof(Homeport.Admiral), (sender, args) => this.Update() },
				{ nameof(Homeport.Admiral), (sender, args) => Record.Updated() },
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
