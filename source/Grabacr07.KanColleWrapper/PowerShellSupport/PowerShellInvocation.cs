using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Grabacr07.KanColleWrapper.PowerShellSupport
{
	/// <summary>
	/// Windows PowerShell おけるコマンド入力から実行までの 1 サイクルの操作を公開します。
	/// </summary>
	public interface IPowerShellInvocation : INotifyPropertyChanged
	{
		InvocationStatus Status { get; }

		InvocationResult Result { get; }

		int Number { get; }

		string Script { get; set; }

		bool SetNextHistory();

		bool SetPreviousHistory();

		void Invoke();
	}

	public enum InvocationStatus
	{
		Ready,
		Invoking,
		Invoked,
	}

	public enum InvocationResultKind
	{
		Empty,
		Normal,
		Error,
	}

	public class InvocationResult
	{
		public InvocationResultKind Kind { get; }

		public string Message { get; }

		public InvocationResult() : this(InvocationResultKind.Empty, null) { }

		public InvocationResult(InvocationResultKind kind, string message)
		{
			this.Kind = kind;
			this.Message = message;
		}
	}

	public class PowerShellInvocation : IPowerShellInvocation
	{
		private readonly Action<PowerShellInvocation> invocationAction;
		private readonly IReadOnlyList<string> history;
		private int currentHistoryIndex;
		private string _Script;
		private InvocationStatus _Status = InvocationStatus.Ready;
		private InvocationResult _Result;

		public InvocationStatus Status
		{
			get { return this._Status; }
			set
			{
				if (this._Status != value)
				{
					this._Status = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public InvocationResult Result
		{
			get { return this._Result; }
			set
			{
				if (this._Result != value)
				{
					this._Result = value;
					this.RaisePropertyChanged();
					this.Status = InvocationStatus.Invoked;
				}
			}
		}

		public string Script
		{
			get { return this._Script; }
			set
			{
				if (this._Script != value)
				{
					this._Script = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public int Number { get; }

		public bool SetNextHistory()
		{
			var index = this.currentHistoryIndex - 1;
			if (index >= 0 && index < this.history.Count)
			{
				this.Script = this.history[index];
				this.currentHistoryIndex = index;
				return true;
			}

			return false;
		}

		public bool SetPreviousHistory()
		{
			var index = this.currentHistoryIndex + 1;
			if (index >= 0 && index < this.history.Count)
			{
				this.Script = this.history[index];
				this.currentHistoryIndex = index;
				return true;
			}

			this.Script = "";
			this.currentHistoryIndex = this.history.Count;
			return false;
		}

		internal PowerShellInvocation(int number, Action<PowerShellInvocation> invocationAction, IReadOnlyList<string> history)
		{
			this.Number = number;
			this.invocationAction = invocationAction;
			this.history = history;
			this.currentHistoryIndex = this.history.Count;
		}

		public void Invoke()
		{
			this.Status = InvocationStatus.Invoking;
			this.invocationAction?.Invoke(this);
		}

		internal void SetResult(InvocationResult result)
		{
			this.Result = result;
			this.Status = InvocationStatus.Invoked;
		}

		#region INotifyPropertyChanged members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}

	public class PowerShellMessage : IPowerShellInvocation
	{
		public InvocationStatus Status { get; } = InvocationStatus.Invoked;

		public InvocationResult Result { get; } 

		int IPowerShellInvocation.Number
		{
			get { throw new NotSupportedException(); }
		}

		string IPowerShellInvocation.Script
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public PowerShellMessage(string message)
		{
			this.Result = new InvocationResult(InvocationResultKind.Normal, message);
		}

		bool IPowerShellInvocation.SetNextHistory()
		{
			throw new NotSupportedException();
		}

		bool IPowerShellInvocation.SetPreviousHistory()
		{
			throw new NotSupportedException();
		}

		void IPowerShellInvocation.Invoke()
		{
			throw new NotSupportedException();
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}
	}
}
