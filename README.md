KanColleViewer!
--

[![Release](https://img.shields.io/github/release/Yuubari/KanColleViewer.svg?style=flat-square)](https://github.com/Yuubari/KanColleViewer/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/Yuubari/KanColleViewer/latest/total.svg?style=flat-square)](https://github.com/Yuubari/KanColleViewer/releases/latest)
[![License](https://img.shields.io/github/license/Yuubari/KanColleViewer.svg?style=flat-square)](https://github.com/Yuubari/KanColleViewer/blob/develop/LICENSE.txt)


KanColleViewer! is a Windows-only browser tool that provides a more informative interface for DMM.com's Kantai Collection ~KanColle~ browser game.

### About This Project

KanColleViewer! uses the Microsoft Internet Explorer components in WPF, WebBrowser, to display the game; [Nekoxy](https://github.com/veigr/Nekoxy) is used to capture communications between the game's Flash client and the DMM servers.
Therefore, from the game servers' point of view, using KCV is basically the same as using IE.
**Of course KanColleViewer! does not modify any game data nor implements any sort of macro cheating.**

This project is a localisation of [@Grabacr07](https://twitter.com/Grabacr07)'s [original application](http://grabacr.net/kancolleviewer) maintained by [@Xiatian](https://twitter.com/Xiatian). The features added in this fork are based on previous work done by [@Zharay](http://twitter.com/Zharay), [@silfumus](http://twitter.com/silfumus) and [@southro_p](https://twitter.com/southro_p); this particular fork, however, does not carry over any legacy code from Zharay's 2.x builds, much unlike my previous 3.x branch, and is a complete re-implementation.

### Features

* Real-time display for instant repair, instant construction, and other materials
* Real-time counters for shipgirls and equipment
* Fleet status, including equipment in use
* Complete list of all shipgirls stationed at your naval base with various filters
* Complete list of your equipment (including which shipgirls have it equipped) grouped by type and by upgrade and proficiency levels
* Notifications for morale recovery
* Repair and construction docks status, including notifications on repair and construction completion
* Quests display, showing both currently active quests and available daily and weekly quests
* Expedition status display, including notifications on expedition completion
* One-click screenshotting
* And more!




### System Requirements

* Windows 8 or later
* Windows 7

The original developer, ([@Grabacr07](https://twitter.com/Grabacr07)), uses Windows 8.1 Pro to build and test this application.
The toast notification system from Windows 8 is not supported on Windows 7 (although you will still get the good old tray icon notifications). It's recommended to run KCV in Windows 8.

* [.NET Framework 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=30653)

Windows 7 requires that you install .NET Framework 4.5.
Windows 8 comes with it pre-installed.

KCV uses IE to display the game's web page, so it depends on IE settings. If you're experiencing issues accessing the game, please verify that Flash works in Internet Explorer. It's also recommended to install the latest version of [Adobe Flash Player](https://get.adobe.com/flashplayer/).

KCV does not perform Flash extraction and instead provides a viewport of sorts to display the game's 800x480 frame.



### Development Environment and Language

Developed in C# + WPF on Windows 8.1 Pro using Visual Studio Enterprise 2015.

### License

* [The MIT License (MIT)](LICENSE.txt)

Released under the MIT License as open source software.

### Libraries Used

The following libraries are used in this project:

#### [JSON.NET](http://www.newtonsoft.com/json)

* **Used for:** JSON serialisation and deserialisation
* **License:** The MIT License (MIT)
* **License, full text:** [licenses/JSON.NET.md](licenses/JSON.NET.md)

#### [DynamicJson](http://dynamicjson.codeplex.com/)

> DynamicJson  
> ver 1.2.0.0 (May. 21th, 2010)
>
> created and maintained by neuecc <ils@neue.cc>  
> licensed under Microsoft Public License(Ms-PL)  
> http://neue.cc/  
> http://dynamicjson.codeplex.com/

* **Used for:** JSON deserialisation
* **License:** Ms-PL
* **License, full text:** [licenses/Ms-PL.txt](licenses/Ms-PL.txt)

#### [Livet](http://ugaya40.hateblo.jp/entry/Livet)

* **Used for:** MVVM (Model/View/ViewModel) infrastructure pattern
* **License:** zlib/libpng

#### [StatefulModel](http://ugaya40.hateblo.jp/entry/StatefulModel)

> The MIT License (MIT)
>
> Copyright (c) 2015 Masanori Onoue

* **Used for:** M-V-Whatever model infrastructure
* **License:** The MIT License (MIT)
* **License, full text:** [licenses/StatefulModel.txt](licenses/StatefulModel.txt)

#### [Nekoxy](https://github.com/veigr/Nekoxy)

> The MIT License (MIT)
>
> Copyright (c) 2015 veigr

* **Used for:** HTTP traffic capture
* **License:** The MIT License (MIT)
* **License, full text:** [licenses/Nekoxy.txt](licenses/Nekoxy.txt)

#### [TrotiNet](https://github.com/krys-g/TrotiNet)

> TrotiNet is a proxy library implemented in C#. It aims at delivering a simple,  
> reusable framework for developing any sort of C# proxies.
>
> TrotiNet is distributed under the GNU Lesser General Public License v3.0  
> (LGPL). See: http://www.gnu.org/licenses/lgpl.html

* **Used for:** Local HTTP proxy
* **License:** GNU LESSER GENERAL PUBLIC LICENSE Version 3
* **License, full text:** [licenses/LGPL.txt](licenses/LGPL.txt) , [licenses/GPL.txt](licenses/GPL.txt)
* **Source code:** [externals/TrotiNet-master.zip](externals/TrotiNet-master.zip)

#### [Apache log4net](https://logging.apache.org/log4net/)

* **Used for:** TrotiNet dependency (log output; unused)
* **License:** Apache License Version 2.0
* **License, full text:** [licenses/Apache.txt](licenses/Apache.txt)

#### [Rx (Reactive Extensions)](https://rx.codeplex.com/)

* **Used for:** Asynchronous processing
* **License:** Apache License Version 2.0
* **License, full text:** [licenses/Apache.txt](licenses/Apache.txt)

#### [Desktop Toast](https://github.com/emoacht/DesktopToast)

> The MIT License (MIT)
>
> Copyright (c) 2014-2015 EMO

* **Used for:** Toast notifications
* **License:** The MIT License (MIT)
* **License, full text:** [licenses/DesktopToast.txt](licenses/DesktopToast.txt)

#### [.NET Core Audio APIs](https://netcoreaudio.codeplex.com/)

> The MIT License (MIT)
>
> Copyright (c) 2011 Vannatech

* **Used for:** Audio volume control
* **License:** The MIT License (MIT)
* **License, full text:** [licenses/NETCoreAudioAPIs.txt](licenses/NETCoreAudioAPIs.txt)

### Acknowledgements and Credits

#### Development
* [@Grabacr07](https://twitter.com/Grabacr07) and [@veigr](https://twitter.com/veigr) — original application
* [@southro_p](https://twitter.com/southro_p) — initial localisation implementation
* [@silfumus](http://twitter.com/silfumus) — continued localisation, translations support
* [@Zharay](http://twitter.com/Zharay) — continued localisation and translations support

#### Translations

##### English
* [@silfumus](http://twitter.com/silfumus)
* [@Zharay](http://twitter.com/Zharay)
* [@southro_p](https://twitter.com/southro_p)
* [gakada](https://github.com/gakada/)

##### German
* [@xshunin](https://twitter.com/xshunin)
* [@hawcy](https://twitter.com/halcy)
