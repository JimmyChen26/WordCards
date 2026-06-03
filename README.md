# WordCards 單字卡程式

## 專案簡介

WordCards 是一個使用 C# Windows Forms 製作的單字卡程式。

程式可以讀取 `WordCards.txt` 單字資料檔，將單字顯示在左側清單中，使用者可以點選單字查看單字、音標與解釋，並播放對應的 mp3 發音檔。

目前程式也支援修改單字資料，修改後會自動儲存回 `WordCards.txt`。




<img width="1085" height="533" alt="螢幕擷取畫面 2026-06-03 110624" src="https://github.com/user-attachments/assets/460b6f48-375d-4529-b584-43f7786b1669" />



---

## 功能特色

- 讀取 `WordCards.txt` 單字資料
- 顯示單字清單
- 顯示單字、音標、解釋
- 播放單字發音 mp3
- 點選單字即可顯示並播放
- 自動播放單字
- Enter 切換到下一個單字
- Space 重播目前單字
- 修改目前選取的單字
- 儲存修改後的單字資料

---

## 開發環境

- Visual Studio
- C#
- Windows Forms App
- .NET Framework
- Windows Media Player COM 元件

---

## 專案檔案結構

```text
WordCards
├── Program.cs
├── frmWordCards.cs
├── frmWordCards.Designer.cs
├── frmEditWord.cs
├── frmEditWord.Designer.cs
├── WordCards.txt
└── Sound
    └── A
        ├── abacus.mp3
        ├── abandon.mp3
        └── ...
