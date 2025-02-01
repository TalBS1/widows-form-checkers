using Ex02.ConsoleUtils;
using System;
using System.Collections.Generic;

namespace Ex05
{
    public class Game
    {
        private Board m_Board;
        private Player m_Player1;
        private Player m_Player2;
        private bool m_IsVsComputer;
        private int m_Player1Score;
        private int m_Player2Score;
        private int m_BoardSize;
        private bool m_IsInitialized;
        private string m_Player1Name;
        private string m_Player2Name;
        private Player m_CurrentPlayer;
        public Game(int i_BoardSize, bool i_IsVsComputer, string i_Player1Name, string i_Player2Name)
        {
            m_BoardSize = i_BoardSize;
            m_IsVsComputer = i_IsVsComputer;
            m_Board = new Board(i_BoardSize);

            m_Player1Name = i_Player1Name;
            m_Player2Name = i_IsVsComputer ? "Computer" : i_Player2Name;

            m_Player1 = new Player(m_Player1Name, ePieceType.X);
            m_Player2 = new Player(m_Player2Name, ePieceType.O, i_IsVsComputer);

            m_CurrentPlayer = m_Player1;
        }
        public Board GameBoard
        {
            get { return m_Board; }
        }
        public string Player1Name
        {
            get { return m_Player1Name; }
            set { m_Player1Name = value; }
        }
        public string Player2Name
        {
            get { return m_Player2Name; }
            set { m_Player2Name = value; }
        }
        public int Player1Score
        {
            get { return m_Player1Score; }
            set { m_Player1Score = value; }
        }
        public int Player2Score
        {
            get { return m_Player2Score; }
            set { m_Player2Score = value; }
        }
        public Player GetPlayer1()
        {
            return m_Player1;
        }
        public Player GetPlayer2()
        {
            return m_Player2;
        }
        public void Start()
        {
            InitializeGame();
            playGame();
        }
        public int BoardSize
        {
            get { return m_BoardSize; }
        }
        public void InitializeGame()
        {
            if (!m_IsInitialized)
            {
                Console.Write("Enter Player 1 name: ");
                m_Player1Name = Console.ReadLine();
                Console.Write("Enter board size (6, 8, or 10): ");
                m_BoardSize = getValidBoardSize();
                Console.Write("Play against computer? (y/n): ");
                m_IsVsComputer = Console.ReadLine()?.Trim().ToLower() == "y";

                if (m_IsVsComputer)
                {
                    m_Player1 = new Player(m_Player1Name, ePieceType.X);
                    m_Player2 = new Player("Computer", ePieceType.O, true);
                }
                else
                {
                    Console.Write("Enter Player 2 name: ");
                    m_Player2Name = Console.ReadLine();
                    m_Player1 = new Player(m_Player1Name, ePieceType.X);
                    m_Player2 = new Player(m_Player2Name, ePieceType.O);
                }

                m_IsInitialized = true;
            }
            else
            {
                if (m_IsVsComputer)
                {
                    m_Player1 = new Player(m_Player1Name, ePieceType.X);
                    m_Player2 = new Player("Computer", ePieceType.O, true);
                }
                else
                {
                    m_Player1 = new Player(m_Player1Name, ePieceType.X);
                    m_Player2 = new Player(m_Player2Name, ePieceType.O);
                }
            }

            m_Board = new Board(m_BoardSize);
            m_CurrentPlayer = m_Player1;
        }
        public Player GetCurrentPlayer()
        {
            return m_CurrentPlayer;
        }
        public bool IsVsComputer()
        {
            return m_IsVsComputer;
        }
        private int getValidBoardSize()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int size) && (size == 6 || size == 8 || size == 10))
                {
                    return size;
                }

                Console.Write("Invalid size. Enter 6, 8, or 10: ");
            }
        }
        private void playGame()
        {
            bool gameRunning = true;
            Player currentPlayer = m_Player1;

            while (gameRunning)
            {
                Screen.Clear();
                m_Board.Display();
                Console.WriteLine($"{currentPlayer.IName}'s Turn ({currentPlayer.ISymbol}):");

                if (currentPlayer.IsComputer)
                {
                    PerformComputerMove(currentPlayer);
                }

                else
                {
                    string move = getValidMove(currentPlayer);
                    processMove(move, currentPlayer);
                }

                if (CheckGameOver(out string winner))
                {
                    Screen.Clear();
                    m_Board.Display();
                    Console.WriteLine(winner);
                    gameRunning = AskForAnotherGame();
                }

                else
                {
                    currentPlayer = currentPlayer == m_Player1 ? m_Player2 : m_Player1;
                }
            }
        }
        private string getValidMove(Player i_CurrentPlayer)
        {
            while (true)
            {
                List<(char, char, char, char)> capturingMoves = m_Board.GetCapturingMoves(i_CurrentPlayer.ISymbol);

                if (capturingMoves.Count > 0)
                {
                    Console.WriteLine("You must make a capturing move.");
                }

                Console.Write("Enter your move (RowCol>RowCol) or 'Q' to quit: ");
                string input = Console.ReadLine()?.Trim();

                if (input?.ToLower() == "q")
                {
                    Console.WriteLine($"{i_CurrentPlayer.IName} has quit the game.");
                    Environment.Exit(0);
                }

                if (isValidMove(input, i_CurrentPlayer.ISymbol, capturingMoves))
                {
                    return input;
                }

                else
                {
                    Console.WriteLine("Invalid move. Please try again.");
                }
            }
        }
        private bool isValidMove(string i_Move, ePieceType i_Symbol, List<(char, char, char, char)> i_CapturingMoves)
        {
            string[] parts = i_Move.Split('>');
            if (parts.Length != 2 || parts[0].Length != 2 || parts[1].Length != 2)
            {
                return false;
            }

            char startRow = parts[0][0];
            char startCol = parts[0][1];
            char endRow = parts[1][0];
            char endCol = parts[1][1];
            int rowDiff = endRow - startRow;
            ePieceType initialLocation = m_Board.GetPieceType(startRow - 'A', startCol - 'a');
            bool isForwardMove = (i_Symbol == ePieceType.X && rowDiff < 0)
                                 || (i_Symbol == ePieceType.O && rowDiff > 0) || initialLocation == ePieceType.K
                                 || initialLocation == ePieceType.U;

            if (i_CapturingMoves.Count > 0)
            {
                foreach (var capturingMove in i_CapturingMoves)
                {
                    if (capturingMove.Item1 == startRow && capturingMove.Item2 == startCol
                                                       && capturingMove.Item3 == endRow
                                                       && capturingMove.Item4 == endCol)
                    {
                        return isForwardMove;
                    }
                }

                return false;
            }

            return m_Board.IsMoveLegal(startRow, startCol, endRow, endCol, i_Symbol);
        }
        private void processMove(string i_Move, Player i_Player)
        {
            string[] parts = i_Move.Split('>');
            char startRow = parts[0][0];
            char startCol = parts[0][1];
            char endRow = parts[1][0];
            char endCol = parts[1][1];
            //Do i have another chance to eat
            bool extraTurn = m_Board.ApplyMove(startRow, startCol, endRow, endCol, i_Player.ISymbol);
            bool isCaptureMove = Math.Abs(startRow - endRow) == 2;

            while (isCaptureMove && extraTurn)
            {
                Screen.Clear();
                m_Board.Display();
                Console.WriteLine("You must continue capturing!");
                List<(char, char, char, char)> capturingMoves = m_Board.GetCapturingMovesForPiece(endRow - 'A', endCol - 'a', i_Player.ISymbol);

                if (capturingMoves.Count > 0)
                {
                    string nextMove = getValidMove(i_Player);
                    parts = nextMove.Split('>');
                    startRow = parts[0][0];
                    startCol = parts[0][1];
                    endRow = parts[1][0];
                    endCol = parts[1][1];
                    isCaptureMove = Math.Abs(startRow - endRow) == 2;
                    extraTurn = m_Board.ApplyMove(startRow, startCol, endRow, endCol, i_Player.ISymbol);
                }
                else
                {
                    break;
                }
            }
        }
        public void PerformComputerMove(Player i_Computer)
        {
            Console.WriteLine("Computer's Turn. Press 'Enter' to see its move.");
            Console.ReadLine();
            List<(char, char, char, char)> capturingMoves = m_Board.GetCapturingMoves(i_Computer.ISymbol);

            if (capturingMoves.Count > 0)
            {
                bool keepCapturing = true;

                while (keepCapturing)
                {
                    capturingMoves = m_Board.GetCapturingMoves(i_Computer.ISymbol);

                    if (capturingMoves.Count == 0)
                    {
                        keepCapturing = false;
                    }

                    else
                    {
                        var currentMove = capturingMoves[0];
                        char startRow = currentMove.Item1;
                        char startCol = currentMove.Item2;
                        char endRow = currentMove.Item3;
                        char endCol = currentMove.Item4;
                        bool canContinueWithSamePiece = m_Board.ApplyMove(
                            startRow, startCol, endRow, endCol, i_Computer.ISymbol);

                        Console.WriteLine($"Computer captured: {startRow}{startCol}>{endRow}{endCol}");

                        if (!canContinueWithSamePiece)
                        {
                            keepCapturing = false;
                        }
                    }
                }
            }

            else
            {
                var randomMove = m_Board.GetRandomMove(i_Computer.ISymbol);
                m_Board.ApplyMove(
                    randomMove.Item1, randomMove.Item2,
                    randomMove.Item3, randomMove.Item4,
                    i_Computer.ISymbol);

                Console.WriteLine(
                    $"Computer moved: {randomMove.Item1}{randomMove.Item2}>{randomMove.Item3}{randomMove.Item4}");
            }
        }
        private int countPlayerPoints(ePieceType i_Symbol)
        {
            int totalPoints = 0;

            for (int row = 0; row < m_BoardSize; row++)
            {

                for (int col = 0; col < m_BoardSize; col++)
                {
                    ePieceType piece = m_Board.GetPieceType(row, col);
                  
                    if (i_Symbol == ePieceType.X || i_Symbol == ePieceType.K)
                    {

                        if (piece == ePieceType.X)
                        {
                            totalPoints += 1;
                        }

                        else if (piece == ePieceType.K)
                        {
                            totalPoints += 4;
                        }
                    }

                    else
                    {
                        if (piece == ePieceType.O)
                        {
                            totalPoints += 1;
                        }
                        else if (piece == ePieceType.U)
                        {
                            totalPoints += 4;
                        }
                    }
                }
            }
            return totalPoints;
        }
        public bool CheckGameOver(out string i_OWinnerMessage)
        {
            bool p1HasPieces = m_Board.HasPieces(m_Player1.ISymbol);
            bool p2HasPieces = m_Board.HasPieces(m_Player2.ISymbol);
            bool p1HasMoves = m_Board.HasLegalMoves(m_Player1.ISymbol);
            bool p2HasMoves = m_Board.HasLegalMoves(m_Player2.ISymbol);
          
            if (!p1HasPieces && !p2HasPieces)
            {
                i_OWinnerMessage = "It's a tie! Both players ran out of pieces.";
                return true;
            }
           
            if (!p1HasPieces)
            {
                i_OWinnerMessage = $"{m_Player2.IName} wins! {m_Player1.IName} is out of pieces.";

                int p2Points = countPlayerPoints(m_Player2.ISymbol);
                m_Player2Score += p2Points;
                return true;
            }

            if (!p2HasPieces)
            {
                i_OWinnerMessage = $"{m_Player1.IName} wins! {m_Player2.IName} is out of pieces.";

                int p1Points = countPlayerPoints(m_Player1.ISymbol);
                m_Player1Score += p1Points;
                return true;
            }

            if (!p1HasMoves && !p2HasMoves)
            {
                i_OWinnerMessage = "It's a tie! Neither player can move.";
                return true;
            }
            else if (!p1HasMoves)
            {
                i_OWinnerMessage = $"{m_Player2.IName} wins! {m_Player1.IName} cannot move.";

                int p1Points = countPlayerPoints(m_Player1.ISymbol);
                int p2Points = countPlayerPoints(m_Player2.ISymbol);
                int difference = Math.Abs(p2Points - p1Points);
                m_Player2Score += difference;
                return true;
            }
            else if (!p2HasMoves)
            {
                i_OWinnerMessage = $"{m_Player1.IName} wins! {m_Player2.IName} cannot move.";

                int p1Points = countPlayerPoints(m_Player1.ISymbol);
                int p2Points = countPlayerPoints(m_Player2.ISymbol);
                int difference = Math.Abs(p2Points - p1Points);
                m_Player1Score += difference;
                return true;
            }
            i_OWinnerMessage = null;
            return false;
        }
        public bool AskForAnotherGame()
        {
            Console.Write("Would you like to play another game? (y/n): ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "y")
            {
                InitializeGame();
                Start();
            }

            else
            {
                Console.WriteLine("Thanks for playing!");
                Console.WriteLine(
                    $"Final Scores: {m_Player1Name}: {m_Player1Score}, {m_Player2Name}: {m_Player2Score}");
                return false;
            }

            return true;
        }
        public Player GetComputerPlayer()
        {
            return m_IsVsComputer ? m_Player2 : null;
        }
        public void SwitchTurn()
        {
            m_CurrentPlayer = (m_CurrentPlayer == m_Player1) ? m_Player2 : m_Player1;
        }
    }
}
