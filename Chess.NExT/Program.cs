﻿using System;
using Chess.Game;
using SFML.System;

namespace Chess.NExT
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            BasicGame game = new ChessGame();
            
            
            game.playGame();

        }
    }
}