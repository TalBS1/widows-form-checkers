
namespace Ex05
{
    public class Player
    {
        public string IName { get; }
        public ePieceType ISymbol { get; }
        public bool IsComputer { get; }

        public Player(string i_Name, ePieceType i_Symbol, bool i_IsComputer = false)
        {
            IName = i_Name;
            ISymbol = i_Symbol;
            IsComputer = i_IsComputer;
        }
    }
}
