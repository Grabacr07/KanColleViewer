using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Views;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels
{
	/// <summary>
	/// アプリケーションの主機能である <see cref="Information"/> をホストするウィンドウのためのデータを提供します。
	/// </summary>
	public class InformationWindowViewModel : MainWindowViewModelBase
	{
		public WindowSettings Settings { get; }

		/// <summary>
		/// アタッチされたウィンドウが閉じられたときに発生します。
		/// </summary>
		public event EventHandler Closed;

		public InformationWindowViewModel(MainWindowViewModelBase owner) : this(false)
		{
			// 別のメイン ウィンドウがいて、その分割ウィンドウとして表示されるケース
			// メイン ウィンドウ側の Content と同じものを表示する

			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			owner.Subscribe(nameof(this.Content), () => this.Content = owner.Content).AddTo(this);
		}

		public InformationWindowViewModel(bool isMainWindow) : base(isMainWindow)
		{
			this.Settings = new WindowSettings(nameof(InformationWindow));
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.Closed?.Invoke(this, new EventArgs());
		}
	}
}
