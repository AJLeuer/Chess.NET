using System;
using System.Collections.Generic;
using Chess.Util;
using static Chess.Game.Color;

namespace Chess.Game
{
	public class Board : ICloneable 
	{

		protected static ulong IDs = 0;

		protected ulong ID { get; } = IDs++;
		
		public virtual Vec2<uint> maxPosition
		{
			get { return new Vec2<uint>((uint) model.Length - 1, (uint) model[0].Length - 1); }
		}

		protected Square[][] model { get; private set; }

		public BasicGame game { get; set; }

		public Board()
		{
			model = new Square[][]
			{
				new Square[] { new Square('♜', 'a', 8, this), new Square('♟', 'a', 7, this), new Square(' ', 'a', 6, this), new Square(' ', 'a', 5, this), new Square(' ', 'a', 4, this), new Square(' ', 'a', 3, this), new Square('♙', 'a', 2, this), new Square('♖', 'a', 1, this) },
				
				new Square[] { new Square('♞', 'b', 8, this), new Square('♟', 'b', 7, this), new Square(' ', 'b', 6, this), new Square(' ', 'b', 5, this), new Square(' ', 'b', 4, this), new Square(' ', 'b', 3, this), new Square('♙', 'b', 2, this), new Square('♘', 'b', 1, this) },
				
				new Square[] { new Square('♝', 'c', 8, this), new Square('♟', 'c', 7, this), new Square(' ', 'c', 6, this), new Square(' ', 'c', 5, this), new Square(' ', 'c', 4, this), new Square(' ', 'c', 3, this), new Square('♙', 'c', 2, this), new Square('♗', 'c', 1, this) },
				
				new Square[] { new Square('♛', 'd', 8, this), new Square('♟', 'd', 7, this), new Square(' ', 'd', 6, this), new Square(' ', 'd', 5, this), new Square(' ', 'd', 4, this), new Square(' ', 'd', 3, this), new Square('♙', 'd', 2, this), new Square('♕', 'd', 1, this) },
				
				new Square[] { new Square('♚', 'e', 8, this), new Square('♟', 'e', 7, this), new Square(' ', 'e', 6, this), new Square(' ', 'e', 5, this), new Square(' ', 'e', 4, this), new Square(' ', 'e', 3, this), new Square('♙', 'e', 2, this), new Square('♔', 'e', 1, this) },
				
				new Square[] { new Square('♝', 'f', 8, this), new Square('♟', 'f', 7, this), new Square(' ', 'f', 6, this), new Square(' ', 'f', 5, this), new Square(' ', 'f', 4, this), new Square(' ', 'f', 3, this), new Square('♙', 'f', 2, this), new Square('♗', 'f', 1, this) },
				
				new Square[] { new Square('♞', 'g', 8, this), new Square('♟', 'g', 7, this), new Square(' ', 'g', 6, this), new Square(' ', 'g', 5, this), new Square(' ', 'g', 4, this), new Square(' ', 'g', 3, this), new Square('♙', 'g', 2, this), new Square('♘', 'g', 1, this) },
				
				new Square[] { new Square('♜', 'h', 8, this), new Square('♟', 'h', 7, this), new Square(' ', 'h', 6, this), new Square(' ', 'h', 5, this), new Square(' ', 'h', 4, this), new Square(' ', 'h', 3, this), new Square('♙', 'h', 2, this), new Square('♖', 'h', 1, this) }

			};
		}

		public Board(Board other)
		{
			// ReSharper disable once CoVariantArrayConversion
			this.model = (Square[][]) other.model.DeepClone();
			updateSquaresAfterCopy();
		}

		public static implicit operator Square[][](Board board)
		{
			return board.model;
		}

		object ICloneable.Clone()
		{
			return new Board(this);
		}

		public Board Clone()
		{
			return new Board(this);
		}
		
		protected void updateSquaresAfterCopy()
		{
			foreach (var file in model)
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
			return model[x][y];
		}

		/**
		 * @param pos This function checks whether pos is within the bounds of the Chess board
		 *
		 * @return true if pos exists on the board, false otherwise
		 */
		public virtual bool isInsideBounds(Vec2<uint> position)
		{
			if (position.x < model.Length)
			{
				return position.y < model[position.x].Length;
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
							Piece nextPiece = nextSquare.piece.Value;

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
							Piece nextPiece = nextSquare.piece.Value;
							
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
			
			Square square = findMatchingSquare(piece.square);

			if (square.piece.HasValue) {

				if (square.piece.GetType() == piece.GetType()) {
					return square.piece.Value;
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

			foreach (var file in model)
			{
				foreach (var square in file)
				{
					if (square.piece.HasValue)
					{
						Piece piece = square.piece.Value;
						
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