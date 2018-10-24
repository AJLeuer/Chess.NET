using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SFML.Graphics;

using Chess.Util;
using Chess.View;
using static Chess.Game.Color;

using Position = Chess.Util.Vec2<uint>;
using File = System.Char;
using Rank = System.UInt16;

namespace Chess.Game
{
	public abstract class Board : ICloneable, IEnumerable<Square>
	{
		public static readonly Square[,] DefaultStartingSquares = new Square[,]
		{
			{ new Square('♖', 'a', 1), new Square('♙', 'a', 2), new Square(' ', 'a', 3), new Square(' ', 'a', 4), new Square(' ', 'a', 5), new Square(' ', 'a', 6), new Square('♟', 'a', 7), new Square('♜', 'a', 8) },
			
			{ new Square('♘', 'b', 1), new Square('♙', 'b', 2), new Square(' ', 'b', 3), new Square(' ', 'b', 4), new Square(' ', 'b', 5), new Square(' ', 'b', 6), new Square('♟', 'b', 7), new Square('♞', 'b', 8) },
			
			{ new Square('♗', 'c', 1), new Square('♙', 'c', 2), new Square(' ', 'c', 3), new Square(' ', 'c', 4), new Square(' ', 'c', 5), new Square(' ', 'c', 6), new Square('♟', 'c', 7), new Square('♝', 'c', 8) },
			
			{ new Square('♕', 'd', 1), new Square('♙', 'd', 2), new Square(' ', 'd', 3), new Square(' ', 'd', 4), new Square(' ', 'd', 5), new Square(' ', 'd', 6), new Square('♟', 'd', 7), new Square('♛', 'd', 8) },
			
			{ new Square('♔', 'e', 1), new Square('♙', 'e', 2), new Square(' ', 'e', 3), new Square(' ', 'e', 4), new Square(' ', 'e', 5), new Square(' ', 'e', 6), new Square('♟', 'e', 7), new Square('♚', 'e', 8) },
			
			{ new Square('♗', 'f', 1), new Square('♙', 'f', 2), new Square(' ', 'f', 3), new Square(' ', 'f', 4), new Square(' ', 'f', 5), new Square(' ', 'f', 6), new Square('♟', 'f', 7), new Square('♝', 'f', 8) },
			
			{ new Square('♘', 'g', 1), new Square('♙', 'g', 2), new Square(' ', 'g', 3), new Square(' ', 'g', 4), new Square(' ', 'g', 5), new Square(' ', 'g', 6), new Square('♟', 'g', 7), new Square('♞', 'g', 8) },
			
			{ new Square('♖', 'h', 1), new Square('♙', 'h', 2), new Square(' ', 'h', 3), new Square(' ', 'h', 4), new Square(' ', 'h', 5), new Square(' ', 'h', 6), new Square('♟', 'h', 7), new Square('♜', 'h', 8) }
		};
		
		public static readonly Square[,] DefaultEmptySquares = new Square[,]
		{
			{ new Square(' ', 'a', 1), new Square(' ', 'a', 2), new Square(' ', 'a', 3), new Square(' ', 'a', 4), new Square(' ', 'a', 5), new Square(' ', 'a', 6), new Square(' ', 'a', 7), new Square(' ', 'a', 8) },
			
			{ new Square(' ', 'b', 1), new Square(' ', 'b', 2), new Square(' ', 'b', 3), new Square(' ', 'b', 4), new Square(' ', 'b', 5), new Square(' ', 'b', 6), new Square(' ', 'b', 7), new Square(' ', 'b', 8) },
			
			{ new Square(' ', 'c', 1), new Square(' ', 'c', 2), new Square(' ', 'c', 3), new Square(' ', 'c', 4), new Square(' ', 'c', 5), new Square(' ', 'c', 6), new Square(' ', 'c', 7), new Square(' ', 'c', 8) },
			
			{ new Square(' ', 'd', 1), new Square(' ', 'd', 2), new Square(' ', 'd', 3), new Square(' ', 'd', 4), new Square(' ', 'd', 5), new Square(' ', 'd', 6), new Square(' ', 'd', 7), new Square(' ', 'd', 8) },
			
			{ new Square(' ', 'e', 1), new Square(' ', 'e', 2), new Square(' ', 'e', 3), new Square(' ', 'e', 4), new Square(' ', 'e', 5), new Square(' ', 'e', 6), new Square(' ', 'e', 7), new Square(' ', 'e', 8) },
			
			{ new Square(' ', 'f', 1), new Square(' ', 'f', 2), new Square(' ', 'f', 3), new Square(' ', 'f', 4), new Square(' ', 'f', 5), new Square(' ', 'f', 6), new Square(' ', 'f', 7), new Square(' ', 'f', 8) },
			
			{ new Square(' ', 'g', 1), new Square(' ', 'g', 2), new Square(' ', 'g', 3), new Square(' ', 'g', 4), new Square(' ', 'g', 5), new Square(' ', 'g', 6), new Square(' ', 'g', 7), new Square(' ', 'g', 8) },
			
			{ new Square(' ', 'h', 1), new Square(' ', 'h', 2), new Square(' ', 'h', 3), new Square(' ', 'h', 4), new Square(' ', 'h', 5), new Square(' ', 'h', 6), new Square(' ', 'h', 7), new Square(' ', 'h', 8) }
		};
		
		protected static ulong IDs = 0;
		
		protected ulong ID { get; } = IDs++;

		public Position MaxPosition
		{
			get { return new Position((uint) Squares.GetLength(0) - 1, (uint) Squares.GetLength(1) - 1); }
		}

		private SquareGrid squareGrid;

		protected virtual SquareGrid squares
		{
			get { return squareGrid; }
			
			set
			{
				squareGrid = value;
				takeOwnershipOfSquares();
			}
		}

		public abstract Square[,] Squares { get; set; }
				
		public BasicGame Game { get; set; }

		public Board() :
			this(DefaultStartingSquares)
		{
			
		}
		
		public Board(Board other):
			// ReSharper disable once CoVariantArrayConversion
			this(new SquareGrid(other.squares))
		{

		}

		public Board(SquareGrid squares)
		{
			this.Squares = squares;
		}

		public Square this[RankFile rankAndFile]
		{
			get
			{
				Position position = rankAndFile;
				return Squares[position.X, position.Y];
			}
			set
			{
				Position position = rankAndFile;
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
		
		public virtual IEnumerator<Square> GetEnumerator()
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

		public abstract Board Clone();

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
		
		public List<Square> SearchForSquares(Predicate<Square> squareMatcher, Position startingSquarePosition,
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

		private List<Square> searchForAvailableSquaresInGivenDirection(Predicate<Square> squareMatcher, Position startingSquarePosition, short maximumDistance, Direction direction)
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

	public class SquareGrid : ICloneable
	{
		protected Square[,] squares;

		public SquareGrid(Square[,] squares)
		{
			this.squares = squares;
			
			for (uint i = 0; i < this.squares.GetLength(0); i++)
			{
				for (uint j = 0; j < this.squares.GetLength(1); j++)
				{
					Square square = this.squares[i, j];

					if (square.BoardPosition != (i, j))
					{
						throw new Exception($"Square at ({i},{j}) was constructed with a position or rank and file that did not match its location in the squares array");
					}
				}
			}
		}

		public SquareGrid(SquareGrid other)
		{
			this.squares = new Square[8,8];
			copyFromSquares(other.squares);
		}
		
		object ICloneable.Clone()
		{
			return Clone();
		}

		public virtual SquareGrid Clone()
		{
			return new SquareGrid(this);
		}
		
		public static implicit operator Square[,] (SquareGrid grid)
		{
			return grid.squares;
		}
		
		public static implicit operator SquareGrid (Square[,] squares)
		{
			return new SquareGrid(squares);
		}

		protected virtual void copyFromSquares(Square[,] otherSquares)
		{
			for (uint i = 0; i < otherSquares.GetLength(0); i++)
			{
				for (uint j = 0; j < otherSquares.GetLength(1); j++)
				{
					Square otherSquare = otherSquares[i, j];
					Square square      = new Square(otherSquare);
					this.squares[i, j] = square;
				}
			}
		}
	}

	namespace Graphical
	{
		public class Board : Chess.Game.Board, ChessDrawable
		{
			public new static readonly Graphical.SquareGrid DefaultStartingSquares = new Graphical.Square[,]
			{
				{ new Square('♖', 'a', 1), new Square('♙', 'a', 2), new Square(' ', 'a', 3), new Square(' ', 'a', 4), new Square(' ', 'a', 5), new Square(' ', 'a', 6), new Square('♟', 'a', 7), new Square('♜', 'a', 8) },
				
				{ new Square('♘', 'b', 1), new Square('♙', 'b', 2), new Square(' ', 'b', 3), new Square(' ', 'b', 4), new Square(' ', 'b', 5), new Square(' ', 'b', 6), new Square('♟', 'b', 7), new Square('♞', 'b', 8) },
				
				{ new Square('♗', 'c', 1), new Square('♙', 'c', 2), new Square(' ', 'c', 3), new Square(' ', 'c', 4), new Square(' ', 'c', 5), new Square(' ', 'c', 6), new Square('♟', 'c', 7), new Square('♝', 'c', 8) },
				
				{ new Square('♕', 'd', 1), new Square('♙', 'd', 2), new Square(' ', 'd', 3), new Square(' ', 'd', 4), new Square(' ', 'd', 5), new Square(' ', 'd', 6), new Square('♟', 'd', 7), new Square('♛', 'd', 8) },
				
				{ new Square('♔', 'e', 1), new Square('♙', 'e', 2), new Square(' ', 'e', 3), new Square(' ', 'e', 4), new Square(' ', 'e', 5), new Square(' ', 'e', 6), new Square('♟', 'e', 7), new Square('♚', 'e', 8) },
				
				{ new Square('♗', 'f', 1), new Square('♙', 'f', 2), new Square(' ', 'f', 3), new Square(' ', 'f', 4), new Square(' ', 'f', 5), new Square(' ', 'f', 6), new Square('♟', 'f', 7), new Square('♝', 'f', 8) },
				
				{ new Square('♘', 'g', 1), new Square('♙', 'g', 2), new Square(' ', 'g', 3), new Square(' ', 'g', 4), new Square(' ', 'g', 5), new Square(' ', 'g', 6), new Square('♟', 'g', 7), new Square('♞', 'g', 8) },
				
				{ new Square('♖', 'h', 1), new Square('♙', 'h', 2), new Square(' ', 'h', 3), new Square(' ', 'h', 4), new Square(' ', 'h', 5), new Square(' ', 'h', 6), new Square('♟', 'h', 7), new Square('♜', 'h', 8) }
			};
			
			public new static readonly Graphical.SquareGrid DefaultEmptySquares = new Graphical.Square[,]
			{
				{ new Square(' ', 'a', 1), new Square(' ', 'a', 2), new Square(' ', 'a', 3), new Square(' ', 'a', 4), new Square(' ', 'a', 5), new Square(' ', 'a', 6), new Square(' ', 'a', 7), new Square(' ', 'a', 8) },
				
				{ new Square(' ', 'b', 1), new Square(' ', 'b', 2), new Square(' ', 'b', 3), new Square(' ', 'b', 4), new Square(' ', 'b', 5), new Square(' ', 'b', 6), new Square(' ', 'b', 7), new Square(' ', 'b', 8) },
				
				{ new Square(' ', 'c', 1), new Square(' ', 'c', 2), new Square(' ', 'c', 3), new Square(' ', 'c', 4), new Square(' ', 'c', 5), new Square(' ', 'c', 6), new Square(' ', 'c', 7), new Square(' ', 'c', 8) },
				
				{ new Square(' ', 'd', 1), new Square(' ', 'd', 2), new Square(' ', 'd', 3), new Square(' ', 'd', 4), new Square(' ', 'd', 5), new Square(' ', 'd', 6), new Square(' ', 'd', 7), new Square(' ', 'd', 8) },
				
				{ new Square(' ', 'e', 1), new Square(' ', 'e', 2), new Square(' ', 'e', 3), new Square(' ', 'e', 4), new Square(' ', 'e', 5), new Square(' ', 'e', 6), new Square(' ', 'e', 7), new Square(' ', 'e', 8) },
				
				{ new Square(' ', 'f', 1), new Square(' ', 'f', 2), new Square(' ', 'f', 3), new Square(' ', 'f', 4), new Square(' ', 'f', 5), new Square(' ', 'f', 6), new Square(' ', 'f', 7), new Square(' ', 'f', 8) },
				
				{ new Square(' ', 'g', 1), new Square(' ', 'g', 2), new Square(' ', 'g', 3), new Square(' ', 'g', 4), new Square(' ', 'g', 5), new Square(' ', 'g', 6), new Square(' ', 'g', 7), new Square(' ', 'g', 8) },
				
				{ new Square(' ', 'h', 1), new Square(' ', 'h', 2), new Square(' ', 'h', 3), new Square(' ', 'h', 4), new Square(' ', 'h', 5), new Square(' ', 'h', 6), new Square(' ', 'h', 7), new Square(' ', 'h', 8) }
			};

			protected override Chess.Game.SquareGrid squares
			{
				get { return base.squares; }
				set
				{
					if ((value is Graphical.SquareGrid) == false)
					{
						throw new ArgumentException("A Graphical Board's SquareGrid must be of the Graphical.SquareGrid subtype");
					}
					else
					{
						base.squares = value;
					}
				}
			}
			
			public override Chess.Game.Square[,] Squares
			{
				get { return base.squares; }

				set
				{
					if ((value is Graphical.Square[,]) == false)
					{
						throw new ArgumentException("A Graphical Board's Squares must be of the Graphical.Square subtype");
					}
					else
					{
						base.squares = value;
					}
				}
			}
			
			public Graphical.Square[,] Squares2D
			{
				get { return (Graphical.Square[,]) Squares; }
			}
			
			public Sprite Sprite { get; set; }
		
			public Size Size
			{
				get { return Sprite.Texture.Size; }
			}
		
			public Position Coordinates2D
			{
				get { return Sprite.Position; }
				set { Sprite.Position = value; }
			}

			public Board():
				base(DefaultStartingSquares)
			{
			}

			public Board(Chess.Game.Board other): 
				base(new SquareGrid(other.Squares))
			{
			}

			public Board(SquareGrid squares): 
				base(squares)
			{
			}
			
			public override Chess.Game.Board Clone()
			{
				return new Graphical.Board(this);
			}
			
			public void InitializeGraphicalElements()
			{
				var spriteTexture = new Texture(Config.BoardSpriteFilePath);
				Sprite = new Sprite(spriteTexture);

				foreach (var square in Squares2D)
				{
					square.InitializeGraphicalElements();
				}
			}

			public void Initialize2DCoordinates(Vec2<uint> coordinates)
			{
				this.Coordinates2D = coordinates;

				Vec2<uint> squareOrigin = Coordinates2D;
			
				for (uint i = 0; i < Squares.GetLength(0); i++)
				{
					Square square = null;
					for (uint j = 0; j < Squares.GetLength(1); j++)
					{
						square = (Square) Squares[i, j];

						square.Initialize2DCoordinates(squareOrigin);

						squareOrigin.Y += square.Size.Height;
					}
				
					// ReSharper disable once PossibleNullReferenceException
					squareOrigin.X += square.Size.Width;
					squareOrigin.Y =  Coordinates2D.Y;
				}
			}
		}
		
		[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
		public class SquareGrid : Chess.Game.SquareGrid
		{
			public SquareGrid(Graphical.Square[,] squares) : 
				base(squares: squares)
			{
				
			}

			public SquareGrid(Chess.Game.SquareGrid other):
				base(other: other)
			{
			}
			
			public override Chess.Game.SquareGrid Clone()
			{
				return new Graphical.SquareGrid(this);
			}
			
			public static implicit operator Graphical.Square[,] (Graphical.SquareGrid grid)
			{
				return (Square[,]) grid.squares;
			}
		
			public static implicit operator SquareGrid (Graphical.Square[,] squares)
			{
				return new SquareGrid(squares);
			}
			
			protected override void copyFromSquares(Chess.Game.Square[,] otherSquares)
			{
				for (uint i = 0; i < otherSquares.GetLength(0); i++)
				{
					for (uint j = 0; j < otherSquares.GetLength(1); j++)
					{
						Chess.Game.Square otherSquare = otherSquares[i, j];
						Square square      = new Square(otherSquare);
						this.squares[i, j] = square;
					}
				}
			}
			

		}
	}

	namespace Simulation
	{
		public class Board : Chess.Game.Board
		{
			public override Square[,] Squares
			{
				get { return squares; }
			
				set
				{
					squares = value;
				}
			}
			
			public Board():
				base(DefaultStartingSquares)
			{
			}

			public Board(Chess.Game.Board other): 
				base(other)
			{
			}

			public Board(SquareGrid squares): 
				base(squares)
			{
			}

			public override Game.Board Clone()
			{
				return new Simulation.Board(this);
			}
		}
	}

}