using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Plugins.Properties;
using Livet;

namespace Grabacr07.KanColleViewer.Plugins.Models
{
	/// <summary>
	/// 多言語化されたリソースへのアクセスを提供します。
	/// </summary>
	public class AudibleNotificationsResourceService : NotificationObject
	{
		#region static members

		public static AudibleNotificationsResourceService Current { get; } = new AudibleNotificationsResourceService();

		#endregion

		/// <summary>
		/// サポートされているカルチャの名前。
		/// </summary>
		private readonly string[] supportedCultureNames =
		{
			"en",
			"ja",
		};

		/// <summary>
		/// 多言語化されたリソースを取得します。
		/// </summary>
		public AudibleNotificationsResources Resources { get; }

		/// <summary>
		/// サポートされているカルチャを取得します。
		/// </summary>
		public IReadOnlyCollection<CultureInfo> SupportedCultures { get; }

		private AudibleNotificationsResourceService()
		{
			this.Resources = new AudibleNotificationsResources();
			this.SupportedCultures = this.supportedCultureNames
				.Select(x =>
				{
					try
					{
						return CultureInfo.GetCultureInfo(x);
					}
					catch (CultureNotFoundException)
					{
						return null;
					}
				})
				.Where(x => x != null)
				.ToList();
		}

		/// <summary>
		/// 指定されたカルチャ名を使用して、リソースのカルチャを変更します。
		/// </summary>
		/// <param name="name">カルチャの名前。</param>
		public void ChangeCulture(string name)
		{
			AudibleNotificationsResources.Culture = this.SupportedCultures.SingleOrDefault(x => x.Name == name);

			this.RaisePropertyChanged(nameof(this.Resources));
		}
	}
}
