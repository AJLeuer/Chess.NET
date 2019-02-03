using Chess.Game;

using AI = Chess.Game.Real.AI;
using SimpleAI = Chess.Game.Simulation.SimpleAI;

namespace Chess
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            
            BasicGame game = 
                new Game.Graphical.Game(new Game.Graphical.Board(), new AI(Color.white), new SimpleAI(Color.black));
            
            game.Play();
        }
    }
}