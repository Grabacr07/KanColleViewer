━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
제독업무도 바빠！ (KanColleViewer)
                                             version 4.2.6  2016/06/20
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━


■이 프로그램에 대해서

「제독업무도 바빠！」는 DMM.com에서 제공되고 있는 브라우저 게임인
「함대컬렉션 ~칸코레~」를 보다 즐기기 쉽게 하기 위한 툴입니다.


■주요 기능

・고속수복재와 고속건조재(게임 내에서 확인이 힘든 것)의 실시간 표시
・소속중인 칸무스의 수, 보유중인 장비의 수 실시간 표시
・함대와, 함대에 속한 칸무스 목록을 나열
・장비와, 장비를 장착한 칸무스의 목록을 나열
・컨디션이 회복되어 함대가 출격 가능 상태가 된 때에 토스트 알림 메시지
・입거독/건조독의 사용 현황과, 수리/건조 완료시에 토스트 알림 메시지
・현재 수행중인 임무의 나열과 남아있는 일일/주간 임무 목록을 표시
・원정의 현황과 종료시에 토스트 알림 메시지
・스크린샷 저장
・음소거


■동작환경

・Windows 10
・Windows 8
・Windows 7

개발자는 Windows 10 Pro 에서만 동작 확인을 하고 있습니다.
Windows 7 에서는, 원정이나 건조가 완료될 때의 토스트 알림 메시지가 작동하지 않습니다. (대신 알림 영역에서 풍선 알림이 표시됩니다.)
Windows 8 이상에서의 사용을 권장합니다.

・.NET Framework 4.6

Windows 8 또는 그 이전 버전에서 사용할 때에는, .NET Framework 4.6의 설치가 필요합니다.
Windows 10 에서는 시스템에 기본적으로 설치되어 있습니다.

IE 컴포넌트를 사용하고 있으며, 브라우저 부분은 Internet Explorer의 설정에 따라 달라집니다.
칸코레 본체의 업데이트 후와 같은 때에 스크립트 에러가 발생하는 경우에는, IE 의 캐시를 삭제해주시기 바랍니다.
또, 게임이 정확하게 표시되지 않는 등의 현상이 발생한 경우에는, IE의 설정이나, IE에서 Flash를 볼 수 있는지 여부를 확인하시기 바랍니다.

또, 칸코레 게임부분의 사이즈 (800 x 480)과 Internet Explorer(WebBrowser 컨트롤)의 사이즈를 딱 맞게 표시하고 있을 뿐이며, Flash 추출 등의 행위도 하지 않습니다.


■사용 조건

오픈소스/프리소프트웨어입니다. 무료로 이용할 수 있습니다.
소스코드는 MIT 라이센스 하에 GitHub 에서 공개하고 있습니다.


■사용 방법

동봉된 KanColleViewer.exe 를 실행해주시기 바랍니다.
각 화면의 설명 등은 http://grabacr.net/kancolleviewer 를 참고해주시기 바랍니다.
한국어 페이지는 http://kcvkr.tistory.com 를 참고해주시기 바랍니다.


■개발환경・언어

C# + WPF 로 개발하고 있습니다.
개발환경은 Windows 10 Pro + Visual Studio Enterprise 2015 입니다.


■사용 라이브러리

아래 라이브러리를 사용하고 있습니다.

DynamicJson
(http://dynamicjson.codeplex.com/)

    DynamicJson
    ver 1.2.0.0 (May. 21th, 2010)

    created and maintained by neuecc ils@neue.cc
    licensed under Microsoft Public License(Ms-PL)
    http://neue.cc/
    http://dynamicjson.codeplex.com/

    ・용도 : JSON 직렬화(Deserialize)
    ・라이센스 : Ms-PL
    ・라이센스전문 : Licenses/Ms-PL.txt

Livet
(http://ugaya40.hateblo.jp/entry/Livet)

    ・용도 : MVVM(Model/View/ViewModel)패턴용 인프라 스트럭쳐
    ・라이센스 : zlib/libpng

StatefulModel
(http://ugaya40.hateblo.jp/entry/StatefulModel)

    ・용도 : M-V-Whatever의 Model을 위한 인프라 스트럭쳐
    ・라이센스 : The MIT License (MIT)

Nekoxy
(https://github.com/veigr/Nekoxy)

    The MIT License (MIT)

    Copyright (c) 2015 veigr

    ・용도 : HTTP 통신 캡쳐
    ・라이센스 : The MIT License (MIT)
    ・라이센스전문 : Licenses/Nekoxy.txt

TrotiNet
(https://github.com/krys-g/TrotiNet)

    TrotiNet is a proxy library implemented in C#. It aims at delivering a simple,
    reusable framework for developing any sort of C# proxies.

    TrotiNet is distributed under the GNU Lesser General Public License v3.0
    (LGPL). See: http://www.gnu.org/licenses/lgpl.html

    ・용도 : 로컬 HTTP Proxy
    ・라이센스 : GNU LESSER GENERAL PUBLIC LICENSE Version 3
    ・라이센스전문 : Licenses/LGPL.txt , Licenses/GPL.txt
    ・ソースコード : https://github.com/Grabacr07/KanColleViewer/blob/master/externals/TrotiNet-master.zip

Apache log4net
(https://logging.apache.org/log4net/)

    ・용도 : TrotiNet의 의존 라이브러리 (로그 출력용/미사용)
    ・라이센스 : Apache License Version 2.0
    ・라이센스전문 : Licenses/Apache.txt

Rx (Reactive Extensions)
(https://rx.codeplex.com/)

    ・용도 : 비동기 처리
    ・라이센스 : Apache License Version 2.0
    ・라이센스전문 : Licenses/Apache.txt

Desktop Toast
(https://github.com/emoacht/DesktopToast)

    The MIT License (MIT)

    Copyright (c) 2014-2015 EMO

    ・용도 : 토스트 알림
    ・라이센스 : The MIT License (MIT)
    ・라이센스전문 : Licenses/DesktopToast.txt

.NET Core Audio APIs
(https://netcoreaudio.codeplex.com/)

    The MIT License (MIT)

    Copyright (c) 2011 Vannatech

    ・용도 : 볼륨 조작
    ・라이센스 : The MIT License (MIT)
    ・라이센스전문 : Licenses/NETCoreAudioAPIs.txt


■면책조항

본 소프트웨어의 사용은 전부 자기 책임으로 합니다.
이 소프트웨어를 사용하는 것으로 발생하는 모든 손해에 대해서,
개발자는 일절 책임지지 않습니다.


■업데이트 내역

2016/06/20 - version 4.2.6 릴리즈
 (잊었습니다)
2016/02/12 - version 4.2.1 릴리즈
2016/02/08 - version 4.2 릴리즈
2015/11/10 - version 4.1.6 릴리즈
2015/10/30 - version 4.1.5 릴리즈
2015/08/28 - version 4.1.3 릴리즈
2015/08/20 - version 4.1.2 릴리즈
2015/08/19 - version 4.1.1 릴리즈
2015/08/12 - version 4.1.0 릴리즈
2015/08/11 - version 4.0.1 릴리즈
2015/08/11 - version 4.0 릴리즈
2015/05/26 - version 3.8.2 릴리즈
2015/05/18 - version 3.8 릴리즈
2015/05/03 - version 3.7 릴리즈
2015/02/07 - version 3.5 릴리즈
2014/09/26 - version 3.4 릴리즈
2014/08/12 - version 3.3 릴리즈
2014/08/10 - version 3.2 릴리즈
2014/08/09 - version 3.1 릴리즈
2014/08/07 - version 3.0 릴리즈
2014/05/16 - version 2.6 릴리즈
2014/04/29 - version 2.6 beta rev.2 릴리즈
2014/04/23 - version 2.6 beta 릴리즈
2014/03/21 - version 2.4 릴리즈
2014/03/04 - version 2.3 릴리즈
2014/03/02 - version 2.2 릴리즈
2014/02/19 - version 2.1 릴리즈
2014/02/06 - version 2.0.1 릴리즈
2014/02/01 - version 2.0 릴리즈
2014/01/20 - version 1.2.1 릴리즈
2013/12/29 - version 1.2 릴리즈
2013/12/27 - version 1.1 릴리즈
2013/12/22 - version 1.0 릴리즈

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 Product name: 제독업무도 바빠！    
 Product URL:  http://grabacr.net/kancolleviewer
 Source code:  https://github.com/Grabacr07/KanColleViewer
 License:      MIT License
 Author:       @Grabacr07
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
