using Chess.Game;

namespace Chess.NExT
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BasicGame game = 
                new Game.Simulation.Game(new Chess.Game.Simulation.Board(), new AI(Color.white), new MockAIPlayer(Color.black));
            
            game.PlayGame();
        }
    }
}