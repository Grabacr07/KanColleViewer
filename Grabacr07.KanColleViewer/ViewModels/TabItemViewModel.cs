using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class TabItemViewModel : ViewModel, ITabItem
	{
		#region Name 変更通知プロパティ

		private string _Name;

		/// <summary>
		/// タブ名を取得します。
		/// </summary>
		public virtual string Name
		{
			get { return this._Name; }
			protected set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}

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

		#region IsSelected 変更通知プロパティ

		private bool _IsSelected;

		public virtual bool IsSelected
		{
			get { return this._IsSelected; }
			set
			{
				if (this._IsSelected != value)
				{
					this._IsSelected = value;
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

	}
}
