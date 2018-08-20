using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace Grabacr07.KanColleViewer.Models.Cef
{
	public class PixiSettingsRewriteFilter : IResponseFilter
	{
		bool IResponseFilter.InitFilter() => true;

		FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
		{
			if (dataIn == null)
			{
				dataInRead = 0;
				dataOutWritten = 0;

				return FilterStatus.Done;
			}

			using (var reader = new StreamReader(dataIn))
			{
				// WebGL の canvas だと toDataURL() で真っ黒の画像しか引っ張ってこられないので、
				// preserveDrawingBuffer = true にする必要があります (既定値は false)。
				var text = reader.ReadToEnd().Replace("pixi.min.js\"></script>", "pixi.min.js\"></script><script>PIXI.settings.RENDER_OPTIONS.preserveDrawingBuffer = true;</script>");
				var buff = Encoding.UTF8.GetBytes(text);
				dataOut.Write(buff, 0, buff.Length);

				dataInRead = dataIn.Length;
				dataOutWritten = Math.Min(buff.Length, dataOut.Length);
			}

			return FilterStatus.Done;
		}

		public void Dispose()
		{
		}
	}
}
