﻿
namespace Grabacr07.KanColleViewer.Views.Catalogs
{
	partial class NotePad
	{
		public NotePad()
		{
			this.InitializeComponent();
			this.Closed += (sender, args) =>
			{
				KanColleViewer.ViewModels.Catalogs.NotePadViewModel.Current.SaveText();
			};

			Application.Instance.MainWindow.Closed += (sender, args) => this.Close();
		}
	}
}
