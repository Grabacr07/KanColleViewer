using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleViewer.Models
{
	public class BrowserZoomFactor : NotificationObject, IZoomFactor
	{
		private const double neutral = 1.0;

		private static readonly double?[] zoomTable =
		{
			0.25, 0.50, 0.75,
			1.00, 1.25, 1.50, 1.75,
			2.00, 2.50,
			3.00,
			4.00,
		};

		private double[] supportedValuesCache;

		public double[] SupportedValues
		{
			get { return this.supportedValuesCache ?? (this.supportedValuesCache = zoomTable.Where(x => x.HasValue).Select(x => x.Value).ToArray()); }
		}

		#region Current 変更通知プロパティ

		private double _Current;

		public double Current
		{
			get { return this._Current; }
			set
			{
				this._Current = value;
				this.CurrentParcentage = (int)(value * 100);
				this.CanZoomDown = (zoomTable.FirstOrDefault() ?? neutral) < value;
				this.CanZoomUp = value < (zoomTable.LastOrDefault() ?? neutral);
				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region CurrentParcentage 変更通知プロパティ

		private int _CurrentParcentage;

		public int CurrentParcentage
		{
			get { return this._CurrentParcentage; }
			private set
			{
				if (this._CurrentParcentage != value)
				{
					this._CurrentParcentage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CanZoomUp 変更通知プロパティ

		private bool _CanZoomUp;

		public bool CanZoomUp
		{
			get { return this._CanZoomUp; }
			private set
			{
				if (this._CanZoomUp != value)
				{
					this._CanZoomUp = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CanZoomDown 変更通知プロパティ

		private bool _CanZoomDown;

		public bool CanZoomDown
		{
			get { return this._CanZoomDown; }
			private set
			{
				if (this._CanZoomDown != value)
				{
					this._CanZoomDown = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public void ZoomUp()
		{
			this.Current = zoomTable.FirstOrDefault(x => this.Current < x) ?? zoomTable.LastOrDefault() ?? neutral;
		}

		public void ZoomDown()
		{
			this.Current = zoomTable.LastOrDefault(x => x < this.Current) ?? zoomTable.FirstOrDefault() ?? neutral;
		}
	}
}
