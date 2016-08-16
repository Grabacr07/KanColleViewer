제독업무도 바빠！ (KanColleViewer)
--

[![Build status (master)](https://img.shields.io/appveyor/ci/Grabacr07/KanColleViewer.svg?style=flat-square)](https://ci.appveyor.com/project/Grabacr07/kancolleviewer)
[![Release](https://img.shields.io/github/release/CirnoV/KanColleViewer.svg?style=flat-square)](https://github.com/CirnoV/KanColleViewer/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/CirnoV/KanColleViewer/latest/total.svg?style=flat-square)](https://github.com/CirnoV/KanColleViewer/releases/latest)
[![NuGet (KanColleWrapper)](https://img.shields.io/nuget/v/KanColleWrapper.svg?style=flat-square)](https://www.nuget.org/packages/KanColleWrapper/)
[![License](https://img.shields.io/github/license/Grabacr07/KanColleViewer.svg?style=flat-square)](https://github.com/Grabacr07/KanColleViewer/blob/develop/LICENSE.txt)


제독업무도 바빠！ (KanColleViewer)는, DMM.com이 제공하는 웹게임 "함대컬렉션 ~칸코레~"를 보다 즐기기 쉽게 하기 위한 툴입니다.

자세한 내용은, [일본어 페이지](http://grabacr.net/kancolleviewer), [한국어 페이지](http://kcvkr.tistory.com/)를 참고해주시기 바랍니다.




### 이 프로젝트에 대해

IE 컴포넌트(WPF의 WebBrowser컨트롤) 위에서 칸코레를 표시하고, [Nekoxy](https://github.com/veigr/Nekoxy)로 통신내용을 캡쳐하고 있습니다.
칸코레의 동작은, Internet Explorer에서 작동하는 것과 동일합니다.
**당연하지만, 통신내용의 변경이나, DMM/칸코레의 서버에 대한 정보의 전송 등은(매크로/치트행위) 일체 제공하지 않습니다.**


### 주요한 기능

* 고속수복재와 고속건조재(게임 내에서 확인이 힘든 것)의 실시간 표시
* 소속중인 칸무스의 수, 보유중인 장비의 수 실시간 표시
* 함대와, 함대에 속한 칸무스 목록을 나열
* 장비와, 장비를 장착한 칸무스의 목록을 나열
* 컨디션이 회복되어 함대가 출격 가능 상태가 된 때에 토스트 알림 메시지
* 입거독/건조독의 사용 현황과, 수리/건조 완료시에 토스트 알림 메시지
* 현재 수행중인 임무의 나열과 남아있는 일일/주간 임무 목록을 표시
* 원정의 현황과 종료시에 토스트 알림 메시지
* 스크린샷 저장
* 음소거



### 동작환경

* Windows 8 이상
* Windows 7

개발자([@Grabacr07](https://twitter.com/Grabacr07))는 Windows 8.1 Pro 에서만 동작 확인을 하고 있습니다.
Windows 7 에서는, 원정이나 건조가 완료될 때의 토스트 알림 메시지가 작동하지 않습니다. (대신 알림 영역에서 풍선 알림이 표시됩니다.) Windows 8 이상에서의 사용을 권장합니다.

* [.NET Framework 4.5](http://www.microsoft.com/ja-jp/download/details.aspx?id=30653)

Windows 7 에서 사용할 때에는, .NET Framework 4.5의 설치가 필요합니다.
Windows 8 이상에서는 시스템에 기본적으로 설치되어 있습니다.

IE 컴포넌트를 사용하고 있으며, 브라우저 부분은 Internet Explorer의 설정에 따라 달라집니다. 또, 게임이 정확하게 표시되지 않는 등의 현상이 발생한 경우에는, IE의 설정이나, IE에서 Flash를 볼 수 있는지 여부를 확인하시기 바랍니다.

또, 칸코레 게임부분의 사이즈 (800 x 480)과 Internet Explorer(WebBrowser 컨트롤)의 사이즈를 딱 맞게 표시하고 있을 뿐이며, Flash 추출 등의 행위도 하지 않습니다.



### 개발환경/언어

C# + WPF로 개발하고 있습니다. 개발환경은 Windows 8.1 Pro + Visual Studio Enterprise 2015 입니다.

### 라이센스

* [The MIT License (MIT)](LICENSE.txt)

MIT 라이센스 하에 공개하는 오픈소스/프리소프트웨어입니다.

### 사용 라이브러리

아래 라이브러리를 사용하고 있습니다.

#### [DynamicJson](http://dynamicjson.codeplex.com/)

> DynamicJson  
> ver 1.2.0.0 (May. 21th, 2010)
>
> created and maintained by neuecc <ils@neue.cc>  
> licensed under Microsoft Public License(Ms-PL)  
> http://neue.cc/  
> http://dynamicjson.codeplex.com/

* **용도 :** JSON 직렬화(Deserialize)
* **라이센스 :** Ms-PL
* **라이센스 전문 :** [licenses/Ms-PL.txt](licenses/Ms-PL.txt)

#### [Livet](http://ugaya40.hateblo.jp/entry/Livet)

* **용도 :** MVVM(Model/View/ViewModel)패턴용 인프라 스트럭쳐
* **라이센스 :** zlib/libpng

#### [StatefulModel](http://ugaya40.hateblo.jp/entry/StatefulModel)

> The MIT License (MIT)
>
> Copyright (c) 2015 Masanori Onoue

* **용도 :** M-V-Whatever의 Model을 위한 인프라 스트럭쳐
* **라이센스 :** The MIT License (MIT)
* **라이센스 전문 :** [licenses/StatefulModel.txt](licenses/StatefulModel.txt)

#### [Nekoxy](https://github.com/veigr/Nekoxy)

> The MIT License (MIT)
>
> Copyright (c) 2015 veigr

* **용도 :** HTTP 통신 캡쳐
* **라이센스 :** The MIT License (MIT)
* **라이센스 전문 :** [licenses/Nekoxy.txt](licenses/Nekoxy.txt)

#### [TrotiNet](https://github.com/krys-g/TrotiNet)

> TrotiNet is a proxy library implemented in C#. It aims at delivering a simple,  
> reusable framework for developing any sort of C# proxies.
>
> TrotiNet is distributed under the GNU Lesser General Public License v3.0  
> (LGPL). See: http://www.gnu.org/licenses/lgpl.html

* **용도 :** 로컬 HTTP Proxy
* **라이센스 :** GNU LESSER GENERAL PUBLIC LICENSE Version 3
* **라이센스 전문 :** [licenses/LGPL.txt](licenses/LGPL.txt) , [licenses/GPL.txt](licenses/GPL.txt)
* **소스코드 :** [externals/TrotiNet-master.zip](externals/TrotiNet-master.zip)

#### [Apache log4net](https://logging.apache.org/log4net/)

* **용도 :** TrotiNet의 의존 라이브러리 (로그 출력용/미사용)
* **라이센스 :** Apache License Version 2.0
* **라이센스 전문 :** [licenses/Apache.txt](licenses/Apache.txt)

#### [Rx (Reactive Extensions)](https://rx.codeplex.com/)

* **용도 :** 비동기 처리
* **라이센스 :** Apache License Version 2.0
* **라이센스 전문 :** [licenses/Apache.txt](licenses/Apache.txt)

#### [Desktop Toast](https://github.com/emoacht/DesktopToast)

> The MIT License (MIT)
>
> Copyright (c) 2014-2015 EMO

* **용도 :** 토스트 알림
* **라이센스 :** The MIT License (MIT)
* **라이센스 전문 :** [licenses/DesktopToast.txt](licenses/DesktopToast.txt)

#### [.NET Core Audio APIs](https://netcoreaudio.codeplex.com/)

> The MIT License (MIT)
>
> Copyright (c) 2011 Vannatech

* **용도 :** 볼륨 조작
* **라이센스 :** The MIT License (MIT)
* **라이센스 전문 :** [licenses/NETCoreAudioAPIs.txt](licenses/NETCoreAudioAPIs.txt)
