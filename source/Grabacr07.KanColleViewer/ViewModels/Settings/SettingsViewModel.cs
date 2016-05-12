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
using Livet;

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

		public NetworkSettingsViewModel NetworkSettings { get; }

		public UserStyleSheetSettingsViewModel UserStyleSheetSettings { get; }

		public NavigatorViewModel Navigator { get; set; }

		public BrowserZoomFactor BrowserZoomFactor { get; }

		public IReadOnlyCollection<CultureViewModel> Cultures { get; }

		public IReadOnlyCollection<BindableTextViewModel> Libraries { get; }

		public List<PluginViewModel> LoadedPlugins { get; }

		public List<LoadFailedPluginViewModel> FailedPlugins { get; }

		#region ViewRangeSettingsCollection 変更通知プロパティ

		private List<ICalcViewRange> _ViewRangeSettingsCollection;

		public List<ICalcViewRange> ViewRangeSettingsCollection
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

		#region SelectedViewRangeCalcType 変更通知プロパティ

		private ICalcViewRange _SelectedViewRangeCalcType;

		public ICalcViewRange SelectedViewRangeCalcType
		{
			get { return this._SelectedViewRangeCalcType; }
			set
			{
				if (this._SelectedViewRangeCalcType != value)
				{
					this._SelectedViewRangeCalcType = value;
					KanColleSettings.ViewRangeCalcType.Value = value.Id;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		private SettingsViewModel()
		{
			this.ScreenshotSettings = new ScreenshotSettingsViewModel().AddTo(this);
			this.WindowSettings = new WindowSettingsViewModel().AddTo(this);
			this.NetworkSettings = new NetworkSettingsViewModel().AddTo(this);
			this.UserStyleSheetSettings = new UserStyleSheetSettingsViewModel().AddTo(this);

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

			this.ViewRangeSettingsCollection = ViewRangeCalcLogic.Logics.ToList();
			this.SelectedViewRangeCalcType = this.ViewRangeSettingsCollection
				.FirstOrDefault(x => x.Id == KanColleSettings.ViewRangeCalcType)
				?? this.ViewRangeSettingsCollection.First();

			this.LoadedPlugins = new List<PluginViewModel>(
				PluginService.Current.Plugins.Select(x => new PluginViewModel(x)));

			this.FailedPlugins = new List<LoadFailedPluginViewModel>(
				PluginService.Current.FailedPlugins.Select(x => new LoadFailedPluginViewModel(x)));
		}


		public void Initialize()
		{
			this.WindowSettings.Initialize();
			this.NetworkSettings.Initialize();
			this.UserStyleSheetSettings.Initialize();
		}
	}
}
