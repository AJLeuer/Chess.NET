using System;
using System.Collections.Generic;
using System.Linq;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
    public class Pawn : Piece
    {
	    
	    protected static readonly List<Direction> blackLegalCaptureDirections = new List<Direction> {downLeft, downRight};
	    protected static readonly List<Direction> whiteLegalCaptureDirections = new List<Direction> {upLeft, upRight};

	    public static readonly new Dictionary<Color, char> defaultSymbols = new Dictionary<Color, char>
	    {
		    {black, '♟'}, 
		    {white, '♙'}
	    };

	    public static readonly new Dictionary<Color, String> defaultImageFiles = new Dictionary<Color, String> 
	    {
		    {black, "./Assets/Bitmaps/BlackPawn.png"},
		    {white, "./Assets/Bitmaps/WhitePawn.png"}
	    };

	    public override ushort value
	    {
		    get { return 1; }
	    }

	    protected Direction legalMovementDirectionToEmptySquares
	    {
		    get
		    {
			    if (this.color == black)
			    {
				    return down;
			    }
			    else /* if (this.color == white) */
			    {
				    return up;
			    }
		    }
	    }

	    protected ref readonly List<Direction> legalCaptureDirections
	    {
		    get
		    {
			    if (this.color == black)
			    {
				    return ref blackLegalCaptureDirections;
			    }
			    else /* if (this.color == white) */
			    {
				    return ref whiteLegalCaptureDirections;
			    }
		    }
	    }
	    
	    public override List<Direction> legalMovementDirections
	    {
		    get
		    {
			    List<Direction> directions = new List<Direction>{legalMovementDirectionToEmptySquares};
			    directions.AddRange(legalCaptureDirections);
			    return directions;
		    }
	    }

		public Pawn(Pawn other) :
			base(other)
	    {
		    
	    }

	    
        public Pawn(Color color, Square square) :
			base((color == black) ? defaultSymbols[black] : defaultSymbols[white],
				 (color == black) ? defaultImageFiles[black] : defaultImageFiles[white], color, square)
		{
	
		}
	
		public Pawn(char symbol, Square square) :
			base((symbol == defaultSymbols[black]) ? defaultSymbols[black] : defaultSymbols[white],
				 (symbol == defaultSymbols[black]) ? defaultImageFiles[black] : defaultImageFiles[white],
				 (symbol == defaultSymbols[black]) ? black : white, square) 
		{
	
		}
	    
	    
	    ~Pawn () {}
	    
	    public override Piece Clone()
	    {
		    return new Pawn(this);
	    }

	    /**
		* @return a List that is either filled with the Squares this Pawn can legally move to, or, if there are
		* no such Squares, empty
		*/
	    public override List<Square> findAllPossibleLegalMoveDestinations()
		{
			List<Square> emptySquares = board.getSpecifiedSquares((square.position == startingPosition) ? (uint)2 : (uint)1, //if this is the pawn's first move, it can move 2 squares
				false, black, position, legalMovementDirectionToEmptySquares); //last argument should be ignored
	
			List<Square> captureSquares = new List<Square>();
			List<Square> potentialCaptureSquares = board.getSpecifiedSquares(1, true, color.getOpposite(), 
				position, legalCaptureDirections.ToArray());
	
			/* Only take the squares in capture directions if they hold pieces */
			foreach (var potentialCaptureSquare in potentialCaptureSquares)
			{
				if (potentialCaptureSquare.isOccupied)
				{
					Piece piece = potentialCaptureSquare.piece.Value;
					
					if (piece.color != this.color) {
						captureSquares.Add(potentialCaptureSquare);
					}
				}
			}

			var squares = captureSquares.Concat(emptySquares).ToList();
			return squares;
		}

	    public override void move(RankAndFile destination)  
	    {
			//todo add move legality checking
		    base.move(destination);
		}

    }
}