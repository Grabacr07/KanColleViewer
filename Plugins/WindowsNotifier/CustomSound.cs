using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Settings = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.Plugins
{
	public class CustomSound
	{

		private BlockAlignReductionStream BlockStream = null;
		private DirectSoundOut SoundOut  = null;
		string Main_folder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Sounds";

		public void SoundOutput(string header, bool IsWin8)
		{
			/**
			 * 
			 * 출력할 소리가 wav인지 mp3인지 비프음인지 채크합니다.
			 * windows8 이상의 경우에는 비프음보다 윈도우8 기본 알림음이 더 알맞다고 생각하기에 IsWin8이 True면 아무 소리도 내보내지 않습니다.
			 * 
			**/
			DisposeWave();//알림이 동시에 여러개가 울릴 경우 소리가 겹치는 문제를 방지
			string Audiofile = FileCheck(header);
			if (string.IsNullOrEmpty(Audiofile) && !IsWin8)
			{
				System.Media.SystemSounds.Beep.Play();
			}
			else if(!string.IsNullOrEmpty(Audiofile))
			{
				float Volume = Settings.Current.CustomSoundVolume > 0 ? (float)Settings.Current.CustomSoundVolume / 100 : 0;
				if (Path.GetExtension(Audiofile).ToLower() == ".wav")//wav인지 채크
				{
					WaveStream pcm = new WaveChannel32(new WaveFileReader(Audiofile), Volume, 0);
					BlockStream = new BlockAlignReductionStream(pcm);
				}
				else if (Path.GetExtension(Audiofile).ToLower() == ".mp3")//mp3인 경우
				{
					WaveStream pcm = new WaveChannel32(new Mp3FileReader(Audiofile), Volume, 0);
					BlockStream = new BlockAlignReductionStream(pcm);
				}
				SoundOut = new DirectSoundOut();
				SoundOut.Init(BlockStream);
				SoundOut.Play();
			}
		}
		public string FileCheck(string header)
		{
			/**
			 * 
			 * 이 코드 안에서 팝업 타이틀 검증과 음소거에 대한 채크를 모두 수행합니다.
			 * 타이틀이 업데이트인경우, 파일이 존재하지 않는경우, 칸코레 뷰어가 음소거인 경우는 Empty을 return합니다
			 * 
			 * 그 외의 경우는 exe파일이 존재하는 기본 루트 폴더의 경로를 반환합니다.
			 * 파일은 MP3파일이 우선권을 가지며 그 다음으로 WAV파일, 해당 경로에 파일이 없는경우에는 루트 폴더에서 알림음을 찾습니다
			 * mp3인지 wav인지는 SoundOutput에서 찾습니다. string의 형태이고 파일명이 정해져있으므로 파일명을 기준으로 구별합니다.
			 * 
			**/
			string SelFolder = "";
			if (header == Resources.Expedition_NotificationMessage_Title) SelFolder = "\\expedition";//원정
			else if (header == Resources.Repairyard_NotificationMessage_Title) SelFolder = "\\repair";//수리
			else if (header == Resources.ReSortie_NotificationMessage_Title) SelFolder = "\\Rejuvenated";//피로회복
			else if (header == Resources.ReSortie_CriticalConditionMessage_Title) SelFolder = "\\critical";//대파
			else if (header == Resources.Dockyard_NotificationMessage_Title) SelFolder = "\\Dockyard";//건조
			else return string.Empty;//해당되는 헤더가 없을 경우 empty을 반환
			if (!Directory.Exists(Main_folder + SelFolder)) return string.Empty;//폴더검사해서 폴더가 없으면 empty 출력
			
			Volume checkV = null;
			VolumeViewModel checkT = new VolumeViewModel();

			if (checkT.IsExistSoundDevice()) checkV = Volume.GetInstance();
			else return string.Empty;

			List<string> FileList = Directory.GetFiles(Main_folder+SelFolder, "*.wav", SearchOption.AllDirectories)
	.Concat(Directory.GetFiles(Main_folder + SelFolder, "*.mp3", SearchOption.AllDirectories)).ToList();

			if (!checkV.IsMute && FileList.Count > 0)
			{
				Random Rnd = new Random();
				return FileList[Rnd.Next(0, FileList.Count)];
			}
			else return string.Empty;//파일이 없는 경우나 음소거인 경우
		}

		private void DisposeWave()
		{
			if (SoundOut != null)
			{
				if (SoundOut.PlaybackState == NAudio.Wave.PlaybackState.Playing) SoundOut.Stop();
				SoundOut.Dispose();
				SoundOut = null;
			}
			if (BlockStream != null)
			{
				BlockStream.Dispose();
				BlockStream = null;
			}
		}

	}
}