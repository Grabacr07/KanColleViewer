﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using MetroRadiance.Controls;
using Grabacr07.KanColleViewer.Models;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public abstract class TabItemViewModel : ItemViewModel, ITabItem
	{
		#region Name 変更通知プロパティ

		/// <summary>
		/// タブ名を取得します。
		/// </summary>
		public abstract string Name { get; protected set; }

		#endregion

		#region Badge 変更通知プロパティ

		private int? _Badge;

		/// <summary>
		/// バッジを取得します。
		/// </summary>
		public virtual int? Badge
		{
			get { return this._Badge; }
			protected set
			{
				if (this._Badge != value)
				{
					this._Badge = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Status 変更通知プロパティ

		private ViewModel _Status;

		/// <summary>
		/// ステータス バーに表示するステータスを取得します。
		/// </summary>
		public virtual ViewModel Status
		{
			get { return this._Status; }
			protected set
			{
				if (this._Status != value)
				{
					this._Status = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		protected TabItemViewModel()
		{
			if (Helper.IsInDesignMode) return;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(ResourceService.Current)
			{
				(sender, args) => this.RaisePropertyChanged("Name"),
			});
		}
	}
}
