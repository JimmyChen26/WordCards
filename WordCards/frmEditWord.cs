using System;
using System.Drawing;
using System.Windows.Forms;

namespace WordCards
{
    public partial class frmEditWord : Form
    {
        /// <summary>
        /// 目前正在修改的單字物件
        /// </summary>
        public WordItem Word { get; set; } = null;

        // 美化用顏色
        private readonly Color colorBackground = Color.FromArgb(255, 241, 242);     // 淡紅色背景
        private readonly Color colorCard = Color.FromArgb(255, 255, 255);
        private readonly Color colorPrimary = Color.FromArgb(37, 99, 235);
        private readonly Color colorPrimaryDark = Color.FromArgb(29, 78, 216);
        private readonly Color colorText = Color.FromArgb(30, 41, 59);
        private readonly Color colorSubText = Color.FromArgb(100, 116, 139);

        /// <summary>
        /// 預設建構子，給 Visual Studio 設計畫面使用
        /// </summary>
        public frmEditWord()
        {
            InitializeComponent();

            WireEvents();
            ApplyTheme();
        }

        /// <summary>
        /// 修改單字用的建構子
        /// </summary>
        public frmEditWord(WordItem word)
        {
            InitializeComponent();

            WireEvents();
            ApplyTheme();

            this.Word = word;

            if (word != null)
            {
                txtWord.Text = word.Word;
                txtPhonogram.Text = word.Phonogram;
                txtSoundPath.Text = word.SoundPath;
                txtExplain.Text = word.Explain;
            }
        }

        /// <summary>
        /// 統一綁定事件，避免重複綁定
        /// </summary>
        private void WireEvents()
        {
            btnSave.Click -= btnSave_Click;
            btnSave.Click += btnSave_Click;

            btnSave.MouseEnter -= btnSave_MouseEnter;
            btnSave.MouseEnter += btnSave_MouseEnter;

            btnSave.MouseLeave -= btnSave_MouseLeave;
            btnSave.MouseLeave += btnSave_MouseLeave;

            this.KeyDown -= frmEditWord_KeyDown;
            this.KeyDown += frmEditWord_KeyDown;
        }

        /// <summary>
        /// 套用修改視窗美化
        /// </summary>
        private void ApplyTheme()
        {
            this.Text = "修改單字";
            this.BackColor = colorBackground;
            this.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Regular);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.KeyPreview = true;

            // 美化 GroupBox
            StyleContainers(this);

            // 美化 Label
            StyleLabels(this);

            // 美化 TextBox
            StyleTextBox(txtWord, true);
            StyleTextBox(txtPhonogram, false);
            StyleTextBox(txtSoundPath, false);
            StyleTextBox(txtExplain, false);

            // 讓單字框寬度自動縮到 GroupBox 內，不會超出去
            FitTextBoxInsideParent(txtWord);

            // 其他單行框也整理一下，讓畫面一致
            FitTextBoxInsideParent(txtPhonogram);
            FitTextBoxInsideParent(txtSoundPath);

            // 說明欄多行設定
            txtExplain.Multiline = true;
            txtExplain.ScrollBars = ScrollBars.Vertical;

            // 儲存按鈕
            btnSave.Text = "✓ 儲存";
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.BackColor = colorPrimary;
            btnSave.ForeColor = Color.White;
            btnSave.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
            btnSave.Cursor = Cursors.Hand;
            btnSave.UseVisualStyleBackColor = false;
        }

        /// <summary>
        /// 讓 TextBox 寬度自動符合所在的 GroupBox
        /// </summary>
        private void FitTextBoxInsideParent(TextBox txt)
        {
            if (txt == null || txt.Parent == null)
                return;

            int rightPadding = 24;

            txt.Width = txt.Parent.ClientSize.Width - txt.Left - rightPadding;

            if (txt.Width < 100)
            {
                txt.Width = 100;
            }
        }

        /// <summary>
        /// 美化 GroupBox 等容器
        /// </summary>
        private void StyleContainers(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is GroupBox groupBox)
                {
                    groupBox.BackColor = colorBackground;
                    groupBox.ForeColor = colorText;
                    groupBox.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold);
                }

                if (ctrl.HasChildren)
                {
                    StyleContainers(ctrl);
                }
            }
        }

        /// <summary>
        /// 美化所有 Label
        /// </summary>
        private void StyleLabels(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label label)
                {
                    label.ForeColor = colorSubText;
                    label.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold);
                    label.BackColor = colorBackground;
                }

                if (ctrl.HasChildren)
                {
                    StyleLabels(ctrl);
                }
            }
        }

        /// <summary>
        /// 美化文字框
        /// </summary>
        private void StyleTextBox(TextBox txt, bool isMainWord)
        {
            if (txt == null)
                return;

            txt.BackColor = colorCard;
            txt.ForeColor = isMainWord ? colorPrimary : colorText;
            txt.BorderStyle = BorderStyle.FixedSingle;

            if (isMainWord)
            {
                // 單字框縮小，避免太高太大
                txt.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
                txt.Height = 36;
            }
            else
            {
                txt.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Regular);
            }
        }

        /// <summary>
        /// 儲存按鈕滑鼠移入
        /// </summary>
        private void btnSave_MouseEnter(object sender, EventArgs e)
        {
            btnSave.BackColor = colorPrimaryDark;
        }

        /// <summary>
        /// 儲存按鈕滑鼠移出
        /// </summary>
        private void btnSave_MouseLeave(object sender, EventArgs e)
        {
            btnSave.BackColor = colorPrimary;
        }

        /// <summary>
        /// Esc 關閉修改視窗
        /// </summary>
        private void frmEditWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        /// <summary>
        /// 儲存按鈕
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Word == null)
                return;

            // 簡單檢查：單字不能是空的
            if (string.IsNullOrWhiteSpace(txtWord.Text))
            {
                MessageBox.Show(
                    "單字不能是空白",
                    "提醒",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                txtWord.Focus();
                return;
            }

            // 把文字框內容寫回 WordItem 物件
            Word.Word = txtWord.Text.Trim();
            Word.Phonogram = txtPhonogram.Text.Trim();
            Word.SoundPath = txtSoundPath.Text.Trim();
            Word.Explain = txtExplain.Text.Trim();

            // 告訴主表單：使用者按了儲存
            this.DialogResult = DialogResult.Yes;

            // 關閉修改視窗
            this.Close();
        }
    }
}