using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Grabacr07.KanColleViewer.Win32
{
	[ComImport(), Guid("0000010d-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IViewObject
	{
		[PreserveSig]
		int Draw(
			[In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect,
			int lindex,
			IntPtr pvAspect,
			[In] DVTARGETDEVICE ptd,
			IntPtr hdcTargetDev,
			IntPtr hdcDraw,
			[In] RECT lprcBounds,
			[In] RECT lprcWBounds,
			IntPtr pfnContinue,
			[In] IntPtr dwContinue);
	}
}
