using Chess.Game;

namespace Chess.NExT
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BasicGame game = new ChessGame();
            
            game.PlayGame();
        }
    }
}