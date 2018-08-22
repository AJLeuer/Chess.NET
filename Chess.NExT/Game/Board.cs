using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess.Util;
using static Chess.Game.Color;

namespace Chess.Game
{
	public class Board : ICloneable, IEnumerable
	{

		protected static ulong IDs = 0;

		protected ulong ID { get; } = IDs++;
		
		public virtual Vec2<uint> maxPosition
		{
			get { return new Vec2<uint>((uint) Squares.Length - 1, (uint) Squares[0].Length - 1); }
		}
		
		protected static readonly Square[][] DefaultSquares = new Square[][]
		{
			new Square[] { new Square('♜', 'a', 8), new Square('♟', 'a', 7), new Square(' ', 'a', 6), new Square(' ', 'a', 5), new Square(' ', 'a', 4), new Square(' ', 'a', 3), new Square('♙', 'a', 2), new Square('♖', 'a', 1) },
			
			new Square[] { new Square('♞', 'b', 8), new Square('♟', 'b', 7), new Square(' ', 'b', 6), new Square(' ', 'b', 5), new Square(' ', 'b', 4), new Square(' ', 'b', 3), new Square('♙', 'b', 2), new Square('♘', 'b', 1) },
			
			new Square[] { new Square('♝', 'c', 8), new Square('♟', 'c', 7), new Square(' ', 'c', 6), new Square(' ', 'c', 5), new Square(' ', 'c', 4), new Square(' ', 'c', 3), new Square('♙', 'c', 2), new Square('♗', 'c', 1) },
			
			new Square[] { new Square('♛', 'd', 8), new Square('♟', 'd', 7), new Square(' ', 'd', 6), new Square(' ', 'd', 5), new Square(' ', 'd', 4), new Square(' ', 'd', 3), new Square('♙', 'd', 2), new Square('♕', 'd', 1) },
			
			new Square[] { new Square('♚', 'e', 8), new Square('♟', 'e', 7), new Square(' ', 'e', 6), new Square(' ', 'e', 5), new Square(' ', 'e', 4), new Square(' ', 'e', 3), new Square('♙', 'e', 2), new Square('♔', 'e', 1) },
			
			new Square[] { new Square('♝', 'f', 8), new Square('♟', 'f', 7), new Square(' ', 'f', 6), new Square(' ', 'f', 5), new Square(' ', 'f', 4), new Square(' ', 'f', 3), new Square('♙', 'f', 2), new Square('♗', 'f', 1) },
			
			new Square[] { new Square('♞', 'g', 8), new Square('♟', 'g', 7), new Square(' ', 'g', 6), new Square(' ', 'g', 5), new Square(' ', 'g', 4), new Square(' ', 'g', 3), new Square('♙', 'g', 2), new Square('♘', 'g', 1) },
			
			new Square[] { new Square('♜', 'h', 8), new Square('♟', 'h', 7), new Square(' ', 'h', 6), new Square(' ', 'h', 5), new Square(' ', 'h', 4), new Square(' ', 'h', 3), new Square('♙', 'h', 2), new Square('♖', 'h', 1) }
		};

		protected Square[][] squares;

		public Square[][] Squares
		{
			get { return squares; }

			internal set
			{
				squares = value;
				takeOwnershipOfSquares();
			}
		}

		public BasicGame game { get; set; }

		public Board() :
			this(DefaultSquares)
		{
			
		}

		public Board(List<List<Square>> squares):
			this(squares.Select(l => l.ToArray()).ToArray())
		{
			
		}

		public Board(Square[][] squares)
		{
			this.Squares = squares;
		}

		public Board(Board other)
		{
			// ReSharper disable once CoVariantArrayConversion
			this.Squares = (Square[][]) other.Squares.DeepClone();
		}

		public static implicit operator Square[][](Board board)
		{
			return board.Squares;
		}
		
		public virtual IEnumerator GetEnumerator()
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

		/**
		 * @return The Square at the position specified by rf
		 */
		public virtual Square getSquare(RankAndFile rankAndFile)
		{
			return getSquare((Vec2<uint>) rankAndFile);
		} 

		/**
		 * @param pos The position of the Square to return
		 *
		 * @return A pointer to the Square at the position specified by pos
		 */
		protected virtual Square getSquare(Vec2<uint> position)
		{
			return getSquare(position.x, position.y);
		}

		public virtual Square getSquare(uint x, uint y)
		{
			return Squares[x][y];
		}

		/**
		 * @param pos This function checks whether pos is within the bounds of the Chess board
		 *
		 * @return true if pos exists on the board, false otherwise
		 */
		public virtual bool isInsideBounds(Vec2<uint> position)
		{
			if (position.x < Squares.Length)
			{
				return position.y < Squares[position.x].Length;
			}
			else {
				return false;
			}
		}

		public List<Square> getSpecifiedSquares(bool includeFirstPieceOfColorEncountered, Color colorToInclude,
			Vec2<uint> startingSquarePosition, params Direction[] directions)
		{
			var squares = new List<Square>();

			for (var i = 0; i < directions.Length; i++) {

				Vec2<short> offset = directions[i]; //directions convert to vectors like (0, 1)

				for (Vec2<long> next = (startingSquarePosition + offset) ; ; next += offset)
				{

					Vec2<uint> nextPosition = next.ConvertMemberType<uint>();
					
					if (isInsideBounds(nextPosition)) {

						Square nextSquare = getSquare(nextPosition);


						if (nextSquare.isOccupied)
						{
							Piece nextPiece = nextSquare.Piece.Value;

							if ((includeFirstPieceOfColorEncountered) &&
							    (nextPiece.color == colorToInclude)) 
							{
								squares.Add(nextSquare);
							}
							break;
						}
						else {
							squares.Add(nextSquare);
						}

					}

					else /* if (isInsideBoardBounds(next) == false) */ { //we've gone outside the bounds of the board. we can can safely break out of this loop and avoid more pointless searching, since we know there's nothing in this direction
						break;
					}
				}
			}

			return squares;
		}


		public List<Square> getSpecifiedSquares(uint maxSearchDistance, bool includeFirstPieceOfColorEncountered,
			Color colorToInclude, Vec2<uint> startingSquarePosition, params Direction[] directions)
		{
			var squares = new List<Square>();

			for (var i = 0; i < directions.Length; i++) {

				Vec2<short> offset = directions[i];  //directions convert to vectors like (0, 1)

				var next = startingSquarePosition + offset;
				var endpoint = startingSquarePosition + (offset * maxSearchDistance);

				for ( ; ; next += offset)
				{

					Vec2<uint> nextPosition = next.ConvertMemberType<uint>();

					if (isInsideBounds(nextPosition)) {

						Square nextSquare = getSquare(nextPosition);

						if (nextSquare.isOccupied) 
						{ 
							Piece nextPiece = nextSquare.Piece.Value;
							
							if ((includeFirstPieceOfColorEncountered) &&
							    (nextPiece.color == colorToInclude)) 
							{
								squares.Add(nextSquare);
							}
							/* Else this is our stopping point */
							break;
						}
						else {
							squares.Add(nextSquare);
						}

						if (next == endpoint) {
							break;
						}
					}

					else /* if (isInsideBoardBounds(next) == false) */ { //we've gone outside the bounds of the board. we can can safely break out of this loop and avoid more pointless searching, since we know there's nothing in this direction
						break;
					}
				}
			}

			return squares;
		}


		/**
		 * @return A vector of Squares that match the given criteria
		 *
		 * @param startingSquarePosition The position of the first square processed
		 * @param searchRadius Specifies the range of other squares to include
		 */
		public List<Square> getSpecifiedSquares(Vec2<uint> startingSquarePosition, int searchRadius)
		{
			var squares = new List<Square>();
			
			for (Vec2<long> index = new Vec2<long>((startingSquarePosition.x - searchRadius),(startingSquarePosition.y - searchRadius));
				index.x <= (startingSquarePosition.x + searchRadius); index.x++) 
			{

				for (index.y = (startingSquarePosition.y - searchRadius);
					index.y <= (startingSquarePosition.y + searchRadius); index.y++)
				{
					var currentIndex = index.ConvertMemberType<uint>();
						
					if (isInsideBounds(currentIndex)) 
					{
						Square square = getSquare(currentIndex);
						squares.Add(square);
					}

				}

			}

			return squares;
		}

		public Square findMatchingSquare(Square square)
		{
			Square ownSquare = getSquare(square.rankAndFile);
			return ownSquare;
		}

		/**
		 * Finds the Piece on this board that matches the given argument. There are two requirements for a match:
		 * first that the two pieces are at the same position on their respective boards (and they may belong to different boards),
		 * second is that they are of the same type. Note that this function does not verify that they are the same object (and indeed
		 * the very purpose of this function is such that in most cases the argument and the return value should point to different objects
		 * entirely).
		 *
		 * @param piece The piece to match
		 */
		public Piece findMatchingPiece(Piece piece)
		{
			
			Square square = findMatchingSquare(piece.Square);

			if (square.Piece.HasValue) 
			{

				if (square.Piece.GetType() == piece.GetType()) {
					return square.Piece.Value;
				}
			}

			throw new ArgumentException("Matching piece not found on this board");
		}


		/**
		 * Calculates a numeric value based on the current state of the chess board (including the existence and configuration of pieces)m
		 * from the perspective of the player playing the Chess::Color color. In other words, if e.g. the player playing white requests the current
		 * value of the board, it will be calculated by subtracting the sum of the extant black pieces from the sum of the remaining white ones.
		 *
		 * @param playerColor The color of the player from whose perspective the value of the game state is calculated
		 */
		public virtual short evaluate(Player player)
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

		/**
		 * Same as running evaluate() after moving piece to the specified RankAndFile. Does not actually change the state of the board.
		 *
		 * @param callingPlayersColor The color of the player from whose perspective the value of the game state is calculated
		 * @param piece The Piece that would move
		 * @param moveTo Where piece would move
		 */
		public short evaluateAfterHypotheticalMove(Player player, Piece piece, RankAndFile destination)
		{
			
			Board testBoard = this.Clone();

			Piece testPiece = testBoard.findMatchingPiece(piece);

			testPiece.move(destination);

			return testBoard.evaluate(player);
		}
	}

}