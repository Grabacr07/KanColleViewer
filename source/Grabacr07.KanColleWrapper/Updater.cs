using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Threading;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Models.Translations;
using Grabacr07.KanColleWrapper.Models.Updater;
using Livet;
using Newtonsoft.Json;

namespace Grabacr07.KanColleWrapper
{
	public class Updater : NotificationObject
	{
		/// <summary>
		/// Whether automatic updates are enabled.
		/// </summary>
		private static bool EnableUpdates => KanColleClient.Current?.Settings?.EnableUpdates ?? false;

		private static bool EnableSubmissions => KanColleClient.Current?.Settings?.EnableAutosubmission ?? false;

		/// <summary>
		/// Currently selected culture.
		/// </summary>
		public string CurrentCulture { get; private set; }

		/// <summary>
		/// KCV updates API version for requests.
		/// </summary>
		private const string apiVersion = "1";

		/// <summary>
		/// URL for version checks.
		/// </summary>
		private readonly string apiVersionCheckUrl;

		/// <summary>
		/// Determines if KCV updates API is available.
		/// </summary>
		private bool apiAvailable;

		/// <summary>
		/// URL to a page with the newest build.
		/// </summary>
		private string downloadUrl = "";

		/// <summary>
		/// URL base for provider requests.
		/// </summary>
		private string kcvApiUrl;

		/// <summary>
		/// Components and their versions.
		/// </summary>
		private Dictionary<TranslationProviderType, string> versions = new Dictionary<TranslationProviderType, string>();

		private UnknownStringsList unknownStrings = new UnknownStringsList();

		/// <summary>
		/// Automatic update timer.
		/// </summary>
		private DispatcherTimer updateCheckTimer = new DispatcherTimer();

		private DispatcherTimer autosubmitTimer = new DispatcherTimer();

		/// <summary>
		/// This event is fired when there is a new version of the application available.
		/// </summary>
		public event EventHandler<UpdateAvailableEventArgs> UpdateAvailable;

		/// <summary>
		/// Initialises the Updater class.
		/// </summary>
		/// <param name="apiurl">Version check URL.</param>
		/// <param name="culture">Culture to set during initialisation.</param>
		public Updater(string apiurl, string culture)
		{
			Debug.WriteLine("{0}: initialising with culture {1} and version check url {2}.", nameof(Updater), culture, apiurl);
			TranslationDataProvider.ProcessUnknown += ProcessUnknown;

			this.updateCheckTimer.Tick += this.dispatcherTimerHandler;
			this.updateCheckTimer.Interval = new TimeSpan(0, 120, 0);

			this.autosubmitTimer.Tick += this.autosubmitTimerHandler;
			this.autosubmitTimer.Interval = new TimeSpan(0, 15, 0);

			this.apiVersionCheckUrl = apiurl;
			this.ChangeCulture(culture);
		}

		/// <summary>
		/// Changes the default culture.
		/// </summary>
		/// <param name="culture">New culture to switch to.</param>
		public void ChangeCulture(string culture)
		{
			if ((culture == null) || (culture == CurrentCulture))
				return;

			Debug.WriteLine("{0}: switching culture to {1}.", nameof(Updater), culture);

			CurrentCulture = culture;
			TranslationDataProvider.LoadLocalTranslations(culture);
			this.CheckForUpdates();
			this.UpdateAsNeeded();
		}

		/// <summary>
		/// Starts or stops the automatic update timer.
		/// </summary>
		/// <param name="enable"></param>
		public void ToggleUpdates(bool enable)
		{
			if (enable)
			{
				if (!this.updateCheckTimer.IsEnabled)
				{
					this.updateCheckTimer.Start();
					if (EnableSubmissions) ToggleSubmission(true);
				}
				Debug.WriteLine("Updater: update checks enabled; timer has been started.");
			}
			else
			{
				if (this.updateCheckTimer.IsEnabled) this.updateCheckTimer.Stop();
				Debug.WriteLine("Updater: update checks disabled; timer has been stopped.");
				ToggleSubmission(false);
			}
		}

		public void ToggleSubmission(bool enable)
		{
			if (enable)
			{
				if (!this.autosubmitTimer.IsEnabled) this.autosubmitTimer.Start();
				Debug.WriteLine("Updater: submission enabled; timer has been started.");
			}
			else
			{
				if (this.autosubmitTimer.IsEnabled) this.autosubmitTimer.Stop();
				Debug.WriteLine("Updater: submission disabled; timer has been stopped.");
			}
		}

		/// <summary>
		/// Loads remote version data from the URL specified.
		/// </summary>
		/// <param name="url">Remote version data URL.</param>
		/// <returns>True if version information was retrieved and parsed successfully.</returns>
		private bool LoadVersions(string url)
		{
			using (ViewerWebClient client = new ViewerWebClient())
			{
				byte[] responseBytes;

				try
				{
					responseBytes = client.UploadValues(url, "POST", this.DefaultRequestParameters());
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Updater: Could not access the API.");
					return false;
				}

				kcvapi_version rawResult;
				if (!this.TryConvertTo(responseBytes, out rawResult)) return false;

				Version apiRemoteVersion;

				if (!Version.TryParse(rawResult.api_version, out apiRemoteVersion) && (apiRemoteVersion.CompareTo("1.0") >= 0))
				{
					Debug.WriteLine("Updater: Server API version check failed.");
					return false;
				}

				kcvApiUrl = rawResult.api_url;

				Debug.WriteLine("Updater: remote API version {0}; providers available: {1}.", rawResult.api_version, rawResult.components.Count());

				foreach (var component in rawResult.components)
				{
					Debug.WriteLine("Updater: provider {0}: version {1}{2}.", component.type, component.version, string.IsNullOrEmpty(component.url) ? "" : " (" + component.url + ")");
					var typeTemp = TranslationDataProvider.StringToTranslationProviderType(component.type);
					if (typeTemp == null) continue;
					versions[typeTemp.Value] = component.version;
					if (typeTemp == TranslationProviderType.App) downloadUrl = component.url; // TODO: proper implementation of overrides for all resource types
				}
			}

			return true;
		}

		/// <summary>
		/// Checks if any updates are needed, downloads and serialises them for local storage.
		/// </summary>
		private void UpdateAsNeeded()
		{
			Debug.WriteLine("API available: {0}, Updates enabled: {1}", apiAvailable, EnableUpdates);
			if (!apiAvailable || !EnableUpdates) return;

			foreach (var version in versions)
			{
				if (!this.IsUpToDate(version.Key))
				{
					Debug.WriteLine("Updater: {0} needs update; local version: {1}, remote: {2}.", version.Key, (version.Key != TranslationProviderType.App) ? TranslationDataProvider.Version(version.Key, CurrentCulture) : Assembly.GetEntryAssembly().GetName().Version.ToString(), string.IsNullOrEmpty(version.Value) ? "N/A" : version.Value);
					if ((version.Key != TranslationProviderType.App) && this.FetchTranslations(version.Key)) TranslationDataProvider.SaveXml(version.Key, CurrentCulture);
				}
			}
		}

		/// <summary>
		/// Checks if specified provider is up to date (compares local version to server version).
		/// </summary>
		/// <param name="type">Translation provider type.</param>
		/// <returns>True if local version is greater than or equal to remote.</returns>
		public bool IsUpToDate(TranslationProviderType type)
		{
			if ((!versions.ContainsKey(type)) || (versions[type] == null) || !apiAvailable) { return true;}

			switch (type)
			{
				case TranslationProviderType.App:
					Version remoteAppVersion, localAppVersion = Assembly.GetEntryAssembly().GetName().Version;

					if (!Version.TryParse(versions[type], out remoteAppVersion)) return true;

					return (remoteAppVersion <= localAppVersion);

				default:
					int verRemote, verLocal;

					if (!int.TryParse(versions[type], out verRemote)) return true;
					if (!int.TryParse(TranslationDataProvider.Version(type, CurrentCulture), out verLocal)) return false;

					return (verRemote <= verLocal);
			}
		}

		/// <summary>
		/// Downloads translations for the specified provider.
		/// </summary>
		/// <param name="type">Provider type.</param>
		/// <returns></returns>
		private bool FetchTranslations(TranslationProviderType type)
		{
			if (!apiAvailable) return false;

			if (type == TranslationProviderType.App) return false;
			if (type == TranslationProviderType.Operations) return false;
			if (type == TranslationProviderType.Expeditions) return false;

			var apiUri = new Uri(kcvApiUrl + type.ToString().ToLower() + "/");

			using (var client = new ViewerWebClient())
			{
				byte[] responseBytes;
				try
				{
					responseBytes = client.UploadValues(apiUri, "POST", this.DefaultRequestParameters());
					Debug.WriteLine("{0}: API request sent for {1}, URL: {2}. Response: {3}.", nameof(Updater), type, apiUri, Encoding.UTF8.GetString(responseBytes));
				}
				catch (Exception ex)
				{
					Debug.WriteLine("{0} API request sent for {1}, URL: {2}. Request failed with exception {3}.", nameof(Updater), type, apiUri, ex.Message);
					return false;
				}

				return TranslationDataProvider.LoadJson(type, CurrentCulture, responseBytes);
			}
		}

		/// <summary>
		/// Accesses the remote version list and sends a notification if a new version is available.
		/// </summary>
		private void CheckForUpdates()
		{
			versions = new Dictionary<TranslationProviderType, string>();
			apiAvailable = this.LoadVersions(apiVersionCheckUrl);
			this.SendUpdateNotificationIfNeeded();
		}

		/// <summary>
		/// Sends a notification if a new version of the application is available.
		/// </summary>
		public void SendUpdateNotificationIfNeeded()
		{
			if (this.apiAvailable && !this.IsUpToDate(TranslationProviderType.App))
				this.UpdateAvailable?.Invoke(this, new UpdateAvailableEventArgs(versions[TranslationProviderType.App]));
		}

		/// <summary>
		/// Adds a new unknown item to the send queue.
		/// </summary>
		/// <param name="sender">Sender; will be null due to a static class sending these.</param>
		/// <param name="args">Parameters: raw data.</param>
		public void ProcessUnknown(object sender, ProcessUnknownEventArgs args)
		{
			Debug.WriteLine("Updater: received unknown translation data, type {0}, culture <{1}>, data type <{2}>, JSON <{3}>.", args.TranslationProvider.ToString(), args.Culture, args.RawData.GetType().ToString(), JsonConvert.SerializeObject(args.RawData));
			if ((args.TranslationProvider == TranslationProviderType.Equipment) && (args.RawData is kcsapi_mst_slotitem) && (((kcsapi_mst_slotitem)args.RawData).api_name == "？？？")) return;
			unknownStrings.Add(new UnknownStringItem(args.TranslationProvider, args.Culture, args.RawData));
		}

		/// <summary>
		/// Submits all unknown resources.
		/// TODO: Upon successful submission check the result and mark items that were reported as already present/accepted.
		/// </summary>
		public void SubmitUnknown()
		{
			if (!apiAvailable || !EnableSubmissions) return;

			var sortedstrings = from submitstring in unknownStrings where !submitstring.AlreadySubmitted group submitstring by new { submitstring.Culture, submitstring.ProviderType };
			foreach (var submitstringgroup in sortedstrings)
			{
				string response; // API response
				var providertype = submitstringgroup.Key.ProviderType;
				var culture = submitstringgroup.Key.Culture;
				var rawData = submitstringgroup.Select(x => x.RawData).ToList();
				var submission = JsonConvert.SerializeObject(new SubmitUnknownApi(culture, rawData));

				Debug.WriteLine("Updater: serialising data for provider {0} and culture {1}.", culture, providertype);

				Debug.WriteLine("Updater: serialised data: " + submission);

				using (var client = new ViewerWebClient())
				{
					var apiUri = new Uri(kcvApiUrl + "submit/" + providertype.ToString().ToLower());
					client.Encoding = Encoding.UTF8;
					client.Headers.Add("Content-Type", "application/json");
					try
					{
						response = client.UploadString(apiUri, "POST", submission);
					}
					catch
					{
						Debug.WriteLine("Updater: couldn't talk to the API.");
						continue;
					}
				}
				// Parse the result and clean up here.
				Debug.WriteLine("Updater: received <" + response + "> from the server.");

				// Attempt to parse the response.
				SubmitUnknownApiResponse apiresponse;
				try { apiresponse = JsonConvert.DeserializeObject<SubmitUnknownApiResponse>(response);}
				catch { Debug.WriteLine("Updater: response could not be handled using the API."); continue; }

				if (!apiresponse.success) { Debug.WriteLine("Updater: API returned an error: "+ apiresponse.verbose + "."); return;}

				// Set already submitted flag on successful submission.
				foreach (var submitstring in submitstringgroup)
				{
					unknownStrings.First(s => (s.Culture == culture) && (s.ProviderType == providertype) && (s.Equals(submitstring))).AlreadySubmitted = true;
				}
			}
		}

		/// <summary>
		/// Returns a link to where the user can download a new version of KCV.
		/// </summary>
		/// <returns>KCV download URL.</returns>
		public string GetDownloadUrl()
		{
			return downloadUrl;
		}

		/// <summary>
		/// Automatic update checks: timer handler.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event arguments.</param>
		private void dispatcherTimerHandler(object sender, EventArgs e)
		{
			this.CheckForUpdates();
			this.UpdateAsNeeded();
		}

		private void autosubmitTimerHandler(object sender, EventArgs e)
		{
			this.SubmitUnknown();
		}

		private class UnknownStringItem
		{
			public TranslationProviderType ProviderType { get; }
			public string Culture { get; }
			public object RawData { get; }
			public bool AlreadySubmitted { get; set; }

			public string Identifier
			{
				get
				{
					string identifier = null;
					try
					{
						switch (ProviderType)
						{
							case TranslationProviderType.Ships:
								identifier = (RawData as kcsapi_mst_ship).api_name;
								break;
							case TranslationProviderType.ShipTypes:
								identifier = (RawData as kcsapi_mst_stype).api_id.ToString();
								break;
							case TranslationProviderType.Equipment:
								identifier = (RawData as kcsapi_mst_slotitem).api_name;
								break;
							case TranslationProviderType.Quests:
								identifier = (RawData as kcsapi_quest).api_no.ToString();
								break;
							case TranslationProviderType.Operations:
								identifier = (RawData as kcsapi_battleresult).api_quest_name + " && " + (RawData as kcsapi_battleresult).api_enemy_info.api_deck_name;
								break;
							case TranslationProviderType.Expeditions:
								break;
							default:
								return null;
						}
					}
					catch
					{
						return null;
					}
					return identifier;
				}
			}

			public UnknownStringItem(TranslationProviderType type, string culture, object rawdata)
			{
				ProviderType = type;
				Culture = culture;
				RawData = rawdata;
				AlreadySubmitted = false;
			}

			public override bool Equals(System.Object obj)
			{
				bool check = false;

				if (obj == null) return false;

				UnknownStringItem p = obj as UnknownStringItem;
				if ((System.Object)p == null) return false;

				try
				{
					switch (ProviderType)
					{
						case TranslationProviderType.Ships:
							var ship1 = (p.RawData as kcsapi_mst_ship);
							var ship2 = (RawData as kcsapi_mst_ship);
							check = (ship1.api_name == ship2.api_name);
							break;
						case TranslationProviderType.ShipTypes:
							var stype1 = (p.RawData as kcsapi_mst_stype);
							var stype2 = (RawData as kcsapi_mst_stype);
							check = (stype1.api_id == stype2.api_id);
							break;
						case TranslationProviderType.Equipment:
							var item1 = (p.RawData as kcsapi_mst_slotitem);
							var item2 = (RawData as kcsapi_mst_slotitem);
							check = (item1.api_id == item2.api_id);
							break;
						case TranslationProviderType.Quests:
							var quest1 = (p.RawData as kcsapi_quest);
							var quest2 = (RawData as kcsapi_quest);
							check = (quest1.api_no == quest2.api_no);
							break;
						case TranslationProviderType.Operations:
							var operation1 = (p.RawData as kcsapi_battleresult);
							var operation2 = (RawData as kcsapi_battleresult);
							check = (operation1.api_quest_name == operation2.api_quest_name) && (operation1.api_enemy_info.api_deck_name == operation2.api_enemy_info.api_deck_name);
							break;
						case TranslationProviderType.Expeditions:
							break;
						default:
							return false;
					}
				}
				catch
				{
					return false;
				}

				return (ProviderType == p.ProviderType) && (Culture == p.Culture) && check;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int hash = (int)2166136261;
					hash = hash * 16777619 ^ (Culture?.GetHashCode() ?? 17);
					hash = hash * 16777619 ^ (ProviderType.GetHashCode());
					hash = hash * 16777619 ^ RawDataHashCode();
					return hash;
				}
			}

			private int RawDataHashCode()
			{
				int hash = (int)486187739;
				unchecked
				{
					try
					{
						switch (ProviderType)
						{
							case TranslationProviderType.Ships:
								hash = hash ^ (RawData as kcsapi_mst_ship).api_name.GetHashCode();
								break;
							case TranslationProviderType.ShipTypes:
								hash = hash ^ (RawData as kcsapi_mst_stype).api_id.GetHashCode();
								break;
							case TranslationProviderType.Equipment:
								hash = hash ^ (RawData as kcsapi_mst_slotitem).api_id.GetHashCode();
								break;
							case TranslationProviderType.Quests:
								hash = hash ^ (RawData as kcsapi_quest).api_no.GetHashCode();
								break;
							case TranslationProviderType.Operations:
								hash = hash * 16777619 ^ (RawData as kcsapi_battleresult).api_quest_name.GetHashCode();
								hash = hash * 16777619 ^ (RawData as kcsapi_battleresult).api_enemy_info.api_deck_name.GetHashCode();
								break;
							//case TranslationProviderType.Expeditions:
							//	break;
							default:
								return hash ^ 17;
						}
					}
					catch
					{
						return hash ^ 17;
					}
					return hash;
				}
			}
		}

		private class UnknownStringsList : HashSet<UnknownStringItem> { }

		[DataContract]
		private class SubmitUnknownApi
		{
			// ReSharper disable InconsistentNaming
			[DataMember]
			private string culture { get; }

			[DataMember]
			private List<object> rawdata { get; }

			[DataMember]
			private string api_version => "1";

			// ReSharper enable InconsistentNaming

			public SubmitUnknownApi(string c, List<object> o)
			{
				culture = c;
				rawdata = o;
			}
		}

		[DataContract]
		private class SubmitUnknownApiResponse
		{
			[DataMember]
			public bool success { get; set; }
			[DataMember]
			public string verbose { get; set; }
			[DataMember]
			public List<string> added { get; set; }
		}

#region Converters

		/// <summary>
		/// Deserialises JSON.
		/// </summary>
		/// <typeparam name="T">Type for deserialisation.</typeparam>
		/// <param name="bytes">JSON byte stream.</param>
		/// <returns></returns>
		private T ConvertTo<T>(byte[] bytes)
		{
			var serialiser = new DataContractJsonSerializer(typeof(T));
			using (var stream = new MemoryStream(bytes))
			{
				return (T)serialiser.ReadObject(stream);
			}
		}

		/// <summary>
		/// Attempts to deserialise JSON.
		/// </summary>
		/// <typeparam name="T">Type for deserialisation.</typeparam>
		/// <param name="bytes">JSON byte stream.</param>
		/// <param name="result">True if successful, false if not.</param>
		/// <returns></returns>
		private bool TryConvertTo<T>(byte[] bytes, out T result)
		{
			try
			{
				result = this.ConvertTo<T>(bytes);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				result = default(T);
				return false;
			}
			return true;
		}

#endregion

		/// <summary>
		/// Builds default API request parameters.
		/// </summary>
		/// <returns>A collection of parameters required to query the KCV API.</returns>
		private NameValueCollection DefaultRequestParameters() => new NameValueCollection { { "api", apiVersion }, { "culture", CurrentCulture } };
	}
}
