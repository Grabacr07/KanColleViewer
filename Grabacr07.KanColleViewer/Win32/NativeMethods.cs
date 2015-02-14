
using System;
using System.Runtime.InteropServices;

namespace Grabacr07.KanColleViewer.Win32
{
	internal static class NativeMethods
	{
		[DllImport("Avrt.dll")]
		public static extern IntPtr AvSetMmThreadCharacteristics(string taskName, ref uint taskIndex);
	}
}
