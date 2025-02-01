using System;
using System.Windows.Forms;

namespace Ex05
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
         
            using (GameSettingsForm settingsForm = new GameSettingsForm())
            {
                DialogResult result = settingsForm.ShowDialog();
               
                if (result == DialogResult.OK)
                {
                    int boardSize = settingsForm.SBoardSize;
                    bool isVsComputer = settingsForm.SIsVsComputer;
                    string player1Name = settingsForm.Player1Name;
                    string player2Name = settingsForm.Player2Name;
                    
                    Application.Run(new CheckersGameForm(boardSize, isVsComputer, player1Name, player2Name));
                }
            }
        }
    }
}
