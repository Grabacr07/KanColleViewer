━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
提督業も忙しい！ (KanColleViewer)
                                             version 3.8.2  2015/05/26
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━


■このソフトウェアについて

「提督業も忙しい！」は、DMM.com が配信しているブラウザゲーム
「艦隊これくしょん ～艦これ～」をより遊びやすくするためのツールです。


■主な機能

・高速修復材や高速建造材 (ゲーム内で確認しにくいやつ) のリアルタイム表示
・所属している艦娘の数、保有している装備の数のリアルタイム表示
・艦隊と、艦隊に属する艦娘の一覧表示
・装備と、それぞれを装備している艦娘の一覧表示
・コンディションが回復し艦隊が出撃可能になったタイミングでのトースト通知
・入渠ドック・建造ドックの使用状況と、整備・建造終了時のトースト通知
・現在遂行中の任務の一覧表示と、残っているデイリー/ウィークリー任務の一覧表示
・遠征の状況と、終了時のトースト通知
・スクリーンショット保存
・ミュート


■動作環境

・Windows 8 以降
・Windows 7

開発者は Windows 8.1 Pro でのみ動作確認を行っております。 
Windows 7 では、遠征や建造の終了時のトースト通知が動作しません (代わりに、タスクトレイからのバルーン通知になります)。
Windows 8 以降での使用を推奨します。

・.NET Framework 4.5

Windows 7 で使用する場合、.NET Framework 4.5 のインストールが必要です。
Windows 8 以降の場合は標準でインストールされています。

IE コンポーネントを使用しており、ブラウザー部分は Internet Explorer の設定に依存します。 
艦これ本体のアップデート後などにスクリプトエラーが頻発する場合は、IE のキャッシュを削除してください。 
また、ゲームが正しく表示されない等の現象が発生した場合は、IE の設定や、IE 上で Flash が表示できるかどうかをご確認ください。

なお、艦これゲーム部分のサイズ (800 x 480) と Internet Explorer (WebBrowser control) のサイズをぴったり合わせて表示しているだけで、Flash 抽出等も行っていません。


■使用条件

オープンソース / フリーソフトウェアです。無料でご利用頂けます。  
ソースコードは、MIT ライセンスの下で GitHub にて公開しています。


■使用方法

同梱の KanColleViewer.exe を起動してください。
各画面の解説等は http://grabacr.net/kancolleviewer を参照してください。



■開発環境・言語

C# + WPF で開発しています。
開発環境は Windows 8.1 Pro + Visual Studio Enterprise 2015 です。


■使用ライブラリ

以下のライブラリを使用しています。

DynamicJson
(http://dynamicjson.codeplex.com/)

	DynamicJson
	ver 1.2.0.0 (May. 21th, 2010)

	created and maintained by neuecc ils@neue.cc
	licensed under Microsoft Public License(Ms-PL)
	http://neue.cc/
	http://dynamicjson.codeplex.com/

	・用途 : JSON デシリアライズ
	・ライセンス : Ms-PL
	・ライセンス全文 : Licenses/Ms-PL.txt

Livet
(http://ugaya40.hateblo.jp/entry/Livet)

	・用途 : MVVM(Model/View/ViewModel)パターン用インフラストラクチャ
	・ライセンス : zlib/libpng

Nekoxy
(https://github.com/veigr/Nekoxy)

	The MIT License (MIT)

	Copyright (c) 2015 veigr

	・用途 : HTTP通信キャプチャ
	・ライセンス : The MIT License (MIT)
	・ライセンス全文 : Licenses/Nekoxy.txt

TrotiNet
(https://github.com/krys-g/TrotiNet)

	TrotiNet is a proxy library implemented in C#. It aims at delivering a simple,
	reusable framework for developing any sort of C# proxies.

	TrotiNet is distributed under the GNU Lesser General Public License v3.0
	(LGPL). See: http://www.gnu.org/licenses/lgpl.html

	・用途 : ローカル HTTP Proxy
	・ライセンス : GNU LESSER GENERAL PUBLIC LICENSE Version 3
	・ライセンス全文 : Licenses/LGPL.txt , Licenses/GPL.txt
	・ソースコード : https://github.com/Grabacr07/KanColleViewer/blob/master/LibrariesSources/TrotiNet-master.zip

Apache log4net
(https://logging.apache.org/log4net/)

	・用途 : TrotiNet の依存ライブラリ (ログ出力用/未使用)
	・ライセンス : Apache License Version 2.0
	・ライセンス全文 : Licenses/Apache.txt

Rx (Reactive Extensions)
(https://rx.codeplex.com/)

	・用途 : 非同期処理
	・ライセンス : Apache License Version 2.0
	・ライセンス全文 : Licenses/Apache.txt

Desktop Toast
(https://github.com/emoacht/DesktopToast)

	The MIT License (MIT)

	Copyright (c) 2014-2015 EMO

	・用途 : トースト通知
	・ライセンス : The MIT License (MIT)
	・ライセンス全文 : Licenses/DesktopToast.txt

.NET Core Audio APIs
(https://netcoreaudio.codeplex.com/)

	The MIT License (MIT)

	Copyright (c) 2011 Vannatech

	・用途 : 音量操作
	・ライセンス : The MIT License (MIT)
	・ライセンス全文 : Licenses/NETCoreAudioAPIs.txt


■免責事項

本ソフトウェアの使用は、すべて自己責任で行ってください。
このソフトウェアを使用した結果生じた損害について、開発者は
一切責任を負いません。


■更新履歴

2015/05/26 - version 3.8.2 リリース
2015/05/18 - version 3.8 リリース
2015/05/03 - version 3.7 リリース
2015/02/07 - version 3.5 リリース
2014/09/26 - version 3.4 リリース
2014/08/12 - version 3.3 リリース
2014/08/10 - version 3.2 リリース
2014/08/09 - version 3.1 リリース
2014/08/07 - version 3.0 リリース
2014/05/16 - version 2.6 リリース
2014/04/29 - version 2.6 beta rev.2 リリース
2014/04/23 - version 2.6 beta リリース
2014/03/21 - version 2.4 リリース
2014/03/04 - version 2.3 リリース
2014/03/02 - version 2.2 リリース
2014/02/19 - version 2.1 リリース
2014/02/06 - version 2.0.1 リリース
2014/02/01 - version 2.0 リリース
2014/01/20 - version 1.2.1 リリース
2013/12/29 - version 1.2 リリース
2013/12/27 - version 1.1 リリース
2013/12/22 - version 1.0 リリース

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 Product name: 提督業も忙しい！    
 Product URL:  http://grabacr.net/kancolleviewer
 Source code:  https://github.com/Grabacr07/KanColleViewer
 License:      MIT License
 Author:       @Grabacr07
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
