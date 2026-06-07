# WordCards 單字卡學習系統

WordCards 是一個使用 C# Windows Forms 製作的單字卡學習程式。  
程式可以讀取 `WordCards.txt` 單字檔，顯示單字、音標與中文解釋，並支援播放單字音檔、自動播放、鍵盤控制與單字修改功能。

本 README 依據目前提供的 `frmWordCards` 與 `frmEditWord` 程式碼整理。:contentReference[oaicite:0]{index=0}
<img width="1115" height="554" alt="螢幕擷取畫面 2026-06-07 124802" src="https://github.com/user-attachments/assets/0b0e3df3-d195-4022-ba9c-0fa6a65a1bde" />

---

## 功能特色

- 顯示單字、音標與解釋
- 從 `WordCards.txt` 載入單字資料
- 支援播放單字音檔
- 支援自動播放模式
- 支援鍵盤快捷鍵操作
- 可修改目前選取的單字
- 修改後會自動儲存回 `WordCards.txt`
- 美化後的 WinForms 介面
- 左側單字清單支援自訂繪製
- 雙擊單字可直接進入修改畫面

---

## 開發環境

- Visual Studio
- C#
- .NET Framework Windows Forms
- Windows Media Player COM 元件

---

## 專案結構

```text
WordCards/
│
├── frmWordCards.cs        # 主視窗，負責顯示單字、播放音檔、自動播放與修改入口
├── frmEditWord.cs         # 修改單字視窗
├── WordCards.txt          # 單字資料檔
└── audio files            # 單字音檔，可放在程式執行目錄下
