using System;
using System.Collections.Generic;
using System.IO;
using Grabacr07.KanColleViewer.Models.Settings;
using MetroTrilithon.Serialization;

namespace Logger.Models
{
	public class LoggerSettings : SettingsHost
	{
		public const string OldStyleTimestampFormat = "yyyy-MM-dd HH:mm:ss";

		public SerializableProperty<string> Location
			=> this.Cache(key => new SerializableProperty<string>(key, Providers.Roaming, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KanColleViewer!")) { AutoSave = true });

		public SerializableProperty<string> DateTimeFormat
			=> this.Cache(key => new SerializableProperty<string>(key, Providers.Roaming, OldStyleTimestampFormat) { AutoSave = true });

		public SerializableProperty<bool> DateTimeUseJapanese
			=> this.Cache(key => new SerializableProperty<bool>(key, Providers.Roaming, true) { AutoSave = true });

		public Dictionary<string, SerializableProperty<bool>> LoggersEnabled = new Dictionary<string, SerializableProperty<bool>>();
		public Dictionary<string, SerializableProperty<string>> LoggersFormats = new Dictionary<string, SerializableProperty<string>>();

		public LoggerSettings(IEnumerable<LoggerBase> loggers)
		{
			foreach (var logger in loggers)
			{
				LoggersEnabled.Add(logger.LoggerName, this.Cache(key => new SerializableProperty<bool>(LoggerEnabledPropertyName(logger.LoggerName), Providers.Roaming, true) { AutoSave = true }, LoggerEnabledPropertyName(logger.LoggerName)));
				LoggersFormats.Add(logger.LoggerName, this.Cache(key => new SerializableProperty<string>(LoggerFormatPropertyName(logger.LoggerName), Providers.Roaming, logger.DefaultFormat) {AutoSave = true }, LoggerFormatPropertyName(logger.LoggerName)));
			}
		}

		public static string LoggerEnabledPropertyName(string type)
			=> nameof(LoggerSettings) + ".Loggers." + type + ".Enabled";
		public static string LoggerFormatPropertyName(string type)
			=> nameof(LoggerSettings) + ".Loggers." + type + ".Format";
	}
}