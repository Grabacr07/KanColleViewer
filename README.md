提督業も忙しい！ (KanColleViewer)
--

[![Build status (master)](https://img.shields.io/appveyor/ci/Grabacr07/KanColleViewer.svg?style=flat-square)](https://ci.appveyor.com/project/Grabacr07/kancolleviewer)
[![Release](https://img.shields.io/github/release/Grabacr07/KanColleViewer.svg?style=flat-square)](https://github.com/Grabacr07/KanColleViewer/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/Grabacr07/KanColleViewer/latest/total.svg?style=flat-square)](https://github.com/Grabacr07/KanColleViewer/releases/latest)
[![NuGet (KanColleWrapper)](https://img.shields.io/nuget/v/KanColleWrapper.svg?style=flat-square)](https://www.nuget.org/packages/KanColleWrapper/)
[![License](https://img.shields.io/github/license/Grabacr07/KanColleViewer.svg?style=flat-square)](https://github.com/Grabacr07/KanColleViewer/blob/develop/LICENSE.txt)


提督業も忙しい！ (KanColleViewer) は、DMM.com が配信しているブラウザゲーム「艦隊これくしょん ～艦これ～」をより遊びやすくするためのツールです。

詳しくは、[特設ページ](http://grabacr.net/kancolleviewer) をご覧ください。



### このプロジェクトについて

IE コンポーネント (WPF の WebBrowser コントロール) 上で艦これを表示し、[Nekoxy](https://github.com/veigr/Nekoxy) で通信内容をキャプチャしています。
艦これの動作は、Internet Explorer 上で動作しているものと同じです。
**当然ですが、通信内容の変更や、DMM/艦これのサーバーに対する情報の送信等 (マクロ・チート行為) は一切行っていません。**


### 主な機能

* 高速修復材や高速建造材 (ゲーム内で確認しにくいやつ) のリアルタイム表示
* 所属している艦娘の数、保有している装備の数のリアルタイム表示
* 艦隊と、艦隊に属する艦娘の一覧表示
* 装備と、それぞれを装備している艦娘の一覧表示
* コンディションが回復し艦隊が出撃可能になったタイミングでのトースト通知
* 入渠ドック・建造ドックの使用状況と、整備・建造終了時のトースト通知
* 現在遂行中の任務の一覧表示と、残っているデイリー/ウィークリー任務の一覧表示
* 遠征の状況と、終了時のトースト通知
* スクリーンショット保存
* ミュート



### 動作環境

* Windows 8 以降
* Windows 7

開発者 ([@Grabacr07](https://twitter.com/Grabacr07)) は Windows 8.1 Pro でのみ動作確認を行っております。
Windows 7 では、遠征や建造の終了時のトースト通知が動作しません (代わりに、タスクトレイからのバルーン通知になります)。 Windows 8 以降での使用を推奨します。

* [.NET Framework 4.5](http://www.microsoft.com/ja-jp/download/details.aspx?id=30653)

Windows 7 で使用する場合、.NET Framework 4.5 のインストールが必要です。  
Windows 8 以降の場合は標準でインストールされています。

IE コンポーネントを使用しており、ブラウザー部分は Internet Explorer の設定に依存します。 また、ゲームが正しく表示されない等の現象が発生した場合は、IE の設定や、IE 上で Flash が表示できるかどうかをご確認ください。

なお、艦これゲーム部分のサイズ (800 x 480) と Internet Explorer (WebBrowser コントロール) のサイズをぴったり合わせて表示しているだけで、Flash 抽出等も行っていません。



### 開発環境・言語

C# + WPF で開発しています。開発環境は Windows 8.1 Pro + Visual Studio Enterprise 2015 です。

### ライセンス

* [The MIT License (MIT)](LICENSE.txt)

MIT ライセンスの下で公開する、オープンソース / フリーソフトウェアです。

### 使用ライブラリ

以下のライブラリを使用しています。

#### [DynamicJson](http://dynamicjson.codeplex.com/)

> DynamicJson  
> ver 1.2.0.0 (May. 21th, 2010)
>
> created and maintained by neuecc <ils@neue.cc>  
> licensed under Microsoft Public License(Ms-PL)  
> http://neue.cc/  
> http://dynamicjson.codeplex.com/

* **用途 :** JSON デシリアライズ
* **ライセンス :** Ms-PL
* **ライセンス全文 :** [licenses/Ms-PL.txt](licenses/Ms-PL.txt)

#### [Livet](http://ugaya40.hateblo.jp/entry/Livet)

* **用途 :** MVVM(Model/View/ViewModel)パターン用インフラストラクチャ
* **ライセンス :** zlib/libpng

#### [StatefulModel](http://ugaya40.hateblo.jp/entry/StatefulModel)

> The MIT License (MIT)
>
> Copyright (c) 2015 Masanori Onoue

* **用途 :** M-V-Whatever の Model 向けインフラストラクチャ
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [licenses/StatefulModel.txt](licenses/StatefulModel.txt)

#### [Nekoxy](https://github.com/veigr/Nekoxy)

> The MIT License (MIT)
>
> Copyright (c) 2015 veigr

* **用途 :** HTTP通信キャプチャ
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [licenses/Nekoxy.txt](licenses/Nekoxy.txt)

#### [TrotiNet](https://github.com/krys-g/TrotiNet)

> TrotiNet is a proxy library implemented in C#. It aims at delivering a simple,  
> reusable framework for developing any sort of C# proxies.
>
> TrotiNet is distributed under the GNU Lesser General Public License v3.0  
> (LGPL). See: http://www.gnu.org/licenses/lgpl.html

* **用途 :** ローカル HTTP Proxy
* **ライセンス :** GNU LESSER GENERAL PUBLIC LICENSE Version 3
* **ライセンス全文 :** [licenses/LGPL.txt](licenses/LGPL.txt) , [licenses/GPL.txt](licenses/GPL.txt)
* **ソースコード :** [externals/TrotiNet-master.zip](externals/TrotiNet-master.zip)

#### [Apache log4net](https://logging.apache.org/log4net/)

* **用途 :** TrotiNet の依存ライブラリ (ログ出力用/未使用)
* **ライセンス :** Apache License Version 2.0
* **ライセンス全文 :** [licenses/Apache.txt](licenses/Apache.txt)

#### [Rx (Reactive Extensions)](https://rx.codeplex.com/)

* **用途 :** 非同期処理
* **ライセンス :** Apache License Version 2.0
* **ライセンス全文 :** [licenses/Apache.txt](licenses/Apache.txt)

#### [Desktop Toast](https://github.com/emoacht/DesktopToast)

> The MIT License (MIT)
>
> Copyright (c) 2014-2015 EMO

* **用途 :** トースト通知
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [licenses/DesktopToast.txt](licenses/DesktopToast.txt)

#### [.NET Core Audio APIs](https://netcoreaudio.codeplex.com/)

> The MIT License (MIT)
>
> Copyright (c) 2011 Vannatech

* **用途 :** 音量操作
* **ライセンス :** The MIT License (MIT)
* **ライセンス全文 :** [licenses/NETCoreAudioAPIs.txt](licenses/NETCoreAudioAPIs.txt)
