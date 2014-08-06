提督業も忙しい！ (KanColleViewer)
--

提督業も忙しい！ (KanColleViewer) は、DMM.com が配信しているブラウザゲーム「艦隊これくしょん ～艦これ～」をより遊びやすくするためのツールです。

詳しくは、[特設ページ](http://grabacr.net/kancolleviewer) をご覧ください。



### このプロジェクトについて

IE コンポーネント (WPF の WebBrowser コントロール) 上で艦これを表示し、[FiddlerCore](http://fiddler2.com/fiddlercore) で通信内容をキャプチャしています。
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

