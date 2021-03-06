﻿using System;
using System.Collections.Generic;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game 
{
    public interface IBishop : IPiece {}

    namespace Simulation 
    {
        public class Bishop : Piece, IBishop 
        {
            protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction> {upLeft, upRight, downLeft, downRight};
        
            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♝'}, 
                {white, '♗'}
            };

            public override char ASCIISymbol
            {
                get { return 'B'; }
            }

            public override ushort Value
            {
                get { return 3; }
            }
        
            public override List<Direction> LegalMovementDirections
            {
                get { return DefaultLegalMovementDirections; }
            }

            public Bishop(IBishop other) :
                base(other)
            {
		    
            }
	    
            public Bishop(Color color) :
                base(DefaultSymbols[color], color)
            {
	
            }
	
            public Bishop(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }
        
            public override IPiece Clone()
            {
                return new Bishop(this);
            }
        }
    }
    
    namespace Graphical 
    {
        public class Bishop : Piece, IBishop 
        {
            protected static readonly List<Direction> DefaultLegalMovementDirections = new List<Direction> {upLeft, upRight, downLeft, downRight};
        
            public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
            {
                {black, '♝'}, 
                {white, '♗'}
            };

            public static readonly Dictionary<Color, String> DefaultSpriteImageFiles = new Dictionary<Color, String> 
            {
                {black, "./Assets/Bitmaps/BlackBishop.png"},
                {white, "./Assets/Bitmaps/WhiteBishop.png"}
            };

            public override char ASCIISymbol
            {
                get { return 'B'; }
            }

            public override ushort Value
            {
                get { return 3; }
            }
        
            public override List<Direction> LegalMovementDirections
            {
                get { return DefaultLegalMovementDirections; }
            }

            public Bishop(IBishop other) :
                base(other)
            {
                SpriteImageFilePath = DefaultSpriteImageFiles[this.Color];
            }
	    
            public Bishop(Color color) :
                base(DefaultSymbols[color], color, DefaultSpriteImageFiles[color])
            {
	
            }
	
            public Bishop(char symbol) :
                this((symbol == DefaultSymbols[black]) ? black : white)
            {
                if (DefaultSymbols.ContainsValue(symbol) == false)
                {
                    throw new ArgumentException($"{symbol} is not a valid chess piece");
                }
            }
        
            public override IPiece Clone()
            {
                return new Bishop(this);
            }
        }
    }
}