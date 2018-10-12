﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chess.Util;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
	public class Pawn : Piece
    {
	    
	    protected static readonly List<Direction> blackLegalCaptureDirections = new List<Direction> {downLeft, downRight};
	    protected static readonly List<Direction> whiteLegalCaptureDirections = new List<Direction> {upLeft, upRight};

	    public new static readonly Dictionary<Color, Char> defaultSymbols = new Dictionary<Color, Char>
	    {
		    {black, '♟'}, 
		    {white, '♙'}
	    };

	    public new static readonly Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
	    {
		    {black, "./Assets/Bitmaps/BlackPawn.png"},
		    {white, "./Assets/Bitmaps/WhitePawn.png"}
	    };
	    
	    public override char ASCIISymbol
	    {
		    get { return '​'; } //zero-width space
	    }

	    public override ushort Value
	    {
		    get { return 1; }
	    }

	    protected Direction legalMovementDirectionToEmptySquares
	    {
		    get
		    {
			    if (this.Color == black)
			    {
				    return down;
			    }
			    else /* if (this.color == white) */
			    {
				    return up;
			    }
		    }
	    }

	    protected List<Direction> legalCaptureDirections
	    {
		    get
		    {
			    if (this.Color == black)
			    {
				    return blackLegalCaptureDirections;
			    }
			    else /* if (this.color == white) */
			    {
				    return whiteLegalCaptureDirections;
			    }
		    }
	    }

	    protected Optional<List<Direction>> legalMovementDirections = Optional<List<Direction>>.Empty;

	    public override List<Direction> LegalMovementDirections
	    {
		    get
		    {
			    if (legalMovementDirections.HasValue == false)
			    {
				    List<Direction> directions = new List<Direction> {legalMovementDirectionToEmptySquares};
				    directions.AddRange(legalCaptureDirections);
				    legalMovementDirections = directions;
			    }
				
			    return legalMovementDirections.Object;
		    }
	    }
		
		public override ushort MaximumMoveDistance { get { return 1; } }

		public Pawn(Pawn other) :
			base(other)
	    {
		    
	    }

	    public Pawn(Color color) :
		    base(defaultSymbols[color], color, defaultImageFiles[color])
	    {
	
	    }
	
	    public Pawn(char symbol) :
		    this((symbol == defaultSymbols[black]) ? black : white)
	    {
		    if (defaultSymbols.ContainsValue(symbol) == false)
		    {
			    throw new ArgumentException($"{symbol} is not a valid chess piece");
		    }
	    }
	
	    public override Piece Clone()
	    {
		    return new Pawn(this);
	    }

	    /**
		* @return a List that is either filled with the Squares this Pawn can legally move to, or, if there are
		* no such Squares, empty
		*/
	    public override List<Square> FindAllPossibleLegalMoveDestinations()
	    {
		    List<Square> legalMoveSquares = findAllPossibleLegalMoveDestinationsForMovesToCapture();

		    Optional<Square> emptySquareToMove = findLegalMoveDestinationForMoveToEmpty();

		    if (emptySquareToMove.HasValue)
		    {
			    legalMoveSquares.Add(emptySquareToMove.Object);
		    }

		    return legalMoveSquares;
	    }

	    protected Optional<Square> findLegalMoveDestinationForMoveToEmpty()
	    {
		    			
		    Predicate<Square> squareCheckerForMovementDirections = (Square squareToCheck) =>
		    {
			    return squareToCheck.isEmpty;
		    };
		    
		    List<Square> availableSquares = Board.SearchForSquares(squareCheckerForMovementDirections,
			    this.BoardPosition, 1, this.legalMovementDirectionToEmptySquares);

		    if (availableSquares.Count > 0)
		    {
			    return availableSquares[0];
		    }
		    else
		    {
			    return Optional<Square>.Empty;
		    }
	    }
	    
	    protected List<Square> findAllPossibleLegalMoveDestinationsForMovesToCapture()
	    {
		    
		    Predicate<Square> squareCheckerForCaptureDirections = (Square squareToCheck) =>
		    {
			    if (squareToCheck.isEmpty)
			    {
				    return false;
			    }
			    else /* if (squareToCheck.isOccupied) */ 
			    {
				    return this.Color.getOpposite() == squareToCheck.Piece.Object.Color;
			    }
		    };
		    
		    List<Square> captureSquares = Board.SearchForSquares(squareCheckerForCaptureDirections,
			    this.BoardPosition, 1, this.legalCaptureDirections.ToArray());

		    return captureSquares;
	    }

	    public override void Move(RankFile destination)  
	    {
			//todo add move legality checking
		    base.Move(destination);
		}

    }
}