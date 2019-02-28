using Chess.Game;
using Chess.Game.Real;
using Chess.Input;
using AI = Chess.Game.Real.AI;
using SimpleAI = Chess.Game.Simulation.SimpleAI;

namespace Chess
{
    internal static class Program
    {
        public static void PlayAIAgainstHuman()
        {
            var computer = new AI(Color.white);
            var inputController = new ConsoleInputController();
            var humanPlayer = new Human(Color.black, inputController);
            
            BasicGame game = 
                new Game.Graphical.Game(new Game.Graphical.Board(), computer, humanPlayer);
            
            game.Play();
        }
        
        public static void PlayAIAgainstAI()
        {
            BasicGame game = 
                new Game.Graphical.Game(new Game.Graphical.Board(), new AI(Color.white), new SimpleAI(Color.black));
            
            game.Play();
        }
        
        public static void Main(string[] args)
        {
            PlayAIAgainstHuman();
        }
    }
}