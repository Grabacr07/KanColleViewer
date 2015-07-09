using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
	public interface IZoomFactor
	{
		double Current { get; set; }
		double[] SupportedValues { get; }

		int CurrentParcentage { get; }

		bool CanZoomUp { get; }
		bool CanZoomDown { get; }

		void ZoomUp();
		void ZoomDown();
	}
}
