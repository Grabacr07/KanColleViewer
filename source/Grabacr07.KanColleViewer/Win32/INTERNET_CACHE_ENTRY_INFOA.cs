using System;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Grabacr07.KanColleViewer.Win32
{
	// ReSharper disable InconsistentNaming
	[StructLayout(LayoutKind.Sequential)]
	internal struct INTERNET_CACHE_ENTRY_INFOA
	{
		public uint dwStructSize;
		public IntPtr lpszSourceUrlName;
		public IntPtr lpszLocalFileName;
		public uint CacheEntryType;
		public uint dwUseCount;
		public uint dwHitRate;
		public uint dwSizeLow;
		public uint dwSizeHigh;
		public FILETIME LastModifiedTime;
		public FILETIME ExpireTime;
		public FILETIME LastAccessTime;
		public FILETIME LastSyncTime;
		public IntPtr lpHeaderInfo;
		public uint dwHeaderInfoSize;
		public IntPtr lpszFileExtension;
		public uint dwReserved;
		public uint dwExemptDelta;
	}

	internal enum WININETCACHEENTRYTYPE
	{
		ALL = 0x31003d,
		COOKIE_CACHE_ENTRY = 0x100000,
		EDITED_CACHE_ENTRY = 8,
		None = 0,
		NORMAL_CACHE_ENTRY = 1,
		SPARSE_CACHE_ENTRY = 0x10000,
		STICKY_CACHE_ENTRY = 4,
		TRACK_OFFLINE_CACHE_ENTRY = 0x10,
		TRACK_ONLINE_CACHE_ENTRY = 0x20,
		URLHISTORY_CACHE_ENTRY = 0x200000
	}

	// ReSharper restore InconsistentNaming
}
