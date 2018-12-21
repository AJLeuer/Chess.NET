using Chess.Game;
using Chess.Game.Graphical;

namespace Chess.NExT
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BasicGame game = 
                new Game.Graphical.Game(new Game.Graphical.Board(), new AI(Color.white), new MockAIPlayer(Color.black));
            
            game.PlayGame();
        }
    }
}