﻿제독업무도 바빠！ (KanColleViewer)
--

본 프로그램은 칸코레 전용 브라우저같은 용도로 사용됩니다.
함대 컬렉션~칸코레~를 더 쉽고 재미있게 플레이하기 위해 사용되는 툴입니다.
해당 버전은 기존 오리지널 칸코레 뷰어에다가 한국 유저들이 좀 더 쉽게 칸코레를 즐기기 위한 기능들을 추가한 버전입니다.

사용 설명 및 기타 내용은 [이 페이지](http://kancolleviewerkr.tk) 에서 확인할 수 있습니다.


### 이 프로젝트에 대해서

WPF WebBrowser 컨트롤 (IE구성요소) 를 기반으로、[FiddlerCore](http://fiddler2.com/fiddlercore) 를 이용하여 통신내역을 캡쳐합니다.

당연하지만 통신내역의 변경, 칸코레서버에 대한 정보의 전송 등은 하지 않습니다.

### 주요기능
* 고속수복제 및 고속건조제 (게임 내에서 확인 하기 어려운 것들) 의 실시간 표시
* 사령부에 소속된 칸무스 수, 보유하고 있는 장비수의 실시간 표시
* 함대와 함대에 속해있는 칸무스 목록 보기
* 입거도크,건조도크의 사용과 입거 ​​· 건조 종료시 알림 메시지
* 현재 수행중인 임무를 나열하고 남아있는 일일 / 주간 임무 목록 보기
* 원정상황과 종료시 알림 메시지
* 스크린샷 저장
* 음소거

### 해당 한글화 버전 주요기능

* 입거, 대파경고, 원정 알림음을 원하는 소리로 설정가능(CustomSound.cs)
* 경험치 계산기 제공(CalExp)
* 칸무스 이름, 장비 이름, 퀘스트에 대한 한글 번역 지원
* 원정 리스트 지원
* 쿠키 조작기능을 통한 해외지역(일본외)에서의 칸코레 접속
* 업데이트 알림 및 번역내용 자동갱신기능


### 작동환경
* Windows 8 이상
* Windows 7 (문제가 발생할 수 있음)


Windows 7은 지원하긴 하지만 개발자([@Grabacr07](https://twitter.com/Grabacr07)) 도 그렇고 지금 버전을 올리는 쪽([@FreyYa312](https://twitter.com/Freyya312))도 기능 업데이트 도중 해당 OS를 통한 별도의 테스트를 진행하지 않습니다.
 
최대한 피드백을 받아서 발생하는 문제들은 해결하고있지만 추후에 정식버전에 추가되는 기능들이 제한될 수도 있습니다.

 * [.NET Framework 4.5](http://www.microsoft.com/ko-kr/download/details.aspx?id=30653)

윈도우7의 경우 .NET Framework 4.5의 설치가 별도로 필요합니다. 윈도우8이상의 경우 별도 설치할 필요없습니다.

되도록이면 Windows 8이상 버전에서 사용을 권장합니다.


### 개발환경, 언어, 라이브러리

C # + WPF에서 개발하고 있습니다.

* [Reactive Extensions](http://rx.codeplex.com/)
* Interactive Extensions
* [Windows API Code Pack](http://archive.msdn.microsoft.com/WindowsAPICodePack)
* [Livet](http://ugaya40.net/livet) (MVVM 인프라스트럭처)
* [DynamicJson](http://dynamicjson.codeplex.com/) (일부 JSON 직렬화 처리)
* [FiddlerCore](http://fiddler2.com/fiddlercore) (네트워크 캡처)
* [NAudio](http://naudio.codeplex.com/) 알림음 송출

### 기타사항

* [Zharay](ttps://github.com/Zharay/KanColleViewer)님의 코드를 많이 참고하였습니다.
* 프로그래밍 기술이 많이 부족한 면이 있어서 직접 추가하거나 수정한 코드쪽은 매우 지저분합니다. 양해부탁드립니다.
* 해당 프로그램을 사용하다가 발생하는 문제에 대해서는 책임을 지지 않습니다.
* 버그신고나 기능추가 제안에 대한것은 Github 이슈항목이나 트위터, DC인사이드 칸코레 갤러리에서 받고있습니다.


#### 라이센스
* MIT License

MIT 라이센스하에 공개하는 오픈 소스 / 자유 소프트웨어입니다.


