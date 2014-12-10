using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Grabacr07.KanColleViewer.Models
{
	public class RelayCommand : ICommand
	{

		readonly Action _execute;
		readonly Func<bool> _canExecute;

		public RelayCommand(Action execute)
			: this(execute, null)
		{
		}

		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_canExecute != null)
					CommandManager.RequerySuggested += value;
			}
			remove
			{
				if (_canExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}


		public void Execute(object parameter)
		{
			_execute();
		}

		public Action CurrentAction { get { return _execute; } }
	}
}