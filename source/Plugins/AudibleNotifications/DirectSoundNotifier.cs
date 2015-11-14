using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NAudio.Wave;

namespace Grabacr07.KanColleViewer.Plugins
{
	internal class DirectSoundNotifier : AudibleNotifierBase
	{
		private string LookupLocation => AudibleNotifications.Settings.Location.Value;

		public override bool IsSupported => true;

		public List<string> Types => AudibleNotifications.Types;
		private string locationDefault => AudibleNotifications.LocationDefault;

		private Random rnd = new Random();

		protected override void InitializeCore()
		{
			Debug.WriteLine("AudibleNotifications: DirectSoundNotifier: Currently enumerated notification types: " + string.Join(", ", Types.ToArray()) + ".");
			// Do nothing for now
		}

		protected override void NotifyCore(string type)
		{
			var localType = type;

			// Check if we know this type; if not, force default
			if (!AudibleNotifications.Settings.TypeSettings.ContainsKey(localType)) localType = locationDefault;

			// Bail out if notifications for this type are disabled
			if (AudibleNotifications.Settings.TypeSettings.ContainsKey(localType) && !AudibleNotifications.Settings.TypeSettings[localType]) return;

			var media = GetRandomMedia(localType);

			if ((media == null) && !AudibleNotifications.IsWindows8OrGreater)
			{
				System.Media.SystemSounds.Beep.Play();
				return;
			}
			if ((media == null) && AudibleNotifications.IsWindows8OrGreater)
				return;

			var sound = new SoundOutput();

			sound.PlaySound(media);
		}

		private string GetRandomMedia(string type)
		{
			try
			{
				if (!Directory.Exists(GetLocation(type)))
				{
					// No directory → try in Default
					Debug.WriteLine("Could not find the directory for type " + type + ".");
					if (type != locationDefault) return GetRandomMedia(locationDefault);
					return null;
				}

				var fileList = Directory.GetFiles(GetLocation(type), "*.wav", SearchOption.AllDirectories)
					.Concat(Directory.GetFiles(GetLocation(type), "*.mp3", SearchOption.AllDirectories))
					.ToList();

				if (fileList.Count > 0) return fileList[rnd.Next(0, fileList.Count)];

				if ((fileList.Count == 0) && (type != locationDefault))
				{
					Debug.WriteLine("Could not find specific media for type " + type + ". Falling back to default media.");
					return GetRandomMedia(locationDefault);
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine("Exception while looking for media: " + ex.Message + ".");
			}
			return null;
		}

		private string GetLocation(string type)
		{
			return Path.Combine(LookupLocation, type);
		}

		private class SoundOutput
		{
			private BlockAlignReductionStream blockStream;
			private DirectSoundOut soundOut;

			private static float Volume => (float)AudibleNotifications.Settings.Volume.Value / 100;

			public void PlaySound(string filename)
			{
				if (!File.Exists(filename)) return;

				DisposeWave();

				try
				{
					WaveChannel32 pcm;

					switch (Path.GetExtension(filename)?.ToLower())
					{
						case ".wav":
							pcm = new WaveChannel32(new WaveFileReader(filename), Volume, 0);
							break;

						case ".mp3":
							pcm = new WaveChannel32(new Mp3FileReader(filename), Volume, 0);
							break;

						default:
							return;
					}

					pcm.PadWithZeroes = false;

					blockStream = new BlockAlignReductionStream(pcm);

					soundOut = new DirectSoundOut();
					soundOut.Init(blockStream);
					soundOut.PlaybackStopped += (sender, args) => DisposeWave();
					soundOut.Play();
				}
				catch (Exception ex)
				{
					Debug.WriteLine("DirectSound: playback exception occurred: " + ex.Message);
				}
			}

			private void DisposeWave()
			{
				try
				{
					if (soundOut != null)
					{
						if (soundOut.PlaybackState == PlaybackState.Playing)
							soundOut.Stop();
						soundOut.Dispose();
						soundOut = null;
					}
					if (blockStream != null)
					{
						blockStream.Dispose();
						blockStream = null;
					}
				}
				catch { }
			}
		}
	}
}