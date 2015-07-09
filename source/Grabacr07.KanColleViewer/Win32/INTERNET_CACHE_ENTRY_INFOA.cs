using System;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Grabacr07.KanColleViewer.Win32
{
	// ReSharper disable InconsistentNaming
	[StructLayout(LayoutKind.Explicit, Size = 80)]
	internal struct INTERNET_CACHE_ENTRY_INFOA
	{
		[FieldOffset(0)]
		public uint dwStructSize;
		[FieldOffset(4)]
		public IntPtr lpszSourceUrlName;
		[FieldOffset(8)]
		public IntPtr lpszLocalFileName;
		[FieldOffset(12)]
		public uint CacheEntryType;
		[FieldOffset(16)]
		public uint dwUseCount;
		[FieldOffset(20)]
		public uint dwHitRate;
		[FieldOffset(24)]
		public uint dwSizeLow;
		[FieldOffset(28)]
		public uint dwSizeHigh;
		[FieldOffset(32)]
		public FILETIME LastModifiedTime;
		[FieldOffset(40)]
		public FILETIME ExpireTime;
		[FieldOffset(48)]
		public FILETIME LastAccessTime;
		[FieldOffset(56)]
		public FILETIME LastSyncTime;
		[FieldOffset(64)]
		public IntPtr lpHeaderInfo;
		[FieldOffset(68)]
		public uint dwHeaderInfoSize;
		[FieldOffset(72)]
		public IntPtr lpszFileExtension;
		[FieldOffset(76)]
		public uint dwReserved;
		[FieldOffset(76)]
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
