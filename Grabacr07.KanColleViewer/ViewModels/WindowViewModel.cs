using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Livet;
using Livet.Behaviors;
using Livet.Messaging;
using Livet.Messaging.Windows;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class WindowViewModel : ViewModel
	{
		#region Title 変更通知プロパティ

		private string _Title;

		public string Title
		{
			get { return this._Title; }
			set
			{
				if (this._Title != value)
				{
					this._Title = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CanClose 変更通知プロパティ

		private bool _CanClose = true;

		public virtual bool CanClose
		{
			get { return this._CanClose; }
			set
			{
				if (this._CanClose != value)
				{
					this._CanClose = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public WindowState WindowState { get; set; }

		public bool DialogResult { get; protected set; }

		public virtual void Initialize() { }

		/// <summary>
		/// ウィンドウをアクティブ化することを試みます。最小化されている場合は通常状態にします。
		/// </summary>
		public virtual void Activate()
		{
			if (this.WindowState == WindowState.Minimized)
			{
				this.SendWindowAction(WindowAction.Normal);
			}

			this.SendWindowAction(WindowAction.Active);
		}

		/// <summary>
		/// ウィンドウが閉じるのがキャンセルされたときに <see cref="WindowCloseCancelBehavior"/> によって呼びだされるコールバック メソッドです。
		/// </summary>
		public virtual void CloseCanceledCallback() { }

		/// <summary>
		/// ウィンドウを閉じます。
		/// </summary>
		public virtual void Close()
		{
			this.SendWindowAction(WindowAction.Close);
		}

		protected void SendWindowAction(WindowAction action)
		{
			this.Messenger.Raise(new WindowActionMessage(action, "Window.WindowAction"));
		}

		protected void Transition(ViewModel viewModel, Type windowType, TransitionMode mode)
		{
			this.Messenger.Raise(new TransitionMessage(windowType, viewModel, mode, "Window.Transition"));
		}

		protected void InvokeOnUIDispatcher(Action action)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(action);
		}
	}
}
