﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.Windows;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Docks
{
	public class BuildingDockViewModel : ViewModel
	{
		private readonly BuildingDock source;

		public int Id
		{
			get { return this.source.Id; }
		}

		public string Ship
		{
			get { return source.Ship == null ? "----" : source.Ship.Name; }
		}

		public string CompleteTime
		{
			get { return source.CompleteTime.HasValue ? source.CompleteTime.Value.LocalDateTime.ToString("MM/dd HH:mm") : "--/-- --:--:--"; }
		}

		public string Remaining
		{
			get { return source.Remaining.HasValue ? source.Remaining.Value.ToString(@"hh\:mm\:ss") : "--:--:--"; }
		}

		public BuildingDockState State
		{
			get { return this.source.State; }
		}

		#region CanDisplayShipName 変更通知プロパティ

		private bool _CanDisplayShipName;

		/// <summary>
		/// 建造中の艦娘の艦名を表示するかどうかを示す値を取得または設定します。
		/// </summary>
		public bool CanDisplayShipName
		{
			get { return this._CanDisplayShipName; }
			set
			{
				if (this._CanDisplayShipName != value)
				{
					this._CanDisplayShipName = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsNotifyCompleted 変更通知プロパティ

		private bool _IsNotifyCompleted;

		public bool IsNotifyCompleted
		{
			get { return this._IsNotifyCompleted; }
			set
			{
				if (this._IsNotifyCompleted != value)
				{
					this._IsNotifyCompleted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public BuildingDockViewModel(BuildingDock source)
		{
			this.source = source;
			this.CompositeDisposable.Add(new PropertyChangedEventListener(source, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));

			if (Helper.IsWindows8OrGreater)
			{
				source.Completed += (sender, args) =>
				{
					if (this.IsNotifyCompleted)
					{
						Toast.Show(
							"建造完了",
							string.Format("工廠第 {0} ドックでの{1}の建造が完了しました。", this.Id, this.CanDisplayShipName ? "「" + this.Ship + "」" : "艦娘"),
							() => App.ViewModelRoot.Messenger.Raise(new WindowActionMessage(WindowAction.Active, "Window/Activate")));
					}
				};
			}
		}
	}
}
