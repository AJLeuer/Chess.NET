using System.Collections.Generic;
using System.Security;
using Chess.Game;
using Chess.Game.Graphical;
using SFML.Window;

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