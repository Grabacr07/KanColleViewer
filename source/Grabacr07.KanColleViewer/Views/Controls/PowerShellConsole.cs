using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Grabacr07.KanColleWrapper.PowerShellSupport;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	public class PowerShellTestWindow : Window
	{
		public PowerShellTestWindow()
		{
			var host = new PowerShellHost();
			var console = new PowerShellConsole { PowerShellHost = host, };

			this.Content = console;

			console.Focus();
			host.Open();
		}
	}

	public class PowerShellConsole : RichTextBox
	{
		private InvocationSection currentSection;

		static PowerShellConsole()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(PowerShellConsole),
				new FrameworkPropertyMetadata(typeof(PowerShellConsole)));
		}

		#region PowerShellHost dependency property

		public static readonly DependencyProperty PowerShellHostProperty = DependencyProperty.Register(
			nameof(PowerShellHost), typeof(IPowerShellHost), typeof(PowerShellConsole), new PropertyMetadata(default(IPowerShellHost), HandlePowerShellHostChanged));

		private static void HandlePowerShellHostChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var instance = (PowerShellConsole)sender;
			var oldValue = (IPowerShellHost)args.OldValue;
			var newValue = (IPowerShellHost)args.NewValue;

			if (oldValue != null)
			{
				((INotifyCollectionChanged)oldValue.Invocations).CollectionChanged -= instance.HandleInvocations;
			}
			if (newValue != null)
			{
				((INotifyCollectionChanged)newValue.Invocations).CollectionChanged += instance.HandleInvocations;
				var last = newValue.Invocations.LastOrDefault();
				if (last != null) instance.Next(last);
			}
		}

		public IPowerShellHost PowerShellHost
		{
			get { return (IPowerShellHost)this.GetValue(PowerShellHostProperty); }
			set { this.SetValue(PowerShellHostProperty, value); }
		}

		#endregion

		#region ErrorForeground dependency property

		public static readonly DependencyProperty ErrorForegroundProperty = DependencyProperty.Register(
			nameof(ErrorForeground), typeof(Brush), typeof(PowerShellConsole), new PropertyMetadata(default(Brush)));

		public Brush ErrorForeground
		{
			get { return (Brush)this.GetValue(ErrorForegroundProperty); }
			set { this.SetValue(ErrorForegroundProperty, value); }
		}

		#endregion

		public PowerShellConsole()
		{
			this.Loaded += (sender, args) => DataObject.AddPastingHandler(this, this.HandlePaste);
			this.Unloaded += (sender, args) => DataObject.RemovePastingHandler(this, this.HandlePaste);

			CommandManager.AddPreviewExecutedHandler(this, this.HandleCommandExecuted);
			this.Document.Blocks.Remove(this.Document.Blocks.FirstBlock);
		}

		private void HandleInvocations(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (!this.Dispatcher.CheckAccess())
			{
				this.Dispatcher.Invoke(() => this.HandleInvocations(sender, args));
				return;
			}

			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				this.Next(args.NewItems.OfType<IPowerShellInvocation>().First());
			}
		}

		private void Next(IPowerShellInvocation invocation)
		{
			var section = new InvocationSection(this, invocation);

			this.Document.Blocks.Add(section);

			if (section.CanEditing)
			{
				this.currentSection = section;
				this.CaretPosition = section.Editor.ContentEnd;
			}
		}

		private void Invoke()
		{
			if (this.currentSection == null
				|| this.currentSection.Invocation.Status != InvocationStatus.Ready)
			{
				return;
			}

			var area = this.currentSection.Editor;
			var prompt = this.currentSection.Prompt;
			var script = new TextRange(area.ContentStart.GetPositionAtOffset(prompt.Length + 1), area.ContentEnd).Text;

			this.currentSection.Invocation.Script = script;
			this.currentSection.Invocation.Invoke();
		}

		private void HandleCommandExecuted(object sender, ExecutedRoutedEventArgs args)
		{
			if (!this.CaretIsInEditArea())
			{
				if (args.Command == ApplicationCommands.Cut ||
					args.Command == ApplicationCommands.Paste ||
					args.Command == ApplicationCommands.Delete)
				{
					args.Handled = true;
				}
			}
		}

		private void MoveCaretToDocumentEnd()
		{
			if (!this.CaretIsInEditArea())
			{
				this.CaretPosition = this.Document.ContentEnd;
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			if (e.Key == Key.Enter)
			{
				this.Invoke();
				e.Handled = true;
			}
			else if (this.CaretIsInEditArea())
			{
				if (e.Key == Key.Escape)
				{
					this.currentSection.Invocation.Script = "";
					e.Handled = true;
				}
				if (e.Key == Key.Up)
				{
					this.currentSection.Invocation.SetNextHistory();
					e.Handled = true;
				}
				else if (e.Key == Key.Down)
				{
					this.currentSection.Invocation.SetPreviousHistory();
					e.Handled = true;
				}
				else if ((e.Key == Key.Left || e.Key == Key.Back)
					&& this.CaretIsInLeftMostOfEditArea())
				{
					e.Handled = true;
				}
			}
			else
			{
				if (e.Key == Key.Back || e.Key == Key.Delete)
				{
					e.Handled = true;
				}
				else if (Interop.GetCharFromKey(e.Key) != null)
				{
					this.MoveCaretToDocumentEnd();
				}
			}
		}

		private void HandlePaste(object sender, DataObjectPastingEventArgs e)
		{
			var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
			if (!isText) return;

			var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
			if (text == null) return;

			this.MoveCaretToDocumentEnd();
		}

		private bool CaretIsInEditArea()
			=> this.currentSection != null && this.currentSection.CaretIsInEditArea(this.CaretPosition);

		private bool CaretIsInLeftMostOfEditArea()
			=> this.currentSection != null && this.currentSection.CaretIsInLeftMostOfEditArea(this.CaretPosition);


		private static class Interop
		{
			private enum MapType : uint
			{
				// ReSharper disable InconsistentNaming
				MAPVK_VK_TO_VSC = 0x0,
				MAPVK_VSC_TO_VK = 0x1,
				MAPVK_VK_TO_CHAR = 0x2,
				MAPVK_VSC_TO_VK_EX = 0x3,
				// ReSharper restore InconsistentNaming
			}

			[DllImport("user32.dll")]
			private static extern int ToUnicode(
				uint wVirtKey,
				uint wScanCode,
				byte[] lpKeyState,
				[Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder pwszBuff,
				int cchBuff,
				uint wFlags);

			[DllImport("user32.dll")]
			private static extern bool GetKeyboardState(byte[] lpKeyState);

			[DllImport("user32.dll")]
			private static extern uint MapVirtualKey(uint uCode, MapType uMapType);

			public static char? GetCharFromKey(Key key)
			{
				char? ch = null;

				var virtualKey = KeyInterop.VirtualKeyFromKey(key);
				var keyboardState = new byte[256];
				GetKeyboardState(keyboardState);

				var scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
				var stringBuilder = new StringBuilder(2);

				var result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
				switch (result)
				{
					case -1:
						break;
					case 0:
						break;
					case 1:
						ch = stringBuilder[0];
						break;
					default:
						ch = stringBuilder[0];
						break;
				}

				return ch;
			}
		}

		public class InvocationSection : Section
		{
			private readonly PowerShellConsole owner;

			public IPowerShellInvocation Invocation { get; }

			public bool CanEditing { get; private set; }

			public Paragraph Editor { get; }

			public string Prompt { get; }

			public InvocationSection(PowerShellConsole owner, IPowerShellInvocation invocation)
			{
				if (invocation.Result == null)
				{
					this.owner = owner;
					this.Invocation = invocation;
					this.Invocation.PropertyChanged += this.HandleInvocationPropertyChanged;

					this.Prompt = $"[{invocation.Number}] > ";
					this.Editor = new Paragraph();
					this.Editor.Inlines.Add(this.Prompt);
					this.Blocks.Add(this.Editor);

					this.CanEditing = invocation.Status == InvocationStatus.Ready;
				}
				else
				{
					this.owner = owner;
					this.Invocation = invocation;
					this.SetResult(invocation.Result);

					this.CanEditing = false;
				}
			}

			private void HandleInvocationPropertyChanged(object sender, PropertyChangedEventArgs args)
			{
				if (!this.Dispatcher.CheckAccess())
				{
					this.Dispatcher.Invoke(() => this.HandleInvocationPropertyChanged(sender, args));
					return;
				}

				switch (args.PropertyName)
				{
					case nameof(IPowerShellInvocation.Script):
					{
						if (Equals(this, this.owner.currentSection))
						{
							var range = new TextRange(this.Editor.ContentStart.GetPositionAtOffset(this.Prompt.Length + 1), this.Editor.ContentEnd);
							var script = this.Invocation.Script;
							range.Text = script;
							this.owner.CaretPosition = this.Editor.ContentEnd;
						}
						break;
					}
					case nameof(IPowerShellInvocation.Status):
					{
						switch (this.Invocation.Status)
						{
							case InvocationStatus.Ready:
								this.CanEditing = true;
								break;

							case InvocationStatus.Invoking:
								this.CanEditing = false;
								break;

							case InvocationStatus.Invoked:
								this.CanEditing = false;
								this.Invocation.PropertyChanged -= this.HandleInvocationPropertyChanged;
								break;
						}
						break;
					}
					case nameof(IPowerShellInvocation.Result):
					{
						this.SetResult(this.Invocation.Result);
						break;
					}
				}
			}

			private void SetResult(InvocationResult result)
			{
				switch (result.Kind)
				{
					case InvocationResultKind.Empty:
						break;

					case InvocationResultKind.Normal:
					{
						var paragraph = new Paragraph();
						paragraph.Inlines.Add(result.Message);
						this.Blocks.Add(paragraph);
						break;
					}

					case InvocationResultKind.Error:
					{
						var paragraph = new Paragraph();
						paragraph.Inlines.Add(result.Message);
						paragraph.Foreground = this.owner.ErrorForeground;
						this.Blocks.Add(paragraph);
						this.Blocks.Add(new Paragraph());
						break;
					}
				}
			}

			public bool CaretIsInEditArea(TextPointer caretPostion)
				=> this.CanEditing
					&& this.Editor.ContentStart.IsInSameDocument(caretPostion)
					&& (this.Editor.ContentStart.GetOffsetToPosition(caretPostion) - 1 >= this.Prompt.Length);

			public bool CaretIsInLeftMostOfEditArea(TextPointer caretPostion)
				=> this.CanEditing
					&& this.Editor.ContentStart.IsInSameDocument(caretPostion)
					&& (this.Editor.ContentStart.GetOffsetToPosition(caretPostion) - 1 == this.Prompt.Length);
		}
	}
}
