// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CefSharp.Handler;
using CefSharp.Internals;
using CefSharp.Structs;
using Grabacr07.KanColleViewer.Models.Cef;
using MetroRadiance.Interop;
using PixelFormat = System.Windows.Media.PixelFormat;
using Size = System.Windows.Size;

namespace CefSharp.Wpf
{
	/// <summary>
	/// Example with Screenshot support - adapted from https://github.com/cefsharp/CefSharp/pull/462/
	/// </summary>
	public class ChromiumWebBrowserWithScreenshotSupport : ChromiumWebBrowser
	{
		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

		private static readonly PixelFormat PixelFormat = PixelFormats.Bgra32;
		private static int BytesPerPixel = PixelFormat.BitsPerPixel / 8;

		private volatile bool isTakingScreenshot = false;
		private Size? screenshotSize;
		private int oldFrameRate;
		private int ignoreFrames = 0;
		private TaskCompletionSource<InteropBitmap> screenshotTaskCompletionSource;
		private Dpi currentDpi;

		public ChromiumWebBrowserWithScreenshotSupport()
		{
			this.RequestHandler = new RequestHandler();
		}

		public Task<InteropBitmap> TakeScreenshot(Size screenshotSize, int? frameRate = 1, int? ignoreFrames = 0, TimeSpan? timeout = null)
		{
			if (this.screenshotTaskCompletionSource != null && this.screenshotTaskCompletionSource.Task.Status == TaskStatus.Running)
			{
				throw new Exception("Screenshot already in progress, you must wait for the previous screenshot to complete");
			}

			if (this.IsBrowserInitialized == false)
			{
				throw new Exception("Browser has not yet finished initializing or is being disposed");
			}

			if (this.IsLoading)
			{
				throw new Exception("Unable to take screenshot while browser is loading");
			}

			var browserHost = this.GetBrowser().GetHost();

			if (browserHost == null)
			{
				throw new Exception("IBrowserHost is null");
			}

			this.screenshotTaskCompletionSource = new TaskCompletionSource<InteropBitmap>(TaskCreationOptions.RunContinuationsAsynchronously);

			if (timeout.HasValue)
			{
				this.screenshotTaskCompletionSource = this.screenshotTaskCompletionSource.WithTimeout(timeout.Value);
			}

			if (frameRate.HasValue)
			{
				this.oldFrameRate = browserHost.WindowlessFrameRate;
				browserHost.WindowlessFrameRate = frameRate.Value;
			}

			this.screenshotSize = screenshotSize;
			this.isTakingScreenshot = true;
			this.ignoreFrames = ignoreFrames.GetValueOrDefault() < 0 ? 0 : ignoreFrames.GetValueOrDefault();
			//Resize the browser using the desired screenshot dimensions
			//The resulting bitmap will never be rendered to the screen
			browserHost.WasResized();

			this.currentDpi = this.GetSystemDpi() ?? Dpi.Default;

			return this.screenshotTaskCompletionSource.Task;
		}

		protected override Structs.Rect? GetViewRect()
		{
			if (this.isTakingScreenshot)
			{
				return new Structs.Rect(0, 0, (int)Math.Ceiling(this.screenshotSize.Value.Width), (int)Math.Ceiling(this.screenshotSize.Value.Height));
			}

			return base.GetViewRect();
		}

		protected override void OnPaint(bool isPopup, Structs.Rect dirtyRect, IntPtr buffer, int width, int height)
		{
			if (this.isTakingScreenshot)
			{
				//We ignore the first n number of frames
				if (this.ignoreFrames > 0)
				{
					this.ignoreFrames--;
					return;
				}

				//Wait until we have a frame that matches the updated size we requested
				//if (screenshotSize.HasValue && screenshotSize.Value.Width == width / currentDpi.ScaleX && screenshotSize.Value.Height == height / currentDpi.ScaleY)
				{
					var stride = width * BytesPerPixel;
					var numberOfBytes = stride * height;

					//Create out own memory mapped view for the screenshot and copy the buffer into it.
					//If we were going to create a lot of screenshots then it would be better to allocate a large buffer
					//and reuse it.
					var mappedFile = MemoryMappedFile.CreateNew(null, numberOfBytes, MemoryMappedFileAccess.ReadWrite);
					var viewAccessor = mappedFile.CreateViewAccessor();

					CopyMemory(viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle(), buffer, (uint)numberOfBytes);

					//Bitmaps need to be created on the UI thread
					this.Dispatcher.BeginInvoke((Action)(() =>
					{
						var backBuffer = mappedFile.SafeMemoryMappedFileHandle.DangerousGetHandle();
						//NOTE: Interopbitmap is not capable of supporting DPI scaling
						var bitmap = (InteropBitmap)Imaging.CreateBitmapSourceFromMemorySection(backBuffer,
							width, height, PixelFormats.Bgra32, stride, 0);
						this.screenshotTaskCompletionSource.TrySetResult(bitmap);

						this.isTakingScreenshot = false;
						var browserHost = this.GetBrowser().GetHost();
						//Return the framerate to the previous value
						browserHost.WindowlessFrameRate = this.oldFrameRate;
						//Let the browser know the size changes so normal rendering can continue
						browserHost.WasResized();

						if (viewAccessor != null)
						{
							viewAccessor.Dispose();
						}

						if (mappedFile != null)
						{
							mappedFile.Dispose();
						}
					}));
				}
			}
			else
			{
				base.OnPaint(isPopup, dirtyRect, buffer, width, height);
			}
		}
	}
}
