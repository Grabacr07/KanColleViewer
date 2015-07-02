using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vannatech.CoreAudio;
using Vannatech.CoreAudio.Interfaces;
using Livet;
using Vannatech.CoreAudio.Constants;
using Vannatech.CoreAudio.Enumerations;
using Vannatech.CoreAudio.Externals;

namespace Grabacr07.KanColleViewer.Models
{
	public class Volume : NotificationObject, IAudioSessionEvents
	{
		private ISimpleAudioVolume simpleAudioVolume;
		private IAudioSessionControl sessionControl;

		#region IsMute 変更通知プロパティ

		private bool _IsMute;

		public bool IsMute
		{
			get { return this._IsMute; }
			private set
			{
				if (this._IsMute != value)
				{
					this._IsMute = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		
		public static Volume GetInstance()
		{
			var volume = new Volume();

			var deviceEnumeratorType = Type.GetTypeFromCLSID(new Guid(ComCLSIDs.MMDeviceEnumeratorCLSID));
			var devenum = (IMMDeviceEnumerator)Activator.CreateInstance(deviceEnumeratorType);

			IMMDevice device;
			IsHResultOk(devenum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out device));

			object objSessionManager;
			IsHResultOk(device.Activate(new Guid(ComIIDs.IAudioSessionManager2IID), (uint)CLSCTX.CLSCTX_INPROC_SERVER, IntPtr.Zero, out objSessionManager));
			var sessionManager = objSessionManager as IAudioSessionManager2;
			if (sessionManager == null)
				throw new Exception("Session is not found.");

			IAudioSessionEnumerator sessions;
			IsHResultOk(sessionManager.GetSessionEnumerator(out sessions));

			//sessionIDは空にするとデフォルトセッションが取れるらしい
			ISimpleAudioVolume simpleAudioVolume;
			IsHResultOk(sessionManager.GetSimpleAudioVolume(Guid.Empty, 0, out simpleAudioVolume));
			volume.simpleAudioVolume = simpleAudioVolume;

			IsHResultOk(simpleAudioVolume.GetMute(out volume._IsMute));

			//sessionControlのインスタンスは取っておかないと通知来なくなる
			IsHResultOk(sessionManager.GetAudioSessionControl(Guid.Empty, 0, out volume.sessionControl));
			IsHResultOk(volume.sessionControl.RegisterAudioSessionNotification(volume));

			return volume;
		}

		public void ToggleMute()
		{
			var newValue = !this.IsMute;
			IsHResultOk(this.simpleAudioVolume.SetMute(newValue, Guid.NewGuid()));
			bool resultValue;
			IsHResultOk(this.simpleAudioVolume.GetMute(out resultValue));
			this.IsMute = resultValue;
		}
		
		private static void IsHResultOk(int hResult)
		{
			if (hResult != 0) throw new Exception("Session is not found.");
		}

		#region IAudioSessionEvents

		int IAudioSessionEvents.OnDisplayNameChanged(string displayName, ref Guid eventContext)
		{
			Debug.WriteLine(nameof(IAudioSessionEvents.OnDisplayNameChanged));
			return 0;
		}

		int IAudioSessionEvents.OnIconPathChanged(string iconPath, ref Guid eventContext)
		{
			Debug.WriteLine(nameof(IAudioSessionEvents.OnIconPathChanged));
			return 0;
		}

		int IAudioSessionEvents.OnSimpleVolumeChanged(float volume, bool isMuted, ref Guid eventContext)
		{
			Debug.WriteLine(nameof(IAudioSessionEvents.OnSimpleVolumeChanged));
			this.IsMute = isMuted;
			return 0;
		}

		int IAudioSessionEvents.OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex, ref Guid eventContext)
		{
			Debug.WriteLine(nameof(IAudioSessionEvents.OnChannelVolumeChanged));
			return 0;
		}

		int IAudioSessionEvents.OnGroupingParamChanged(ref Guid groupingId, ref Guid eventContext)
		{
			Debug.WriteLine(nameof(IAudioSessionEvents.OnGroupingParamChanged));
			return 0;
		}

		int IAudioSessionEvents.OnStateChanged(AudioSessionState state)
		{
			Debug.WriteLine(nameof(IAudioSessionEvents.OnStateChanged));
			return 0;
		}

		int IAudioSessionEvents.OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
		{
			Debug.WriteLine(nameof(IAudioSessionEvents.OnSessionDisconnected));
			return 0;
		}

		#endregion
	}
}
