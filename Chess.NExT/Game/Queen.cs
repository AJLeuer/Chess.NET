﻿using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;


namespace Chess.Game
{
    public class Queen : Piece
    {
        
        protected static readonly List<Direction> defaultLegalMovementDirections = new List<Direction>
        {
            up,
            down,
            left,
            right,
            upLeft,
            upRight,
            downLeft,
            downRight
        };
        
        public static readonly new Dictionary<Color, char> defaultSymbols = new Dictionary<Color, char>
        {
            {black, '♛'}, 
            {white, '♕'}
        };

        public static readonly new Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
        {
            {black, "./Assets/Bitmaps/BlackQueen.png"},
            {white, "./Assets/Bitmaps/WhiteQueen.png"}
        };
        
        public override ushort value
        {
            get { return 9; }
        }
        
        public override List<Direction> legalMovementDirections
        {
            get { return defaultLegalMovementDirections; }
        }
        
        public Queen(Queen other) :
            base(other)
        {
		    
        }
	    
        public Queen(Color color, Square square) :
            base((color == black) ? defaultSymbols[black] : defaultSymbols[white],
                 (color == black) ? defaultImageFiles[black] : defaultImageFiles[white], color, square)
        {
	
        }
	
        public Queen(char symbol, Square square) :
            base((symbol == defaultSymbols[black]) ? defaultSymbols[black] : defaultSymbols[white],
                 (symbol == defaultSymbols[black]) ? defaultImageFiles[black] : defaultImageFiles[white],
                 (symbol == defaultSymbols[black]) ? black : white, square) 
        {
	
        }
        
        public override Piece Clone()
        {
            return new Queen(this);
        }

        public override void move(RankAndFile destination) 
        {
            //todo add move legality checking
            base.move(destination);
        }
    }
}