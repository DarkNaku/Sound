# Sound

### 소개
단순한 사운드 플레이 시스템

### 설치방법
1. 패키지 관리자의 툴바에서 좌측 상단에 플러스 메뉴를 클릭합니다.
2. 추가 메뉴에서 Add package from git URL을 선택하면 텍스트 상자와 Add 버튼이 나타납니다.
3. https://github.com/DarkNaku/Sound.git?path=/Assets/Sound 입력하고 Add를 클릭합니다.

### 사용방법
* 'Tools > Sound Config'에 사용할 오디오 클립을 지정합니다.
치
```csharp
Sound.Play("Click";)
Sound.Stop("Click";)
Sound.PlayBGM("Music";)
Sound.StopBGM("Music";)
```

### 추가 하려고 계획하고 있는 기능
* 배경음악 페이드
* 랜덤 피치