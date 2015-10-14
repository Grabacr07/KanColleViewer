namespace Grabacr07.KanColleWrapper.Models.Translations
{
	public class ProcessUnknownEventArgs
	{
		public object RawData { get; set; }
		public string Culture { get; set; }
		public TranslationProviderType TranslationProvider { get; set; }
		public bool AlreadySubmitted { get; set; }

		public ProcessUnknownEventArgs(TranslationProviderType translationProviderType, string culture, object rawdata)
		{
			Culture = culture;
			TranslationProvider = translationProviderType;
			RawData = rawdata;
			AlreadySubmitted = false;
		}
	}
}