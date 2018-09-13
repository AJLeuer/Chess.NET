using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using C5;

using Chess.Util;
using static Chess.Util.Util;
using static Chess.Game.Color;

using File = System.Char;
using Rank = System.UInt16;

namespace Chess.Game
{
	
	public class Board : ICloneable , IEnumerable< ArrayList<Square> >
	{
		protected static ulong IDs = 0;

		protected ulong ID { get; } = IDs++;
		
		public virtual Vec2<uint> maxPosition
		{
			get { return new Vec2<uint>((uint) Squares.Count - 1, (uint) Squares[0].Count - 1); }
		}

		public static readonly ArrayList< ArrayList<Square> > DefaultStartingSquares = new ArrayList< ArrayList<Square> >
		{
			new ArrayList<Square> { new Square('♜', 'a', 8), new Square('♟', 'a', 7), new Square(' ', 'a', 6), new Square(' ', 'a', 5), new Square(' ', 'a', 4), new Square(' ', 'a', 3), new Square('♙', 'a', 2), new Square('♖', 'a', 1) },
			
			new ArrayList<Square> { new Square('♞', 'b', 8), new Square('♟', 'b', 7), new Square(' ', 'b', 6), new Square(' ', 'b', 5), new Square(' ', 'b', 4), new Square(' ', 'b', 3), new Square('♙', 'b', 2), new Square('♘', 'b', 1) },
			
			new ArrayList<Square> { new Square('♝', 'c', 8), new Square('♟', 'c', 7), new Square(' ', 'c', 6), new Square(' ', 'c', 5), new Square(' ', 'c', 4), new Square(' ', 'c', 3), new Square('♙', 'c', 2), new Square('♗', 'c', 1) },
			
			new ArrayList<Square> { new Square('♛', 'd', 8), new Square('♟', 'd', 7), new Square(' ', 'd', 6), new Square(' ', 'd', 5), new Square(' ', 'd', 4), new Square(' ', 'd', 3), new Square('♙', 'd', 2), new Square('♕', 'd', 1) },
			
			new ArrayList<Square> { new Square('♚', 'e', 8), new Square('♟', 'e', 7), new Square(' ', 'e', 6), new Square(' ', 'e', 5), new Square(' ', 'e', 4), new Square(' ', 'e', 3), new Square('♙', 'e', 2), new Square('♔', 'e', 1) },
			
			new ArrayList<Square> { new Square('♝', 'f', 8), new Square('♟', 'f', 7), new Square(' ', 'f', 6), new Square(' ', 'f', 5), new Square(' ', 'f', 4), new Square(' ', 'f', 3), new Square('♙', 'f', 2), new Square('♗', 'f', 1) },
			
			new ArrayList<Square> { new Square('♞', 'g', 8), new Square('♟', 'g', 7), new Square(' ', 'g', 6), new Square(' ', 'g', 5), new Square(' ', 'g', 4), new Square(' ', 'g', 3), new Square('♙', 'g', 2), new Square('♘', 'g', 1) },
			
			new ArrayList<Square> { new Square('♜', 'h', 8), new Square('♟', 'h', 7), new Square(' ', 'h', 6), new Square(' ', 'h', 5), new Square(' ', 'h', 4), new Square(' ', 'h', 3), new Square('♙', 'h', 2), new Square('♖', 'h', 1) }
		};
		
		public static readonly ArrayList< ArrayList<Square> > EmptySquares = new ArrayList< ArrayList<Square> >
		{
			new ArrayList<Square> { new Square(' ', 'a', 8), new Square(' ', 'a', 7), new Square(' ', 'a', 6), new Square(' ', 'a', 5), new Square(' ', 'a', 4), new Square(' ', 'a', 3), new Square(' ', 'a', 2), new Square(' ', 'a', 1) },
			
			new ArrayList<Square> { new Square(' ', 'b', 8), new Square(' ', 'b', 7), new Square(' ', 'b', 6), new Square(' ', 'b', 5), new Square(' ', 'b', 4), new Square(' ', 'b', 3), new Square(' ', 'b', 2), new Square(' ', 'b', 1) },
			
			new ArrayList<Square> { new Square(' ', 'c', 8), new Square(' ', 'c', 7), new Square(' ', 'c', 6), new Square(' ', 'c', 5), new Square(' ', 'c', 4), new Square(' ', 'c', 3), new Square(' ', 'c', 2), new Square(' ', 'c', 1) },
			
			new ArrayList<Square> { new Square(' ', 'd', 8), new Square(' ', 'd', 7), new Square(' ', 'd', 6), new Square(' ', 'd', 5), new Square(' ', 'd', 4), new Square(' ', 'd', 3), new Square(' ', 'd', 2), new Square(' ', 'd', 1) },
			
			new ArrayList<Square> { new Square(' ', 'e', 8), new Square(' ', 'e', 7), new Square(' ', 'e', 6), new Square(' ', 'e', 5), new Square(' ', 'e', 4), new Square(' ', 'e', 3), new Square(' ', 'e', 2), new Square(' ', 'e', 1) },
			
			new ArrayList<Square> { new Square(' ', 'f', 8), new Square(' ', 'f', 7), new Square(' ', 'f', 6), new Square(' ', 'f', 5), new Square(' ', 'f', 4), new Square(' ', 'f', 3), new Square(' ', 'f', 2), new Square(' ', 'f', 1) },
			
			new ArrayList<Square> { new Square(' ', 'g', 8), new Square(' ', 'g', 7), new Square(' ', 'g', 6), new Square(' ', 'g', 5), new Square(' ', 'g', 4), new Square(' ', 'g', 3), new Square(' ', 'g', 2), new Square(' ', 'g', 1) },
			
			new ArrayList<Square> { new Square(' ', 'h', 8), new Square(' ', 'h', 7), new Square(' ', 'h', 6), new Square(' ', 'h', 5), new Square(' ', 'h', 4), new Square(' ', 'h', 3), new Square(' ', 'h', 2), new Square(' ', 'h', 1) }
		};

		protected ArrayList< ArrayList<Square> > squares;

		public ArrayList< ArrayList<Square> > Squares
		{
			get { return squares; }

			protected set
			{
				squares = value;
				takeOwnershipOfSquares();
			}
		}

		public BasicGame game { get; set; }

		public Board() :
			this(DefaultStartingSquares)
		{
			
		}
		
		public Board(Board other)
		{
			Square[][] otherSquaresArray = other.Squares.Select( (ArrayList<Square> file) => file.ToArray()).ToArray();
			// ReSharper disable once CoVariantArrayConversion
			Square[][] otherSquaresClone =  (Square[][]) otherSquaresArray.DeepClone();

			this.Squares = CreateFrom2DArray(otherSquaresClone);
		}

		public Board(ArrayList< ArrayList<Square> > squares)
		{
			this.Squares = squares;
		}

		public Square this[RankAndFile boardPosition]
		{
			get
			{
				return this[boardPosition.file, boardPosition.rank];
			}
			set
			{
				this[boardPosition.file, boardPosition.rank] = value;
			}
		}
				
		public Square this[File file, Rank rank]
		{
			get
			{
				return this[RankAndFile.convertToInteger(file: file)][(int) RankAndFile.convertToInteger(rank: rank)];
			}
			set
			{
				this[RankAndFile.convertToInteger(file: file)][(int) RankAndFile.convertToInteger(rank: rank)] = value;
			}
		}

		protected ArrayList<Square> this[uint index]
		{
			get
			{
				return Squares[(int)index];
			}
			set
			{
				Squares[(int)index] = value;
			}
		}
		
		///<return>The Square at the position specified by boardPosition</return>
		public virtual Square getSquare(RankAndFile boardPosition)
		{
			return this[boardPosition.file, boardPosition.rank];
		} 
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Squares.GetEnumerator();
		}
		
		public virtual IEnumerator<ArrayList<Square>> GetEnumerator()
		{
			return Squares.GetEnumerator();
		}

		object ICloneable.Clone()
		{
			return new Board(this);
		}

		public Board Clone()
		{
			return new Board(this);
		}
		
		internal void initializeSpriteTextures()
		{
			foreach (var file in this)
			{
				foreach (Square square in file)
				{
					var piece = square.Piece;

					if (piece.HasValue)
					{
						piece.Value.initializeSpriteTexture();
					}
				}
			}
		}
		
		protected void takeOwnershipOfSquares()
		{
			if (Squares == null)
			{
				return;
			}
			
			foreach (var file in Squares)
			{
				foreach (var square in file)
				{
					square.board = this;
				}
			}	
		}

		
		/// <param name="position">This function checks whether <paramref name="position"/> is within the bounds of the Chess board</param>
		///
		/// <return>true if position exists on the board, false otherwise</return>
		public virtual bool IsInsideBounds(Vec2<int> position)
		{
			if ((position.x >= 0) && (position.x < Squares.Count))
			{
				return ((position.y >= 0) && (position.y < Squares[position.x].Count));
			}
			else {
				return false;
			}
		}

		public List<Square> SearchForSquares(Predicate<Square> squareMatcher, Vec2<uint> startingSquarePosition,
			ushort distance = 1, params Direction[] directions)
		{
			var matchingSquares = new List<Square>();
			
			foreach (var direction in directions)
			{
				var squaresInCurrentDirection = searchForSquaresInGivenDirection(squareMatcher, 
					startingSquarePosition, (short) distance, direction);
				
				matchingSquares.AddRange(squaresInCurrentDirection);
			}

			return matchingSquares;
		}

		private IEnumerable<Square> searchForSquaresInGivenDirection(Predicate<Square> squareMatcher, Vec2<uint> startingSquarePosition, short maximumDistance, Direction direction)
		{
			var matchingSquares = new List<Square>();

			for (short distance = 1; distance <= maximumDistance; distance++)
			{
				Vec2<long> position = startingSquarePosition + ((Vec2<short>) direction * distance);
				//todo: can't convert to uint here if we have negative coord
				Vec2<int> boardPosition = position.ConvertMemberType<int>();
				
				Optional<Square> square = checkForMatchingSquare(squareMatcher, boardPosition);
			
				if (square.HasValue)
				{
					matchingSquares.Add(square.Value);

					if (square.Value.isOccupied)
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			
			return matchingSquares;
		}

		private Optional<Square> checkForMatchingSquare(Predicate<Square> squareMatcher, Vec2<int> position)
		{
			if (IsInsideBounds(position))
			{
				Square square = this[position.ConvertMemberType<uint>()];

				if (squareMatcher(square))
				{
					return square;
				}
				else
				{
					return Optional<Square>.Empty;
				}
			}
			else
			{
				return Optional<Square>.Empty;
			}
		}
		
		public Square findMatchingSquare(Square square)
		{
			Square ownSquare = getSquare(square.boardPosition);
			return ownSquare;
		}

		/// <summary>Finds the Piece on this board that matches the given argument. There are three requirements for a match:
		///first that the two pieces are at the same position on their respective boards (and they may belong to different boards),
		///second is that they are of the same type, the third that they are the same color. Note that this function does
		/// not verify that they are the same object (and indeed the very purpose of this function is such that in most
		/// cases the argument and the return value should point to different objects entirely)</summary>
		///
		/// <param name="piece">The piece to match</param>
		public Piece findMatchingPiece(Piece piece)
		{
			Square square = findMatchingSquare(piece.Square);

			if (square.Piece.HasValue)
			{
				Piece potentialMatchingPiece = square.Piece.Value;
				
				if ((potentialMatchingPiece.GetType() == piece.GetType()) && (potentialMatchingPiece.color == piece.color)) {
					return potentialMatchingPiece;
				}
			}

			throw new ArgumentException("Matching piece not found on this board");
		}


		/// <summary>Calculates the value of the current state of the board from the perspective of Player player</summary>
		///
		/// <param name="player">The player from whose perspective the value of the game state is calculated</param>
		public virtual short CalculateRelativeValue(Player player)
		{	
			short blackSum = 0;
			short whiteSum = 0;

			foreach (var file in Squares)
			{
				foreach (var square in file)
				{
					if (square.Piece.HasValue)
					{
						Piece piece = square.Piece.Value;
						
						if (piece.color == black)
						{
							blackSum += (short)piece.value;
						}
						else /* if (piece.color == white) */
						{
							whiteSum += (short)piece.value;
						}
					}
				}
			}

			if (player.color == black) {
				short result = (short)(blackSum - whiteSum);
				return result;
			}
			else /* if (player.color == white) */ {
				short result = (short)(whiteSum - blackSum);
				return result;
			}
		}

		/// <summary>Same as running CalculateRelativeValue() after moving piece to the specified RankAndFile. Does not actually change the state of the board.</summary>
		///
		/// <param name="player">The player from whose perspective the value of the game state is calculated</param>
		/// <param name="piece">The Piece that would move</param>
		/// <param name="destination">Where piece would move</param>
		public short evaluateAfterHypotheticalMove(Player player, Piece piece, RankAndFile destination)
		{
			
			Board testBoard = this.Clone();

			Piece testPiece = testBoard.findMatchingPiece(piece);

			testPiece.move(destination);

			return testBoard.CalculateRelativeValue(player);
		}
	}

}