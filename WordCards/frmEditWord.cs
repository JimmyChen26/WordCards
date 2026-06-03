using System;
using System.Windows.Forms;

namespace WordCards
{
    public partial class frmEditWord : Form
    {
        /// <summary>
        /// 目前正在修改的單字物件
        /// </summary>
        public WordItem Word { get; set; } = null;

        /// <summary>
        /// 預設建構子，給 Visual Studio 設計畫面使用
        /// </summary>
        public frmEditWord()
        {
            InitializeComponent();

            btnSave.Click -= btnSave_Click;
            btnSave.Click += btnSave_Click;
        }

        /// <summary>
        /// 修改單字用的建構子
        /// </summary>
        public frmEditWord(WordItem word)
        {
            InitializeComponent();

            btnSave.Click -= btnSave_Click;
            btnSave.Click += btnSave_Click;

            this.Word = word;

            txtWord.Text = word.Word;
            txtPhonogram.Text = word.Phonogram;
            txtSoundPath.Text = word.SoundPath;
            txtExplain.Text = word.Explain;
        }

        /// <summary>
        /// 儲存按鈕
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Word == null)
                return;

            // 把文字框內容寫回 WordItem 物件
            Word.Word = txtWord.Text;
            Word.Phonogram = txtPhonogram.Text;
            Word.SoundPath = txtSoundPath.Text;
            Word.Explain = txtExplain.Text;

            // 告訴主表單：使用者按了儲存
            this.DialogResult = DialogResult.Yes;

            // 關閉修改視窗
            this.Close();
        }
    }
}