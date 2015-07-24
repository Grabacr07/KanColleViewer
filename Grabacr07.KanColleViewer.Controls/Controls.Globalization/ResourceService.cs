using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Controls.Globalization
{
	/// <summary>
	/// 多言語化されたリソースへのアクセスを提供します。
	/// </summary>
	public class ResourceService : INotifyPropertyChanged
	{
		#region static members

		private static readonly ResourceService current = new ResourceService();

		public static ResourceService Current
		{
			get { return current; }
		}

		#endregion

		/// <summary>
		/// サポートされているカルチャの名前。
		/// </summary>
		private readonly string[] supportedCultureNames =
		{
			"ja", // Resources.resx
			"en",
			"zh-CN",
			"ko-KR",
		};

		/// <summary>
		/// 多言語化されたリソースを取得します。
		/// </summary>
		public Resources Resources { get; private set; }

		/// <summary>
		/// サポートされているカルチャを取得します。
		/// </summary>
		public IReadOnlyCollection<CultureInfo> SupportedCultures { get; private set; }

		private ResourceService()
		{
			this.Resources = new Resources();
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
			Resources.Culture = this.SupportedCultures.SingleOrDefault(x => x.Name == name);
			this.OnPropertyChanged("Resources");
		}


		#region PropertyChanged event

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = this.PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
