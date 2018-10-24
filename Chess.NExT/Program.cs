using Chess.Game;

namespace Chess.NExT
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BasicGame game = new Game.Graphical.Game();
            
            game.PlayGame();
        }
    }
}