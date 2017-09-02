# MiMU
MiMU는 최신 버전의 마인크래프트를 사용하는 소규모 모드 서버를 위한 모드팩 업데이터입니다.

## 요구사항
- nginx

## 설치
### 모드팩 파일 서버 설정
모드팩 파일은 zip 패키지 파일로, 모드파일들만을 포함해야 합니다.
이 모드파일들은 `modsYYYYMMDDNNN.zip` (NNN은 그 날짜의 몇번째 업데이트를 나타냅니다.) 형식을 따라야 하며, nginx 자동 html index를 이용하여 파일 목록을 표시해야 합니다.

### ini파일 설정
`mimu.example.ini` 파일을 참고하여 MiMU 실행파일에 있는 곳에 `mimu.ini`의 이름으로 존재해야 합니다. `mimu.ini` 파일 예시는 다음과 같습니다.
```
[Server]
; 모드팩 파일 서버
ModPackUpdateUrl=https://someserver.com/MinecraftFiles/mods
; 게임 디렉토리 뒤에 붙는 문자열
GameDirectorySuffix=someserver
; 런쳐 프로파일의 "보이는" 이름 (런쳐에서 설정하는 그 프로파일 이름입니다)
LauncherProfileName=An awesome server
; 런쳐 프로파일를 설정하는 데 내부적으로 쓰이는 ID. "보이는" 이름이 같아도 이 ID가 다르면 다른 프로파일로 보여집니다, 아마도.
LauncherProfileInternalId=someserver
; 마인크래프트 버전
MinecraftVersion=1.10.2
; 공지사항 주소
NoticeWebUrl=https://someserver.com/index.html
```
### LICENSE
Copyright (C) 2017 LiteHell

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.