# MiMU
MiMU�� �ֽ� ������ ����ũ����Ʈ�� ����ϴ� �ұԸ� ��� ������ ���� ����� ���������Դϴ�.

## �䱸����
- nginx

## ��ġ
### ����� ���� ���� ����
����� ������ zip ��Ű�� ���Ϸ�, ������ϵ鸸�� �����ؾ� �մϴ�.
�� ������ϵ��� `modsYYYYMMDDNNN.zip` (NNN�� �� ��¥�� ���° ������Ʈ�� ��Ÿ���ϴ�.) ������ ����� �ϸ�, nginx �ڵ� html index�� �̿��Ͽ� ���� ����� ǥ���ؾ� �մϴ�.

### ini���� ����
`mimu.example.ini` ������ �����Ͽ� MiMU �������Ͽ� �ִ� ���� `mimu.ini`�� �̸����� �����ؾ� �մϴ�. `mimu.ini` ���� ���ô� ������ �����ϴ�.
```
[Server]
; ����� ���� ����
ModPackUpdateUrl=https://someserver.com/MinecraftFiles/mods
; ���� ���丮 �ڿ� �ٴ� ���ڿ�
GameDirectorySuffix=someserver
; ���� ���������� "���̴�" �̸� (���Ŀ��� �����ϴ� �� �������� �̸��Դϴ�)
LauncherProfileName=An awesome server
; ���� �������ϸ� �����ϴ� �� ���������� ���̴� ID. "���̴�" �̸��� ���Ƶ� �� ID�� �ٸ��� �ٸ� �������Ϸ� �������ϴ�, �Ƹ���.
LauncherProfileInternalId=someserver
; ����ũ����Ʈ ����
MinecraftVersion=1.10.2
; �������� �ּ�
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