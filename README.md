提督業も忙しい！ (KanColleViewer)
--

艦これブラウザーのようなもの。
艦隊これくしょん ～艦これ～ を遊びやすくするためのツールです。

詳しくは、[特設ページ](http://grabacr.net/kancolleviewer) をご覧ください。


### このプロジェクトについて
WPF の WebBrowser コントロール (IE コンポーネント) 上で艦これを表示し、[FiddlerCore](http://fiddler2.com/fiddlercore) で通信内容をキャプチャしています。
当然ですが、通信内容の変更や、艦これのサーバーに対する情報の送信等は一切行っていません。


### 主な機能
* 高速修復材や高速建造材 (ゲーム内で確認しにくいやつ) のリアルタイム表示
* 司令部に所属している艦娘の数、保有している装備の数のリアルタイム表示
* 艦隊と、艦隊に属する艦娘の一覧表示
* 入渠ドック・建造ドックの使用状況と、整備・建造終了時のトースト通知
* 現在遂行中の任務の一覧表示と、残っているデイリー/ウィークリー任務の一覧表示
* 遠征の状況と、終了時のトースト通知
* スクリーンショット保存
* ミュート


### 動作環境
* Windows 8 以降
* Windows 7 (機能制限あり)

開発者 ([@Grabacr07](https://twitter.com/Grabacr07)) は Windows 8.1 Pro でのみ動作確認を行っております。
Windows 7 でも起動はできますが、遠征や建造の終了時のトースト通知が動作しません。Windows 8 以降での使用を推奨します。

* [.NET Framework 4.5](http://www.microsoft.com/ja-jp/download/details.aspx?id=30653)

Windows 7 で使用する場合、.NET Framework 4.5 のインストールが必要です。  
Windows 8 以降の場合は標準でインストールされています。


### 開発環境・言語・ライブラリ
C# + WPF で開発しています。開発環境は Windows 8.1 Pro + Visual Studio Premium 2013 です。以下のライブラリを使用しています。

* [Reactive Extensions](http://rx.codeplex.com/)
* Interactive Extensions
* [Windows API Code Pack](http://archive.msdn.microsoft.com/WindowsAPICodePack)
* [Livet](http://ugaya40.net/livet) (MVVM インフラストラクチャ)
* [DynamicJson](http://dynamicjson.codeplex.com/) (一部の JSON デシリアライズ処理)
* [FiddlerCore](http://fiddler2.com/fiddlercore) (ネットワークキャプチャ)


#### ライセンス
* MIT License

MIT ライセンスの下で公開する、オープンソース / フリーソフトウェアです。

