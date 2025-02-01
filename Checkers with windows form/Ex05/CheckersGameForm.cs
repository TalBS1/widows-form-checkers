using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ex05
{
    public partial class CheckersGameForm : Form
    {
        private Game m_Game;
        private readonly Board r_MBoard;
        private Button[,] m_BoardButtons;
        private Button m_SelectedPiece = null;
        private int m_SelectedRow = -1;
        private int m_SelectedCol = -1;
        private readonly int r_MBoardSize = 0;
        private readonly bool r_IsVsComputer = false;
        public CheckersGameForm(int i_BoardSize, bool i_IsVsComputer, string i_PlayerOneName, string i_PlayerTwoName)
        {
            r_MBoardSize = i_BoardSize;
            r_IsVsComputer = i_IsVsComputer;
            InitializeComponent();
            m_Game = new Game(i_BoardSize, i_IsVsComputer, i_PlayerOneName, i_PlayerTwoName);
            r_MBoard = new Board(i_BoardSize);
            initializeBoard(i_BoardSize);
            initializeScoreLabels(i_PlayerOneName, i_PlayerTwoName);
        }
        private void initializeBoard(int i_BoardSize)
        {
            Color lightWoodColor = Color.FromArgb(222, 184, 135); 
            Color darkWoodColor = Color.FromArgb(139, 69, 19);   
            m_BoardButtons = new Button[i_BoardSize, i_BoardSize];
            int panelSize = Math.Min(panelBoard.Width, panelBoard.Height);
            int buttonSize = panelSize / i_BoardSize;

            for (int row = 0; row < i_BoardSize; row++)
            {

                for (int col = 0; col < i_BoardSize; col++)
                {
                    Button button = new Button();
                    button.Size = new Size(buttonSize, buttonSize);
                    button.Location = new Point(col * buttonSize, row * buttonSize);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    bool isDarkSquare = (row + col) % 2 != 0;
                    button.BackColor = isDarkSquare ? darkWoodColor : lightWoodColor;
                    button.Enabled = isDarkSquare;
                    ePieceType pieceType = r_MBoard.GetPieceType(row, col);
                    
                    switch (pieceType)
                    {
                        case ePieceType.X: 
                            button.Text = "◉";
                            button.Font = new Font("Arial", buttonSize / 2, FontStyle.Bold);
                            button.ForeColor = Color.Black;
                            break;

                        case ePieceType.K: 
                            button.Text = "♚";
                            button.Font = new Font("Arial", buttonSize / 2, FontStyle.Bold);
                            button.ForeColor = Color.Black;
                            break;

                        case ePieceType.O: 
                            button.Text = "◉";
                            button.Font = new Font("Arial", buttonSize / 2, FontStyle.Bold);
                            button.ForeColor = Color.White;
                            break;

                        case ePieceType.U: 
                            button.Text = "♔";
                            button.Font = new Font("Arial", buttonSize / 2, FontStyle.Bold);
                            button.ForeColor = Color.White;
                            break;

                        default:
                            button.Text = "";
                            break;
                    }
                    button.Click += Button_Click;
                    panelBoard.Controls.Add(button);
                    m_BoardButtons[row, col] = button;
                }
            }
        }
        private void selectPiece(Button i_PieceButton)
        {
            if (m_SelectedPiece == i_PieceButton)
            {
                m_SelectedPiece.BackColor = Color.White;
                m_SelectedPiece = null;
                m_SelectedRow = -1;
                m_SelectedCol = -1;
                return;
            }

            if (m_SelectedPiece != null)
            {
                m_SelectedPiece.BackColor = Color.White;
            }

            m_SelectedPiece = i_PieceButton;
            m_SelectedRow = i_PieceButton.Location.Y / i_PieceButton.Height;
            m_SelectedCol = i_PieceButton.Location.X / i_PieceButton.Width;
            i_PieceButton.BackColor = Color.LightBlue;
        }
        private void attemptMove(Button i_TargetButton)
        {

            if (m_SelectedPiece == null)
            {
                return;
            }

            int targetRow = i_TargetButton.Location.Y / i_TargetButton.Height;
            int targetCol = i_TargetButton.Location.X / i_TargetButton.Width;

            Player currentPlayer = m_Game.GetCurrentPlayer();

            if (currentPlayer == null)
            {
                MessageBox.Show(
                    "Error: No current player found!",
                    "Game Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            List<(char, char, char, char)> capturingMoves = m_Game.GameBoard.GetCapturingMoves(currentPlayer.ISymbol);
            bool isCaptureMove = false;

            for (int i = 0; i < capturingMoves.Count; i++)
            {

                if (capturingMoves[i].Item1 - 'A' == m_SelectedRow && capturingMoves[i].Item2 - 'a' == m_SelectedCol
                                                                  && capturingMoves[i].Item3 - 'A' == targetRow
                                                                  && capturingMoves[i].Item4 - 'a' == targetCol)
                {
                    isCaptureMove = true;
                    break;
                }
            }

            if (capturingMoves.Count > 0 && !isCaptureMove)
            {
                MessageBox.Show(
                    "You must make a capturing move!",
                    "Capture Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            bool isValidMove = m_Game.GameBoard.IsMoveLegal(
                (char)(m_SelectedRow + 'A'),
                (char)(m_SelectedCol + 'a'),
                (char)(targetRow + 'A'),
                (char)(targetCol + 'a'),
                currentPlayer.ISymbol);

            if (isValidMove)
            {
                bool hasMoreCaptures = m_Game.GameBoard.ApplyMove(
                    (char)(m_SelectedRow + 'A'),
                    (char)(m_SelectedCol + 'a'),
                    (char)(targetRow + 'A'),
                    (char)(targetCol + 'a'),
                    currentPlayer.ISymbol);

                updateUiAfterMove(targetRow, targetCol, isCaptureMove);
                refreshBoardUi();
                checkGameOver();

                if (m_Game.CheckGameOver(out string winner))
                {
                    MessageBox.Show(winner, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                else
                {

                    if (isCaptureMove && hasMoreCaptures)
                    {
                        m_SelectedPiece = m_BoardButtons[targetRow, targetCol];
                        m_SelectedRow = targetRow;
                        m_SelectedCol = targetCol;
                        MessageBox.Show(
                            "You must continue capturing!",
                            "Multiple Captures",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    m_Game.SwitchTurn();

                    if (m_Game.IsVsComputer() && m_Game.GetCurrentPlayer().IsComputer)
                    {
                        performComputerMove();
                    }
                }
            }

            else
            {
                MessageBox.Show("Invalid move! Try again.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void performComputerMove()
        {
            Player computerPlayer = m_Game.GetComputerPlayer();
            if (computerPlayer == null)
            {
                return;
            }
            List<(char, char, char, char)> capturingMoves = m_Game.GameBoard.GetCapturingMoves(computerPlayer.ISymbol);

            if (capturingMoves.Count > 0)
            {
                bool keepCapturing = true;

                while (keepCapturing)
                {
                    capturingMoves = m_Game.GameBoard.GetCapturingMoves(computerPlayer.ISymbol);

                    if (capturingMoves.Count == 0)
                    {
                        keepCapturing = false;
                    }

                    else
                    {
                        var move = capturingMoves[0];
                        bool canContinue = m_Game.GameBoard.ApplyMove(
                            move.Item1, move.Item2, move.Item3, move.Item4, computerPlayer.ISymbol);
                        refreshBoardUi();
                        if (!canContinue)
                        {
                            keepCapturing = false;
                        }
                    }
                }
            }

            else
            {
                var randomMove = m_Game.GameBoard.GetRandomMove(computerPlayer.ISymbol);
                m_Game.GameBoard.ApplyMove(
                    randomMove.Item1, randomMove.Item2,
                    randomMove.Item3, randomMove.Item4,
                    computerPlayer.ISymbol);
                refreshBoardUi();
            }

            if (m_Game.CheckGameOver(out string winner))
            {
                updateScoreLabels();
                MessageBox.Show(winner, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (askForAnotherGame())
                {
                    restartGame();
                }

                else
                {
                    Close();
                }
            }

            else
            {
                m_Game.SwitchTurn();
            }
        }
        private void checkGameOver()
        {
            if (m_Game.CheckGameOver(out string winner))
            {
                updateScoreLabels();
                MessageBox.Show(winner, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (askForAnotherGame())
                {
                    restartGame();
                }

                else
                {
                    Close();
                }
            }
        }
        private bool askForAnotherGame()
        {
            DialogResult result = MessageBox.Show(
                "Do you want to play again?",
                "Play Again?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }
        private void restartGame()
        {
            panelBoard.Controls.Clear();
            int playerOneScore = m_Game.Player1Score;
            int playerTwoScore = m_Game.Player2Score;
            m_Game = new Game(r_MBoardSize, r_IsVsComputer, m_Game.Player1Name, m_Game.Player2Name);
            m_Game.Player1Score = playerOneScore;
            m_Game.Player2Score = playerTwoScore;
            initializeBoard(r_MBoardSize);
            refreshBoardUi();
            updateScoreLabels();
        }
        private void refreshBoardUi()
        {
            
            Color lightWoodColor = Color.FromArgb(222, 184, 135); 
            Color darkWoodColor = Color.FromArgb(139, 69, 19);   

            for (int row = 0; row < m_Game.BoardSize; row++)
            {

                for (int col = 0; col < m_Game.BoardSize; col++)
                {
                    Button button = m_BoardButtons[row, col];
                    bool isDarkSquare = (row + col) % 2 != 0;
                    button.BackColor = isDarkSquare ? darkWoodColor : lightWoodColor;
                    button.Enabled = isDarkSquare;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                   
                    ePieceType pieceType = m_Game.GameBoard.GetPieceType(row, col);
                  
                    switch (pieceType)
                    {
                        case ePieceType.X: 
                            button.Text = "◉";
                            button.Font = new Font("Arial", button.Height / 2, FontStyle.Bold);
                            button.ForeColor = Color.Black;
                            break;

                        case ePieceType.K: 
                            button.Text = "♚";
                            button.Font = new Font("Arial", button.Height / 2, FontStyle.Bold);
                            button.ForeColor = Color.Black;
                            break;

                        case ePieceType.O: 
                            button.Text = "◉";
                            button.Font = new Font("Arial", button.Height / 2, FontStyle.Bold);
                            button.ForeColor = Color.White;
                            break;

                        case ePieceType.U: 
                            button.Text = "♔";
                            button.Font = new Font("Arial", button.Height / 2, FontStyle.Bold);
                            button.ForeColor = Color.White;
                            break;

                        default:
                            button.Text = "";
                            break;
                    }
                }
            }
        }
        private void updateUiAfterMove(int i_TargetRow, int i_TargetCol, bool i_IsCaptureMove)
        {
            Button targetButton = m_BoardButtons[i_TargetRow, i_TargetCol];
            targetButton.Text = m_SelectedPiece.Text;
            targetButton.ForeColor = m_SelectedPiece.ForeColor;

            m_SelectedPiece.Text = "";
            m_SelectedPiece.BackColor = Color.White;

            if (i_IsCaptureMove)
            {
                int midRow = (m_SelectedRow + i_TargetRow) / 2;
                int midCol = (m_SelectedCol + i_TargetCol) / 2;
                m_BoardButtons[midRow, midCol].Text = "";
            }

            m_SelectedPiece = null;
        }
        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton == null)
            {
                return;
            }

            if (m_SelectedPiece == clickedButton)
            {
                m_SelectedPiece.BackColor = Color.White;
                m_SelectedPiece = null;
                return;
            }

            if (m_SelectedPiece != null)
            {
                attemptMove(clickedButton);
            }

            else if (!string.IsNullOrEmpty(clickedButton.Text))
            {
                selectPiece(clickedButton);
            }
        }
        private void initializeScoreLabels(string i_Player1Name, string i_Player2Name)
        {
            labelPlayer1.Text = $"{i_Player1Name}: 0 Points";
            labelPlayer2.Text = $"{i_Player2Name}: 0 Points";
        }
        private void updateScoreLabels()
        {
            labelPlayer1.Text = $"{m_Game.GetPlayer1().IName}: {m_Game.Player1Score} Points";
            labelPlayer2.Text = $"{m_Game.GetPlayer2().IName}: {m_Game.Player2Score} Points";
        }
    }
}

