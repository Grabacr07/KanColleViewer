﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
	public class ProductInfo
	{
		private readonly Assembly assembly = Assembly.GetExecutingAssembly();
		private string _Title;
		private string _Description;
		private string _Company;
		private string _Product;
		private string _Copyright;
		private string _Trademark;
		private Version _Version;
		private string _VersionString;
		private IReadOnlyCollection<Library> _Libraries;

		public string Title => this._Title ?? (this._Title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyTitleAttribute))).Title);

	    public string Description => this._Description ?? (this._Description = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyDescriptionAttribute))).Description);

	    public string Company => this._Company ?? (this._Company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyCompanyAttribute))).Company);

	    public string Product => this._Product ?? (this._Product = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyProductAttribute))).Product);

	    public string Copyright => this._Copyright ?? (this._Copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyCopyrightAttribute))).Copyright);

	    public string Trademark => this._Trademark ?? (this._Trademark = ((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(this.assembly, typeof(AssemblyTrademarkAttribute))).Trademark);

	    public Version Version => this._Version ?? (this._Version = this.assembly.GetName().Version);

	    public string VersionString => this._VersionString ?? (this._VersionString = string.Format("{0}{1}{2}", this.Version.ToString(3), this.IsBetaRelease ? " β" : "", this.Version.Revision == 0 ? "" : " rev." + this.Version.Revision));

	    public bool IsBetaRelease => false;


	    public IReadOnlyCollection<Library> Libraries => this._Libraries ?? (this._Libraries = new List<Library>
	    {
	        new Library("Reactive Extensions", new Uri("http://rx.codeplex.com/")),
	        new Library("Interactive Extensions", new Uri("http://rx.codeplex.com/")),
	        new Library("Windows API Code Pack", new Uri("http://archive.msdn.microsoft.com/WindowsAPICodePack")),
	        new Library("Livet", new Uri("http://ugaya40.net/livet")),
	        new Library("DynamicJson", new Uri("http://dynamicjson.codeplex.com/")),
	        new Library("FiddlerCore", new Uri("http://fiddler2.com/fiddlercore")),
	    });
	}

	public class Library
	{
		public string Name { get; private set; }
		public Uri Url { get; private set; }

		public Library(string name, Uri url)
		{
			this.Name = name;
			this.Url = url;
		}
	}
}
