using System;
using System.Collections.ObjectModel;
using System.Drawing;
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

        // 美化用顏色
        private readonly Color colorBackground = Color.FromArgb(248, 250, 252);
        private readonly Color colorCard = Color.FromArgb(255, 255, 255);
        private readonly Color colorPrimary = Color.FromArgb(37, 99, 235);
        private readonly Color colorPrimaryDark = Color.FromArgb(29, 78, 216);
        private readonly Color colorText = Color.FromArgb(30, 41, 59);
        private readonly Color colorSubText = Color.FromArgb(100, 116, 139);
        private readonly Color colorSelected = Color.FromArgb(219, 234, 254);
        private readonly Color colorBorder = Color.FromArgb(226, 232, 240);

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

            // 雙擊左邊單字清單，直接進入修改畫面
            lstWordList.MouseDoubleClick -= lstWordList_MouseDoubleClick;
            lstWordList.MouseDoubleClick += lstWordList_MouseDoubleClick;

            // 左邊清單自訂繪製，讓 ListBox 更好看
            lstWordList.DrawItem -= lstWordList_DrawItem;
            lstWordList.DrawItem += lstWordList_DrawItem;

            btnAutoPlay.Click -= btnAutoPlay_Click;
            btnAutoPlay.Click += btnAutoPlay_Click;

            btnEditWord.Click -= btnEditWord_Click;
            btnEditWord.Click += btnEditWord_Click;

            timPlayer.Tick -= timPlayer_Tick;
            timPlayer.Tick += timPlayer_Tick;
        }

        /// <summary>
        /// 套用畫面美化
        /// </summary>
        private void ApplyTheme()
        {
            this.BackColor = colorBackground;
            this.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Regular);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 左邊單字清單
            lstWordList.BackColor = colorCard;
            lstWordList.ForeColor = colorText;
            lstWordList.BorderStyle = BorderStyle.FixedSingle;
            lstWordList.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            lstWordList.ItemHeight = 28;
            lstWordList.DrawMode = DrawMode.OwnerDrawFixed;
            lstWordList.IntegralHeight = false;

            // 大單字
            StyleDisplayControl(
                txtWord,
                new Font("Segoe UI", 48F, FontStyle.Bold),
                colorPrimary,
                colorBackground
            );

            // 音標
            StyleDisplayControl(
                txtPhonogram,
                new Font("Segoe UI", 18F, FontStyle.Regular),
                Color.FromArgb(22, 163, 74),
                colorBackground
            );

            // 解釋
            StyleDisplayControl(
                txtExplain,
                new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular),
                colorText,
                colorBackground
            );

            // 按鈕
            StyleButton(btnAutoPlay, colorPrimary, Color.White);
            StyleButton(btnEditWord, Color.FromArgb(241, 245, 249), colorText);

            btnAutoPlay.Text = "▶ Play";
            btnEditWord.Text = "✎ 修改";

            // 狀態列
            if (tsslMessage != null)
            {
                tsslMessage.ForeColor = colorSubText;

                if (tsslMessage.GetCurrentParent() != null)
                {
                    tsslMessage.GetCurrentParent().BackColor = Color.FromArgb(241, 245, 249);
                }
            }
        }

        /// <summary>
        /// 美化顯示文字的控制項
        /// 可支援 Label / TextBox / RichTextBox
        /// </summary>
        private void StyleDisplayControl(Control ctrl, Font font, Color foreColor, Color backColor)
        {
            if (ctrl == null)
                return;

            ctrl.Font = font;
            ctrl.ForeColor = foreColor;
            ctrl.BackColor = backColor;

            if (ctrl is TextBoxBase textBox)
            {
                textBox.BorderStyle = BorderStyle.None;
                textBox.ReadOnly = true;
            }

            if (ctrl is Label label)
            {
                label.AutoSize = false;
                label.BackColor = backColor;
            }
        }

        /// <summary>
        /// 美化按鈕
        /// </summary>
        private void StyleButton(Button btn, Color backColor, Color foreColor)
        {
            if (btn == null)
                return;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;

            btn.MouseEnter -= Button_MouseEnter;
            btn.MouseEnter += Button_MouseEnter;

            btn.MouseLeave -= Button_MouseLeave;
            btn.MouseLeave += Button_MouseLeave;
        }

        /// <summary>
        /// 按鈕滑鼠移入效果
        /// </summary>
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
                return;

            if (btn == btnAutoPlay)
            {
                btn.BackColor = colorPrimaryDark;
            }
            else if (btn == btnEditWord)
            {
                btn.BackColor = Color.FromArgb(226, 232, 240);
            }
        }

        /// <summary>
        /// 按鈕滑鼠移出效果
        /// </summary>
        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
                return;

            if (btn == btnAutoPlay)
            {
                btn.BackColor = colorPrimary;
            }
            else if (btn == btnEditWord)
            {
                btn.BackColor = Color.FromArgb(241, 245, 249);
            }
        }

        /// <summary>
        /// 自訂繪製左邊單字清單
        /// </summary>
        private void lstWordList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Rectangle bounds = e.Bounds;

            Color backColor = isSelected ? colorSelected : colorCard;
            Color foreColor = isSelected ? colorPrimary : colorText;

            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, bounds);
            }

            // 選取時左邊加一條藍線
            if (isSelected)
            {
                using (SolidBrush accentBrush = new SolidBrush(colorPrimary))
                {
                    e.Graphics.FillRectangle(accentBrush, bounds.X, bounds.Y, 4, bounds.Height);
                }
            }

            string text = lstWordList.Items[e.Index].ToString();

            Rectangle textRect = new Rectangle(
                bounds.X + 10,
                bounds.Y,
                bounds.Width - 15,
                bounds.Height
            );

            TextRenderer.DrawText(
                e.Graphics,
                text,
                lstWordList.Font,
                textRect,
                foreColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis
            );

            e.DrawFocusRectangle();
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

            // 套用美化
            ApplyTheme();

            timPlayer.Interval = 2000;
            btnAutoPlay.Text = "▶ Play";

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
        /// 雙擊左邊單字清單時，跳到修改畫面
        /// </summary>
        private void lstWordList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int idx = lstWordList.IndexFromPoint(e.Location);

            // 避免雙擊空白處也觸發修改
            if (idx == ListBox.NoMatches)
                return;

            lstWordList.SelectedIndex = idx;

            // 直接使用原本「修改」按鈕的功能
            btnEditWord.PerformClick();
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
                btnAutoPlay.Text = "⏸ Stop";
                isPlay = true;

                PlaySelectedWord();

                timPlayer.Start();
            }
            else
            {
                btnAutoPlay.Text = "▶ Play";
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