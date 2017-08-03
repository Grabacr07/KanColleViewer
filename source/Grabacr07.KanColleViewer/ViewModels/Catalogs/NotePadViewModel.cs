using MetroTrilithon.Mvvm;
using System.IO;
using System.Text;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class NotePadViewModel : WindowViewModel
	{
		#region singleton

		private static NotePadViewModel current = new NotePadViewModel();

		public static NotePadViewModel Current
		{
			get { return current; }
		}

		#endregion

		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		private string _NotePadContent;
		public string NotePadContent
		{
			get { return this._NotePadContent; }
			set
			{
				if (this._NotePadContent != value)
				{
					this._NotePadContent = value;
					this.RaisePropertyChanged();
				}
			}
		}
		public NotePadViewModel()
		{
			current = this;
			this.Title = "메모장";
			LoadText();

		}
		public void SaveText()
		{
			var stream = new StreamWriter(Path.Combine(MainFolder, "Note.txt"), false, Encoding.UTF8);
			stream.Write(NotePadContent);
			stream.Flush();
			stream.Close();
		}
		private void LoadText()
		{
			if (File.Exists(Path.Combine(MainFolder, "Note.txt")))
			{
				var stream = new StreamReader(Path.Combine(MainFolder, "Note.txt"), Encoding.UTF8);

				this.NotePadContent = stream.ReadToEnd();
				stream.Close();
			}
		}
	}
}
