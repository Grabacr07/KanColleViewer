using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.Messaging;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels
{
	/// <summary>
	/// アプリケーションのメイン ウィンドウのためのデータを提供します。このクラスは抽象クラスです。
	/// </summary>
	public abstract class MainWindowViewModelBase : WindowViewModel
	{
		/// <summary>
		/// アタッチされたウィンドウがアプリケーションのメイン ウィンドウかどうかを示す値を取得します。
		/// </summary>
		public bool IsMainWindow { get; }

		#region Content 変更通知プロパティ

		private ViewModel _Content;

		/// <summary>
		/// アタッチされたウィンドウに表示するコンテンツを特定するための <see cref="ViewModel"/> オブジェクトを取得または設定します。
		/// </summary>
		public virtual ViewModel Content
		{
			get { return this._Content; }
			set
			{
				if (this._Content != value)
				{
					this._Content = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region StatusBar 変更通知プロパティ

		private ViewModel _StatusBar;

		public ViewModel StatusBar
		{
			get { return this._StatusBar; }
			set
			{
				if (this._StatusBar != value)
				{
					this._StatusBar = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// ウィンドウを閉じることができるかどうかを示す値を取得または設定します。
		/// このプロパティは、以下のいずれかの条件を満たすとき、true を返します。
		/// <para>・このプロパティ値に true が設定されているとき。</para>
		/// <para>・メイン ウィンドウであるとマークされていないとき。</para>
		/// <para>・アプリケーションの終了確認動作が「なし」に設定されているとき。</para>
		/// <para>・アプリケーションの終了確認動作が「出撃中のみ」に設定されており、出撃中でないとき。</para>
		/// <para>・アプリケーションの起動または終了動作中のとき。</para>
		/// </summary>
		public override sealed bool CanClose => base.CanClose
												|| !this.IsMainWindow
												|| (GeneralSettings.ExitConfirmationType == ExitConfirmationType.None)
												|| (GeneralSettings.ExitConfirmationType == ExitConfirmationType.InSortieOnly && !KanColleClient.Current.IsInSortie)
												|| Application.Instance.State != ApplicationState.Running;

		protected MainWindowViewModelBase(bool isMainWindow)
		{
			this.Title = ProductInfo.Title;
			this.IsMainWindow = isMainWindow;
			this.CanClose = false;

			Application.Instance.Subscribe(nameof(Application.State), this.RaiseCanCloseChanged).AddTo(this);
			KanColleClient.Current.Subscribe(nameof(KanColleClient.IsInSortie), this.RaiseCanCloseChanged).AddTo(this);
			GeneralSettings.ExitConfirmationType.Subscribe(_ => this.RaiseCanCloseChanged());
		}


		/// <summary>
		/// 現在のウィンドウから、指定したウィンドウに <see cref="TransitionMode.NewOrActive"/> で遷移します。
		/// </summary>
		public void Transition(ViewModel viewModel, Type windowType)
		{
			this.Transition(viewModel, windowType, TransitionMode.NewOrActive, false);
		}

		/// <summary>
		/// 現在のウィンドウでダイアログを表示します。
		/// </summary>
		public void Dialog(ViewModel viewModel, Type windowType)
		{
			this.Transition(viewModel, windowType, TransitionMode.Modal, true);
		}

		protected override void CloseCanceledCallbackCore()
		{
			var dialog = new DialogViewModel { Title = "終了確認", };

			this.Dialog(dialog, typeof(ExitDialog));

			if (dialog.DialogResult)
			{
				this.CanClose = true;
				this.InvokeOnUIDispatcher(this.Close);
			}
		}

		protected void RaiseCanCloseChanged()
		{
			this.RaisePropertyChanged(nameof(this.CanClose));
		}
	}
}
