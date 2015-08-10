using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Composition;
using Grabacr07.KanColleWrapper.Models;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class SettingsViewModel : TabItemViewModel
	{
		public static SettingsViewModel Instance { get; } = new SettingsViewModel();


		public override string Name
		{
			get { return Resources.Settings; }
			protected set { throw new NotImplementedException(); }
		}


		public ScreenshotSettingsViewModel ScreenshotSettings { get; }

		public WindowSettingsViewModel WindowSettings { get; }

		public NavigatorViewModel Navigator { get; set; }

		public BrowserZoomFactor BrowserZoomFactor { get; }

		public IReadOnlyCollection<CultureViewModel> Cultures { get; }

		public IReadOnlyCollection<BindableTextViewModel> Libraries { get; }

		public List<PluginViewModel> LoadedPlugins { get; }

		public List<LoadFailedPluginViewModel> FailedPlugins { get; }

		#region ViewRangeSettingsCollection 変更通知プロパティ

		private List<ViewRangeSettingsViewModel> _ViewRangeSettingsCollection;

		public List<ViewRangeSettingsViewModel> ViewRangeSettingsCollection
		{
			get { return this._ViewRangeSettingsCollection; }
			set
			{
				if (this._ViewRangeSettingsCollection != value)
				{
					this._ViewRangeSettingsCollection = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private SettingsViewModel()
		{
			this.ScreenshotSettings = new ScreenshotSettingsViewModel().AddTo(this);
			this.WindowSettings = new WindowSettingsViewModel().AddTo(this);

			this.BrowserZoomFactor = new BrowserZoomFactor { Current = GeneralSettings.BrowserZoomFactor };
			this.BrowserZoomFactor
				.Subscribe(nameof(this.BrowserZoomFactor.Current), () => GeneralSettings.BrowserZoomFactor.Value = this.BrowserZoomFactor.Current)
				.AddTo(this);
			GeneralSettings.BrowserZoomFactor.Subscribe(x => this.BrowserZoomFactor.Current = x).AddTo(this);

			this.Cultures = new[] { new CultureViewModel { DisplayName = "(auto)" } }
				.Concat(ResourceService.Current.SupportedCultures
					.Select(x => new CultureViewModel { DisplayName = x.EnglishName, Name = x.Name })
					.OrderBy(x => x.DisplayName))
				.ToList();

			this.Libraries = ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					// ☝プロダクト名の途中で改行されないように、space を non-break space に置き換えてあげてるんだからねっっ
					return list;
				});

			this.ViewRangeSettingsCollection = ViewRangeCalcLogic.Logics
				.Select(x => new ViewRangeSettingsViewModel(x))
				.ToList();

			this.LoadedPlugins = new List<PluginViewModel>(
				PluginService.Current.Plugins.Select(x => new PluginViewModel(x)));

			this.FailedPlugins = new List<LoadFailedPluginViewModel>(
				PluginService.Current.FailedPlugins.Select(x => new LoadFailedPluginViewModel(x)));
		}


		public void Initialize()
		{
			this.WindowSettings.Initialize();
		}


		public class ViewRangeSettingsViewModel
		{
			private bool selected;

			public ICalcViewRange Logic { get; set; }

			public bool Selected
			{
				get { return this.selected; }
				set
				{
					this.selected = value;
					if (value)
					{
						KanColleSettings.ViewRangeCalcType.Value = this.Logic.Id;
					}
				}
			}

			public ViewRangeSettingsViewModel(ICalcViewRange logic)
			{
				this.Logic = logic;
				this.selected = KanColleSettings.ViewRangeCalcType == logic.Id;
			}
		}
	}
}
