﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Grabacr07.KanColleViewer.PluginAnalyzer {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Grabacr07.KanColleViewer.PluginAnalyzer.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   {0} の Export 属性が重複しています。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string DuplicateExportAttributeMessageFormat {
            get {
                return ResourceManager.GetString("DuplicateExportAttributeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   {0} の ExportMetadata 属性が重複しています。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string DuplicateExportMetadataAttributeMessageFormat {
            get {
                return ResourceManager.GetString("DuplicateExportMetadataAttributeMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   既に他の プラグイン {0} で使用されている GUID です。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string DuplicateGuidMessageFormat {
            get {
                return ResourceManager.GetString("DuplicateGuidMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   GUID の ExportMetadata 属性が不足しています。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ExportGuidMetadataMessageFormat {
            get {
                return ResourceManager.GetString("ExportGuidMetadataMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   プラグインを実装するには、プラグイン インターフェイスの実装と、それに対応する Export 属性の指定を行う必要があります。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ExportMessageFormat {
            get {
                return ResourceManager.GetString("ExportMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   ExportMetadata 属性が不足しています。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string ExportMetadataMessageFormat {
            get {
                return ResourceManager.GetString("ExportMetadataMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   GUID の ExportMetadata 属性には、GUID と解釈できる形式で、 IPlugin インターフェイスを実装したクラスで指定した GUID と同じ値を指定する必要があります。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string InvalidGuidMetadataMessageFormat {
            get {
                return ResourceManager.GetString("InvalidGuidMetadataMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   プラグインを実装するには、IPlugin インターフェイスを実装したクラスが必要です。 に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string RequireIPluginMessageFormat {
            get {
                return ResourceManager.GetString("RequireIPluginMessageFormat", resourceCulture);
            }
        }
    }
}
