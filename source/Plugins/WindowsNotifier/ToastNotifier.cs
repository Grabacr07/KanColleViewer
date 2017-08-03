﻿using System;

namespace Grabacr07.KanColleViewer.Plugins
{
	internal class ToastNotifier : NotifierBase
	{
		public override bool IsSupported => Toast.IsSupported;
		private CustomSound sound = new CustomSound();

		protected override void InitializeCore() {}

		protected override void NotifyCore(string header, string body, Action activated, Action<Exception> failed)
		{
			var toast = new Toast(header, body);
			toast.Activated += activated;
			if (failed != null) toast.ToastFailed += () => failed(new Exception("Toast failed."));
			this.sound.SoundOutput(header, true);
			toast.Show();
		}
	}
}
