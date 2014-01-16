using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Livet;

namespace Grabacr07.KanColleViewer.Model
{
	public enum Theme
	{
		Dark, 
		Light,
	}

	public enum Accent
	{
		Purple,
		Blue,
		Orange,
	}

	public class ThemeService : NotificationObject
	{
		#region singleton members

		private static readonly ThemeService current = new ThemeService();

		public static ThemeService Current
		{
			get { return current; }
		}

		#endregion

		private ResourceDictionary theme;
		private ResourceDictionary accent;
		private bool initialized;

		#region Theme 変更通知プロパティ

		private Theme _Theme = Theme.Dark;

		public Theme Theme
		{
			get { return this._Theme; }
			set
			{
				if (this.initialized && this._Theme != value)
				{
					DispatcherHelper.UIDispatcher.Invoke(() =>
					{
						var uri = new Uri(string.Format(@"pack://application:,,,/Themes/Mukyutter.{0}.xaml", value));
						var dic = new ResourceDictionary { Source = uri };

						dic.Keys.OfType<string>()
							.Where(key => this.theme.Contains(key))
							.ForEach(key => this.theme[key] = dic[key]);
					});

					this._Theme = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Accent 変更通知プロパティ

		private Accent _Accent = Accent.Purple;

		public Accent Accent
		{
			get { return this._Accent; }
			set
			{
				if (this.initialized && this._Accent != value)
				{
					DispatcherHelper.UIDispatcher.Invoke(() =>
					{
						var uri = new Uri(string.Format(@"pack://application:,,,/Themes/Accent.{0}.xaml", value));
						var dic = new ResourceDictionary { Source = uri };

						dic.Keys.OfType<string>()
							.Where(key => this.accent.Contains(key))
							.ForEach(key => this.accent[key] = dic[key]);
					});

					this._Accent = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private ThemeService() { }

		public void Initialize(App app)
		{
			this.theme = app.Resources.MergedDictionaries.FirstOrDefault(x => Uri.Compare(
				x.Source, new Uri("/KanColleViewer;component/Themes/Mukyutter.Dark.xaml", UriKind.Relative),
				UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) == 0);

			this.accent = app.Resources.MergedDictionaries.FirstOrDefault(x => Uri.Compare(
				x.Source, new Uri("/KanColleViewer;component/Themes/Accent.Purple.xaml", UriKind.Relative),
				UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) == 0);

			this.initialized = (this.theme != null && this.accent != null);
		}
	}
}
