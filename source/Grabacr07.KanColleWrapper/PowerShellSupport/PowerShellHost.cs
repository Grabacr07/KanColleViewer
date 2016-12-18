using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.PowerShellSupport
{
	public interface IPowerShellHost
	{
		ReadOnlyObservableCollection<IPowerShellInvocation> Invocations { get; }
	}

	public class PowerShellHost : IPowerShellHost, IDisposable
	{
		private const string errorMessage = "{0}\r\n    + CategoryInfo          : {1}\r\n    + FullyQualifiedErrorId : {2}";
		private const string errorMessageWithPosition = "{0}\r\n{1}\r\n    + CategoryInfo          : {2}\r\n    + FullyQualifiedErrorId : {3}";

		private readonly Runspace runspace;
		private readonly List<string> history = new List<string>();
		private readonly ObservableCollection<IPowerShellInvocation> invocations = new ObservableCollection<IPowerShellInvocation>();
		private ReadOnlyObservableCollection<IPowerShellInvocation> readonlyInvocations;
		private int count;

		ReadOnlyObservableCollection<IPowerShellInvocation> IPowerShellHost.Invocations
			=> this.readonlyInvocations ?? (this.readonlyInvocations = new ReadOnlyObservableCollection<IPowerShellInvocation>(this.invocations));

		public PowerShellHost()
		{
			this.runspace = RunspaceFactory.CreateRunspace();
		}

		public void Open()
		{
			this.runspace.Open();

			var assembly = new FileInfo(Assembly.GetAssembly(typeof(GetShipCmdlet)).Location ?? "dummy");
			if (assembly.Exists)
			{
				using (var powershell = PowerShell.Create())
				{
					powershell.Runspace = this.runspace;
					powershell.AddCommand("Import-Module").AddParameter("Name", assembly.FullName);
					powershell.Invoke();
				}
			}

			this.invocations.Add(new PowerShellMessage("KanColleViewer PowerShell Host - version 0.1"));
			this.invocations.Add(new PowerShellInvocation(++this.count, x => this.HandleInvocationRequested(x), this.history));
		}

		protected async void HandleInvocationRequested(PowerShellInvocation sender)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(sender.Script))
				{
					sender.SetResult(new InvocationResult());
				}
				else
				{
					using (var powershell = PowerShell.Create())
					{
						powershell.Runspace = this.runspace;
						powershell.AddScript(sender.Script);

						// ReSharper disable once AccessToDisposedClosure
						var results = await Task.Factory.FromAsync(powershell.BeginInvoke(), x => powershell.EndInvoke(x));
						var error = this.CreateResultIfError(powershell);

						sender.SetResult(error ?? await this.HandleResult(results));
					}

					this.history.Add(sender.Script);
				}
			}
			catch (Exception ex)
			{
				this.CreateErrorMessage(ex);
			}
			finally
			{
				this.invocations.Add(new PowerShellInvocation(++this.count, x => this.HandleInvocationRequested(x), this.history));
			}
		}

		protected virtual Task<InvocationResult> HandleResult(PSDataCollection<PSObject> results)
		{
			return Task.Run(() => this.OutString(results));
		}

		protected InvocationResult OutString<T>(IEnumerable<T> input)
		{
			try
			{
				var sb = new StringBuilder();

				using (var powershell = PowerShell.Create())
				{
					powershell.Runspace = this.runspace;
					powershell.AddCommand("Out-String");

					foreach (var result in powershell.Invoke(input))
					{
						sb.AppendLine(result.ToString());
					}
				}

				return new InvocationResult(InvocationResultKind.Normal, sb.ToString());
			}
			catch (Exception ex)
			{
				return new InvocationResult(InvocationResultKind.Error, this.CreateErrorMessage(ex));
			}
		}

		protected InvocationResult CreateResultIfError(PowerShell powershell)
		{
			if (powershell.Streams.Error == null || powershell.Streams.Error.Count == 0) return null;

			var sb = new StringBuilder();
			foreach (var error in powershell.Streams.Error)
			{
				sb.AppendLine(string.Format(errorMessageWithPosition, error, error.InvocationInfo.PositionMessage, error.CategoryInfo, error.FullyQualifiedErrorId));
			}

			return new InvocationResult(InvocationResultKind.Error, sb.ToString());
		}

		protected string CreateErrorMessage(Exception ex)
		{
			var container = ex as IContainsErrorRecord;
			if (container?.ErrorRecord == null)
			{
				return ex.Message;
			}

			var invocationInfo = container.ErrorRecord.InvocationInfo;
			if (invocationInfo == null)
			{
				return string.Format(errorMessage, container.ErrorRecord, container.ErrorRecord.CategoryInfo, container.ErrorRecord.FullyQualifiedErrorId);
			}

			if (invocationInfo.PositionMessage != null && errorMessage.IndexOf(invocationInfo.PositionMessage, StringComparison.Ordinal) != -1)
			{
				return string.Format(errorMessage, container.ErrorRecord, container.ErrorRecord.CategoryInfo, container.ErrorRecord.FullyQualifiedErrorId);
			}

			return string.Format(errorMessageWithPosition, container.ErrorRecord, invocationInfo.PositionMessage, container.ErrorRecord.CategoryInfo, container.ErrorRecord.FullyQualifiedErrorId);
		}

		public void Dispose()
		{
			this.runspace?.Dispose();
		}
	}
}
