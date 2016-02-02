using System;
using System.Windows.Shell;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// <see cref="ITaskbarProgress.Updated"/> イベントのデータを提供します。
	/// </summary>
	public class TaskbarProgressEventArgs : EventArgs
	{
		public TaskbarItemProgressState ProgressState { get; }

		public double ProgressValue { get; }

		public TaskbarProgressEventArgs() : this(TaskbarItemProgressState.None, .0) { }

		public TaskbarProgressEventArgs(TaskbarItemProgressState state, double value)
		{
			this.ProgressState = state;
			this.ProgressValue = value;
		}
	}
}
