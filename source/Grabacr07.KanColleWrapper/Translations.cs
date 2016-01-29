using System.Diagnostics;
using Grabacr07.KanColleWrapper.Models.Translations;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// Currently just a stub. Will most likely be completely removed.
	/// </summary>
	public class Translations : NotificationObject
	{
		/// <summary>
		/// Culture selected for translations.
		/// </summary>
		private static string CurrentCulture => TranslationDataProvider.CurrentCulture;

		private bool enableTranslations;

		/// <summary>
		/// Settings, Enable translations: allows the display of translated resources.
		/// </summary>
		public bool EnableTranslations
		{
			get
			{
				if (this.enableTranslations == (KanColleClient.Current?.Settings?.EnableTranslations ?? false)) return this.enableTranslations;

				this.enableTranslations = KanColleClient.Current?.Settings?.EnableTranslations ?? false;
				this.RaisePropertyChanged();
				return this.enableTranslations;
			}
			set
			{
				if (this.enableTranslations == value) return;

				this.enableTranslations = value;
				this.RaisePropertyChanged();
			}
		}

		internal Translations() : this("en") { }
		/// <summary>
		/// The constructor performs initial loading of translation files.
		/// </summary>
		/// <param name="culture">The culture to use for translations. Defaults to "en-US".</param>
		internal Translations(string culture)
		{
			Debug.WriteLine(this.GetType().Name + ": constructor initialising with <" + culture + ">.");
			this.ChangeCulture();
		}

		/// <summary>
		/// Changes culture and loads or re-loads translation files.
		/// </summary>
		public void ChangeCulture()
		{
			Debug.WriteLine(this.GetType().Name + ": got <" + CurrentCulture + "> in a culture change request.");
			this.RaisePropertyChanged();
		}

		/// <summary>
		/// Looks up a translated string for a resource.
		/// </summary>
		/// <param name="type">Resource type.</param>
		/// <param name="rawData">Raw API data; provides the necessary data (ID, names, etc.) for a look-up.</param>
		/// <returns></returns>
		public string Lookup(TranslationType type, object rawData)
		{
			if (!EnableTranslations || CurrentCulture.StartsWith("ja"))
			{
				Debug.WriteLine(this.GetType().Name + ": lookup called for {0} but translations are disabled or culture is Japanese.", type);
				return null;
			}

			return TranslationDataProvider.Lookup(type, rawData);
		}
	}
}
