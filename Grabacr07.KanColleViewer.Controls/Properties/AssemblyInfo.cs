using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("KanColleViewer.Controls")]
[assembly: AssemblyCompany("grabacr.net")]
[assembly: AssemblyProduct("KanColleViewer.Controls")]
[assembly: AssemblyDescription("UI controls for KanColleViewer and plugins.")]
[assembly: AssemblyCopyright("Copyright © 2015 Grabacr07")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

//ローカライズ可能なアプリケーションのビルドを開始するには、
//.csproj ファイルの <UICulture>CultureYouAreCodingWith</UICulture> を
//<PropertyGroup> 内部で設定します。たとえば、
//ソース ファイルで英語を使用している場合、<UICulture> を en-US に設定します。次に、
//下の NeutralResourceLanguage 属性のコメントを解除します。下の行の "en-US" を
//プロジェクト ファイルの UICulture 設定と一致するよう更新します。

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
	// テーマ固有のリソース ディクショナリが置かれている場所
	// (リソースがページ、またはアプリケーション リソース ディクショナリに見つからない場合に使用されます)
	ResourceDictionaryLocation.None,

	// 汎用リソース ディクショナリが置かれている場所
	// (リソースがページ、アプリケーション、またはいずれのテーマ固有のリソース ディクショナリにも見つからない場合に使用されます)
	ResourceDictionaryLocation.SourceAssembly
)]

[assembly: XmlnsDefinition("http://schemes.grabacr.net/winfx/2015/kancolleviewer/controls", "Grabacr07.KanColleViewer.Controls")]
[assembly: XmlnsDefinition("http://schemes.grabacr.net/winfx/2015/kancolleviewer/converters", "Grabacr07.KanColleViewer.Converters")]
[assembly: XmlnsDefinition("http://schemes.grabacr.net/winfx/2015/kancolleviewer/interactivity", "Grabacr07.KanColleViewer.Interactivity")]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// すべての値を指定するか、下のように '*' を使ってビルドおよびリビジョン番号を 
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.1.0")]
