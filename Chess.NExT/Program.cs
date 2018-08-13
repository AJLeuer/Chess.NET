using System;
using Chess.Game;
using SFML.System;

namespace Chess.NExT
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Vector2f vector = new Vector2f(1.1f, 2f);

            BasicGame game = new ChessGame();
            
            game.playGame();

            Console.Write("fuck");
        }
    }
}