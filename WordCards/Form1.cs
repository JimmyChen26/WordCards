using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WMPLib;

namespace WordCards
{
    public partial class frmWordCards : Form
    {
        /// <summary>
        /// 單字清單
        /// </summary>
        WordCollection _WordList = new WordCollection();

        /// <summary>
        /// Windows Media Player 播放器
        /// </summary>
        WindowsMediaPlayer wmp = new WindowsMediaPlayer();

        /// <summary>
        /// 單字檔名
        /// </summary>
        string strWordFile = "WordCards.txt";

        /// <summary>
        /// 是否自動播放
        /// </summary>
        bool isPlay = false;

        public frmWordCards()
        {
            InitializeComponent();

            // 避免事件沒有綁到，這裡直接用程式綁定
            this.Load -= frmWordCards_Load;
            this.Load += frmWordCards_Load;

            this.KeyPress -= frmWordCards_KeyPress;
            this.KeyPress += frmWordCards_KeyPress;

            lstWordList.Click -= lstWordList_Click;
            lstWordList.Click += lstWordList_Click;

            btnAutoPlay.Click -= btnAutoPlay_Click;
            btnAutoPlay.Click += btnAutoPlay_Click;

            btnEditWord.Click -= btnEditWord_Click;
            btnEditWord.Click += btnEditWord_Click;

            timPlayer.Tick -= timPlayer_Tick;
            timPlayer.Tick += timPlayer_Tick;
        }

        /// <summary>
        /// 顯示單字
        /// </summary>
        private void ShowWord(WordItem word)
        {
            if (word == null)
                return;

            txtWord.Text = word.Word;
            txtPhonogram.Text = word.Phonogram;
            txtExplain.Text = word.Explain;
        }

        /// <summary>
        /// 更新左邊單字清單
        /// </summary>
        private void UpdateWordList()
        {
            lstWordList.BeginUpdate();

            lstWordList.Items.Clear();

            foreach (WordItem item in _WordList)
            {
                lstWordList.Items.Add(item);
            }

            lstWordList.EndUpdate();
        }

        /// <summary>
        /// 播放單字音檔
        /// </summary>
        private void PlayWord(WordItem word)
        {
            if (word == null)
                return;

            string soundPath = word.SoundPath;

            // 如果不是完整路徑，就從程式執行目錄找音檔
            if (!Path.IsPathRooted(soundPath))
            {
                soundPath = Path.Combine(Application.StartupPath, soundPath);
            }

            if (File.Exists(soundPath))
            {
                wmp.URL = soundPath;
                wmp.settings.autoStart = false;
                wmp.settings.mute = false;
                wmp.controls.play();

                tsslMessage.Text = "播放：" + word.Word;
            }
            else
            {
                tsslMessage.Text = "找不到音效檔：" + word.SoundPath;
            }
        }

        /// <summary>
        /// 播放目前選取的單字
        /// </summary>
        private void PlaySelectedWord()
        {
            if (lstWordList.SelectedIndex < 0)
                return;

            int idx = lstWordList.SelectedIndex;

            if (idx >= 0 && idx < _WordList.Count)
            {
                ShowWord(_WordList[idx]);
                PlayWord(_WordList[idx]);
            }
        }

        /// <summary>
        /// 移到下一個單字
        /// </summary>
        private void NextWordList()
        {
            if (lstWordList.Items.Count == 0)
                return;

            lstWordList.Focus();

            if (lstWordList.SelectedIndex < 0)
            {
                lstWordList.SelectedIndex = 0;
            }
            else if (lstWordList.SelectedIndex + 1 >= lstWordList.Items.Count)
            {
                lstWordList.SelectedIndex = 0;
            }
            else
            {
                lstWordList.SelectedIndex++;
            }

            int itemHeight = lstWordList.GetItemHeight(0);

            if (itemHeight <= 0)
                return;

            int lstRows = lstWordList.Height / itemHeight;

            if (lstRows > 0 && lstWordList.SelectedIndex >= lstRows / 2)
            {
                lstWordList.TopIndex = Math.Max(0, lstWordList.SelectedIndex - lstRows / 2);
            }
        }

        /// <summary>
        /// 表單載入時，讀取 WordCards.txt
        /// </summary>
        private void frmWordCards_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            timPlayer.Interval = 2000;
            btnAutoPlay.Text = "Play";

            string wordFilePath = Path.Combine(Application.StartupPath, strWordFile);

            if (!File.Exists(wordFilePath))
            {
                MessageBox.Show(
                    "找不到單字檔\n" + wordFilePath,
                    "錯誤",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                Application.Exit();
                return;
            }

            string[] lines = File.ReadAllLines(wordFilePath, Encoding.UTF8);

            _WordList.LoadFromStringArray(lines);

            if (_WordList.Count > 0)
            {
                UpdateWordList();

                lstWordList.SelectedIndex = 0;
                ShowWord(_WordList[0]);

                tsslMessage.Text = "單字數量：" + _WordList.Count;
            }
            else
            {
                tsslMessage.Text = "單字檔沒有資料";
            }
        }

        /// <summary>
        /// 點選左邊單字清單時，顯示並播放單字
        /// </summary>
        private void lstWordList_Click(object sender, EventArgs e)
        {
            if (isPlay == true)
                btnAutoPlay.PerformClick();

            if (lstWordList.SelectedItem != null)
            {
                if (lstWordList.SelectedItem.ToString().Length != 0)
                {
                    PlaySelectedWord();
                }
            }
        }

        /// <summary>
        /// 自動播放按鈕
        /// </summary>
        private void btnAutoPlay_Click(object sender, EventArgs e)
        {
            if (_WordList.Count == 0)
                return;

            lstWordList.Focus();

            if (lstWordList.SelectedIndex < 0)
                lstWordList.SelectedIndex = 0;

            if (isPlay == false)
            {
                btnAutoPlay.Text = "Stop";
                isPlay = true;

                PlaySelectedWord();

                timPlayer.Start();
            }
            else
            {
                btnAutoPlay.Text = "Play";
                isPlay = false;

                timPlayer.Stop();
            }
        }

        /// <summary>
        /// 修改目前選取的單字
        /// </summary>
        private void btnEditWord_Click(object sender, EventArgs e)
        {
            if (lstWordList.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "請先選擇一個單字",
                    "提醒",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            // 如果正在自動播放，先停止
            if (isPlay == true)
            {
                btnAutoPlay.PerformClick();
            }

            int idx = lstWordList.SelectedIndex;

            frmEditWord edit = new frmEditWord(_WordList[idx]);
            DialogResult result = edit.ShowDialog(this);

            if (result == DialogResult.Yes)
            {
                // 更新左邊清單
                UpdateWordList();

                // 保持選在原本那個單字
                lstWordList.SelectedIndex = idx;

                // 更新主畫面顯示
                ShowWord(_WordList[idx]);

                // 儲存回 WordCards.txt
                string wordFilePath = Path.Combine(Application.StartupPath, strWordFile);
                _WordList.SaveToFile(wordFilePath);

                tsslMessage.Text = "單字已修改並儲存";
            }
        }

        /// <summary>
        /// Timer 觸發時，自動跳下一個單字
        /// </summary>
        private void timPlayer_Tick(object sender, EventArgs e)
        {
            NextWordList();
            PlaySelectedWord();
        }

        /// <summary>
        /// 鍵盤控制
        /// Enter：下一個單字
        /// Space：重播目前單字
        /// </summary>
        private void frmWordCards_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (isPlay == true)
                return;

            switch (e.KeyChar)
            {
                case (char)Keys.Return:
                    NextWordList();
                    PlaySelectedWord();
                    e.Handled = true;
                    break;

                case (char)Keys.Space:
                    if (lstWordList.SelectedIndex >= 0)
                    {
                        PlaySelectedWord();
                    }

                    e.Handled = true;
                    break;
            }
        }
    }

    /// <summary>
    /// 單字資料類別
    /// </summary>
    public class WordItem
    {
        public string Word { get; set; }
        public string Phonogram { get; set; }
        public string SoundPath { get; set; }
        public string Explain { get; set; }

        public WordItem(string str)
        {
            Word = "";
            Phonogram = "";
            SoundPath = "";
            Explain = "";

            if (string.IsNullOrWhiteSpace(str))
                return;

            string[] strLists = str.Split('\t');

            if (strLists.Length >= 3)
            {
                Word = strLists[0];
                Phonogram = strLists[1];
                SoundPath = strLists[2];

                if (strLists.Length > 3)
                {
                    Explain = string.Join(Environment.NewLine, strLists.Skip(3));
                }
            }
        }

        /// <summary>
        /// 把 WordItem 轉成 TSV 一行文字，方便寫回 WordCards.txt
        /// </summary>
        public string ToLineString()
        {
            string explainText = Explain ?? "";

            explainText = explainText
                .Replace("\r\n", "\t")
                .Replace("\n", "\t")
                .Replace("\r", "\t");

            return Word + "\t" + Phonogram + "\t" + SoundPath + "\t" + explainText;
        }

        public override string ToString()
        {
            return Word;
        }
    }

    /// <summary>
    /// 單字集合類別
    /// </summary>
    public class WordCollection : Collection<WordItem>
    {
        public void LoadFromStringArray(string[] lines)
        {
            this.Clear();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                WordItem item = new WordItem(line);

                if (!string.IsNullOrWhiteSpace(item.Word))
                {
                    this.Add(item);
                }
            }
        }

        /// <summary>
        /// 將單字集合儲存回 WordCards.txt
        /// </summary>
        public void SaveToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                foreach (WordItem item in this)
                {
                    writer.WriteLine(item.ToLineString());
                }
            }
        }
    }
}