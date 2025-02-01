using System;
using System.Windows.Forms;

namespace Ex05
{
    public partial class GameSettingsForm : Form
    {
        public GameSettingsForm()
        {
            InitializeComponent();
        }
        private void checkBoxVsComputer_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPlayer2.Enabled = !checkBoxVsComputer.Checked;
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxPlayer1.Text))
            {
                MessageBox.Show("Please enter Player 1 name.",
                    "Input Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBoxPlayer1.Focus();
                return;
            }

            if (!checkBoxVsComputer.Checked && string.IsNullOrWhiteSpace(textBoxPlayer2.Text))
            {
                MessageBox.Show("Please enter Player 2 name.",
                    "Input Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                textBoxPlayer2.Focus();
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public string Player1Name
        {
            get { return textBoxPlayer1.Text.Trim(); }
        }
        public string Player2Name
        {
            get
            {
                return checkBoxVsComputer.Checked
                           ? "Computer"
                           : textBoxPlayer2.Text.Trim();
            }
        }
        public bool SIsVsComputer
        {
            get { return checkBoxVsComputer.Checked; }
        }
        public int SBoardSize
        {
            get
            {
                if (radioButton6.Checked) return 6;
                if (radioButton8.Checked) return 8;
                if (radioButton10.Checked) return 10;
                return 6;
            }
        }
    }
}


