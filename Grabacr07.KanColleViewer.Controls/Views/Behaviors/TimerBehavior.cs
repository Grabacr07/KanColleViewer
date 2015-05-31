using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	/// <summary>
	/// <see cref="TextBlock"/> にタイマー機能を付与します。
	/// </summary>
	public class TimerBehavior : Behavior<TextBlock>
	{
		private DispatcherTimer timer;

		#region Interval 依存関係プロパティ

		/// <summary>
		/// タイマー刻みの間隔の時間を取得または設定します。
		/// </summary>
		public TimeSpan Interval
		{
			get { return (TimeSpan)this.GetValue(IntervalProperty); }
			set { this.SetValue(IntervalProperty, value); }
		}

		/// <summary>
		/// <see cref="Interval"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty IntervalProperty =
			DependencyProperty.Register("Interval", typeof(TimeSpan), typeof(TimerBehavior), new UIPropertyMetadata(TimeSpan.FromSeconds(1), IntervalChangedCallback));

		private static void IntervalChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (TimerBehavior)d;
			instance.timer.Stop();
			instance.timer.Interval = (TimeSpan)e.NewValue;
			instance.timer.Start();
		}

		#endregion

		#region Proc 依存関係プロパティ

		/// <summary>
		/// タイマーによって実行される処理を取得または設定します。
		/// </summary>
		public Func<string> Proc
		{
			get { return (Func<string>)this.GetValue(ProcProperty); }
			set { this.SetValue(ProcProperty, value); }
		}

		/// <summary>
		/// <see cref="Proc"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty ProcProperty =
			DependencyProperty.Register("Proc", typeof(Func<string>), typeof(TimerBehavior), new UIPropertyMetadata(null));

		#endregion

		protected override void OnAttached()
		{
			base.OnAttached();

			this.Initialize();
			this.Start();
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			this.Stop();
			this.timer.Tick -= this.Tick;
		}

		protected virtual void Initialize()
		{
			if (this.timer != null) return;

			this.timer = new DispatcherTimer(DispatcherPriority.Normal)
			{
				Interval = this.Interval
			};
			this.timer.Tick += this.Tick;
		}

		protected virtual void Start()
		{
			this.timer.Start();
			this.AssociatedObject.Text = this.GetText();
		}

		protected virtual void Stop()
		{
			this.timer.Stop();
		}

		protected virtual void Tick(object sender, EventArgs e)
		{
			this.AssociatedObject.Text = this.GetText();
		}

		private string GetText()
		{
			return this.Proc == null ? DateTime.Now.ToShortTimeString() : this.Proc();
		}
	}
}
