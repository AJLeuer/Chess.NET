using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using C5;

using Chess.Util;
using Chess.View;
using SFML.Graphics;
using static Chess.Game.Color;

using File = System.Char;
using Rank = System.UInt16;

namespace Chess.Game
{
	public class Board : ICloneable, IEnumerable<Square>, ChessDrawable
	{
		public static readonly Square[,] DefaultStartingSquares = new Square[,]
		{
			{ new Square('♖'), new Square('♙'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♟'), new Square('♜') },
			
			{ new Square('♘'), new Square('♙'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♟'), new Square('♞') },
			
			{ new Square('♗'), new Square('♙'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♟'), new Square('♝') },
			
			{ new Square('♕'), new Square('♙'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♟'), new Square('♛') },
			
			{ new Square('♔'), new Square('♙'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♟'), new Square('♚') },
			
			{ new Square('♗'), new Square('♙'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♟'), new Square('♝') },
			
			{ new Square('♘'), new Square('♙'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♟'), new Square('♞') },
			
			{ new Square('♖'), new Square('♙'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♟'), new Square('♜') }
		};
		
		public static readonly Square[,] EmptySquares = new Square[,]
		{
			{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
			
			{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
			
			{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
			
			{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
			
			{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
			
			{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
			
			{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
			
			{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') }
		};
		
		protected static ulong IDs = 0;
		
		protected ulong ID { get; } = IDs++;

		public virtual Vec2<uint> MaxPosition
		{
			get { return new Vec2<uint>((uint) Squares.GetLength(0) - 1, (uint) Squares.GetLength(1) - 1); }
		}

		private SquareGrid squares;

		public virtual Square[,] Squares
		{
			get { return squares; }

			set
			{
				squares = value;
				takeOwnershipOfSquares();
			}
		}
				
		public Sprite Sprite { get; set; }
		
		public Size Size
		{
			get { return Sprite.Texture.Size; }
		}
		
		public Vec2<uint> Position2D
		{
			get { return Sprite.Position; }
			set { Sprite.Position = value; }
		}
		
		public BasicGame Game { get; set; }

		public Board() :
			this(DefaultStartingSquares)
		{
			
		}
		
		public Board(Board other):
			// ReSharper disable once CoVariantArrayConversion
			this((Square[,]) other.Squares.DeepClone())
		{

		}

		public Board(Square[,] squares)
		{
			this.Squares = squares;
		}

		public Square this[RankFile rankAndFile]
		{
			get
			{
				Vec2<uint> position = rankAndFile;
				return Squares[position.X, position.Y];
			}
			set
			{
				Vec2<uint> position = rankAndFile;
				Squares[position.X, position.Y] = value;
			}
		}
				
		public Square this[File file, Rank rank]
		{
			get
			{
				var rankAndFile = new RankFile(file, rank);
				return this[rankAndFile];
			}
			set
			{
				var rankAndFile = new RankFile(file, rank);
				this[rankAndFile] = value;
			}
		}
		
		public static implicit operator Square[,] (Board board)
		{
			return board.Squares;
		}
		
		public IEnumerator<Square> GetEnumerator()
		{
			return Squares.OfType<Square>().GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public Board Clone()
		{
			return new Board(this);
		}

		public void InitializeGraphicalElements()
		{
			var spriteTexture = new Texture(Config.BoardSpriteFilePath);
			Sprite = new Sprite(spriteTexture);

			foreach (var square in Squares)
			{
				square.InitializeGraphicalElements();
			}
		}

		public void Initialize2DPosition(Vec2<uint> position)
		{
			this.Position2D = position;

			Vec2<uint> squareOrigin = Position2D;
			
			for (uint i = 0; i < Squares.GetLength(0); i++)
			{
				Square square = null;
				for (uint j = 0; j < Squares.GetLength(1); j++)
				{
					square = Squares[i, j];

					square.Initialize2DPosition(squareOrigin);

					squareOrigin.Y += square.Size.Height;
				}
				
				// ReSharper disable once PossibleNullReferenceException
				squareOrigin.X += square.Size.Width;
				squareOrigin.Y = Position2D.Y;
			}
		}
		
		private void takeOwnershipOfSquares()
		{
			if (Squares == null)
			{
				return;
			}
			
			foreach (var square in Squares)
			{
				square.Board = this;
			}	
		}
		
		/// <param name="position">This function checks whether <paramref name="position"/> is within the bounds of the chess board</param>
		///
		/// <return>true if position exists on the board, false otherwise</return>
		public virtual bool IsInsideBounds(Vec2<int> position)
		{
			if ((position.X >= 0) && (position.X < Squares.GetLength(0)))
			{
				return ((position.Y >= 0) && (position.Y < Squares.GetLength(1)));
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
				var squaresInCurrentDirection = searchForAvailableSquaresInGivenDirection(squareMatcher, 
					startingSquarePosition, (short) distance, direction);
				
				matchingSquares.AddRange(squaresInCurrentDirection);
			}

			return matchingSquares;
		}

		private List<Square> searchForAvailableSquaresInGivenDirection(Predicate<Square> squareMatcher, Vec2<uint> startingSquarePosition, short maximumDistance, Direction direction)
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
					matchingSquares.Add(square.Object);

					if (square.Object.isOccupied)
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
		
		public Square FindMatchingSquare(Square square)
		{
			Square ownSquare = this[square.RankAndFile];
			return ownSquare;
		}

		/// <summary>Finds the Piece on this board that matches the given argument. There are three requirements for a match:
		///first that the two pieces are at the same position on their respective boards (and they may belong to different boards),
		///second is that they are of the same type, the third that they are the same color. Note that this function does
		/// not verify that they are the same object (and indeed the very purpose of this function is such that in most
		/// cases the argument and the return value should point to different objects entirely)</summary>
		///
		/// <param name="piece">The piece to match</param>
		public Piece FindMatchingPiece(Piece piece)
		{
			Square square = FindMatchingSquare(piece.Square);

			if (square.Piece.HasValue)
			{
				Piece potentialMatchingPiece = square.Piece.Object;
				
				if ((potentialMatchingPiece.GetType() == piece.GetType()) && (potentialMatchingPiece.Color == piece.Color)) {
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
			var (blackSum, whiteSum) = CalculateAbsoluteValueToPlayers();

			if (player.Color == black) 
			{
				short result = (short)(blackSum - whiteSum);
				return result;
			}
			else /* if (player.color == white) */ 
			{
				short result = (short)(whiteSum - blackSum);
				return result;
			}
		}

		public (short valueToBlack, short valueToWhite) CalculateAbsoluteValueToPlayers()
		{
			short valueToBlack = 0;
			short valueToWhite = 0;

			foreach (var square in Squares)
			{
				if (square.Piece.HasValue)
				{
					Piece piece = square.Piece.Object;

					if (piece.Color == black)
					{
						valueToBlack += (short) piece.Value;
					}
					else /* if (piece.color == white) */
					{
						valueToWhite += (short) piece.Value;
					}
				}
			}

			return (valueToBlack, valueToWhite);
		}
	}

	public class SquareGrid
	{
		private Square[,] squares;

		private SquareGrid(Square[,] squares)
		{
			this.squares = squares;
			
			for (uint i = 0; i < this.squares.GetLength(0); i++)
			{
				for (uint j = 0; j < this.squares.GetLength(1); j++)
				{
					Square square = this.squares[i, j];
					square.BoardPosition = new Vec2<uint>(i, j);
				}
			}
		}

		public static implicit operator Square[,] (SquareGrid grid)
		{
			return grid.squares;
		}
		
		public static implicit operator SquareGrid (Square[,] squares)
		{
			return new SquareGrid(squares);
		}
	}

}