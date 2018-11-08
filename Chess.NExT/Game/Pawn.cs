using System;
using System.Collections.Generic;
using Chess.Util;
using static Chess.Game.Color;
using static Chess.Game.Direction;

namespace Chess.Game
{
	public interface IPawn : IPiece
	{
		Direction LegalMovementDirectionToEmptySquares { get; }
		
		List<Direction> LegalCaptureDirections { get; }
	}

	public static class PawnDefaults 
	{
		public static readonly List<Direction> BlackLegalCaptureDirections = new List<Direction> {downLeft, downRight};
		public static readonly List<Direction> WhiteLegalCaptureDirections = new List<Direction> {upLeft, upRight};
		
		public static readonly Dictionary<Color, Char> DefaultSymbols = new Dictionary<Color, Char>
		{
			{black, '♟'}, 
			{white, '♙'}
		};
		
		public static char ASCIISymbol = '​'; //zero-width space
		public const ushort Value = 1;
		public const ushort MaximumMoveDistance = 1;
		
		public static List<Square> findAllPossibleLegalMoveDestinations(this IPawn pawn) 
		{
			List<Square> legalMoveSquares = pawn.findAllPossibleLegalMoveDestinationsForMovesToCapture();
	
			Optional<Square> emptySquareToMove = pawn.findLegalMoveDestinationForMoveToEmpty();
	
			if (emptySquareToMove.HasValue)
			{
				legalMoveSquares.Add(emptySquareToMove.Object);
			}
	
			return legalMoveSquares;
		}
		
		public static List<Square> findAllPossibleLegalMoveDestinationsForMovesToCapture(this IPawn pawn) 
		{
				
			Predicate<Square> squareCheckerForCaptureDirections = (Square squareToCheck) =>
			{
				if (squareToCheck.IsEmpty)
				{
					return false;
				}
				else /* if (squareToCheck.isOccupied) */ 
				{
					return pawn.Color.GetOpposite() == squareToCheck.Piece.Object.Color;
				}
			};
				
			List<Square> captureSquares = pawn.Board.SearchForSquares(squareCheckerForCaptureDirections,
																 pawn.RankAndFile, 1, pawn.LegalCaptureDirections.ToArray());
	
			return captureSquares;
		}
		
		public static Optional<Square> findLegalMoveDestinationForMoveToEmpty(this IPawn pawn) 
		{
							
			Predicate<Square> squareCheckerForMovementDirections = (Square squareToCheck) =>
			{
				return squareToCheck.IsEmpty;
			};
				
			List<Square> availableSquares = pawn.Board.SearchForSquares(squareCheckerForMovementDirections,
																   pawn.RankAndFile, 1, pawn.LegalMovementDirectionToEmptySquares);
	
			if (availableSquares.Count > 0)
			{
				return availableSquares[0];
			}
			else
			{
				return Optional<Square>.Empty;
			}
		}

		public static List<Direction> getLegalCaptureDirections(this IPawn pawn) 
		{
			if (pawn.Color == black)
			{
				return BlackLegalCaptureDirections;
			}
			else /* if (this.color == white) */
			{
				return WhiteLegalCaptureDirections;
			}
		}

		public static Direction getLegalMovementDirectionToEmptySquares(this IPawn pawn)
		{
			{
				if (pawn.Color == black)
				{
					return down;
				}
				else /* if (this.color == white) */
				{
					return up;
				}
			}
		}
	}

	namespace Simulation 
	{
		public class Pawn : Piece, IPawn 
		{
			public override char ASCIISymbol
			{
				get { return PawnDefaults.ASCIISymbol; }
			}
	
			public override ushort Value
			{
				get { return PawnDefaults.Value; }
			}

			public Direction LegalMovementDirectionToEmptySquares
			{
				get { return this.getLegalMovementDirectionToEmptySquares(); } 
			}
	
			public List<Direction> LegalCaptureDirections 
			{
				get { return this.getLegalCaptureDirections(); }
			}
	
			protected Optional<List<Direction>> legalMovementDirections = Optional<List<Direction>>.Empty;
	
			public override List<Direction> LegalMovementDirections 
			{
				get
				{
					if (legalMovementDirections.HasValue == false)
					{
						List<Direction> directions = new List<Direction> {LegalMovementDirectionToEmptySquares};
						directions.AddRange(LegalCaptureDirections);
						legalMovementDirections = directions;
					}
					
					return legalMovementDirections.Object;
				}
			}
			
			public override ushort MaximumMoveDistance { get { return PawnDefaults.MaximumMoveDistance; } }
	
			public Pawn(IPawn other) :
				base(other)
			{
				
			}
	
			public Pawn(Color color) :
				base(PawnDefaults.DefaultSymbols[color], color)
			{
		
			}
		
			public Pawn(char symbol) :
				this((symbol == PawnDefaults.DefaultSymbols[black]) ? black : white)
			{
				if (PawnDefaults.DefaultSymbols.ContainsValue(symbol) == false)
				{
					throw new ArgumentException($"{symbol} is not a valid chess piece");
				}
			}
		
			public override IPiece Clone()
			{
				return new Pawn(this);
			}
	
			/**
			* @return a List that is either filled with the Squares this Pawn can legally move to, or, if there are
			* no such Squares, empty
			*/
			public override List<Chess.Game.Square> FindAllPossibleLegalMoveDestinations()
			{
				return this.findAllPossibleLegalMoveDestinations();
			}
			
			public override void Move(RankFile destination)  
			{
				//todo add move legality checking
				base.Move(destination);
			}
		}
	}

	namespace Graphical 
	{
		public class Pawn : Piece, IPawn 
		{
			public static readonly Dictionary<Color, String> DefaultSpriteImageFiles = new Dictionary<Color, String> 
			{
				{black, "./Assets/Bitmaps/BlackPawn.png"},
				{white, "./Assets/Bitmaps/WhitePawn.png"}
			};
			
			public override char ASCIISymbol
			{
				get { return PawnDefaults.ASCIISymbol; } //zero-width space
			}
	
			public override ushort Value
			{
				get { return PawnDefaults.Value; }
			}
	
			public Direction LegalMovementDirectionToEmptySquares
			{
				get { return this.getLegalMovementDirectionToEmptySquares(); }
			}
	
			public List<Direction> LegalCaptureDirections
			{
				get { return this.getLegalCaptureDirections(); }
			}
	
			protected Optional<List<Direction>> legalMovementDirections = Optional<List<Direction>>.Empty;
	
			public override List<Direction> LegalMovementDirections
			{
				get
				{
					if (legalMovementDirections.HasValue == false)
					{
						List<Direction> directions = new List<Direction> {LegalMovementDirectionToEmptySquares};
						directions.AddRange(LegalCaptureDirections);
						legalMovementDirections = directions;
					}
					
					return legalMovementDirections.Object;
				}
			}
			
			public override ushort MaximumMoveDistance { get { return PawnDefaults.MaximumMoveDistance; } }
	
			public Pawn(IPawn other) :
				base(other)
			{
				SpriteImageFilePath = DefaultSpriteImageFiles[this.Color];
			}
	
			public Pawn(Color color) :
				base(PawnDefaults.DefaultSymbols[color], color, DefaultSpriteImageFiles[color])
			{
		
			}
		
			public Pawn(char symbol) :
				this((symbol == PawnDefaults.DefaultSymbols[black]) ? black : white)
			{
				if (PawnDefaults.DefaultSymbols.ContainsValue(symbol) == false)
				{
					throw new ArgumentException($"{symbol} is not a valid chess piece");
				}
			}
		
			public override IPiece Clone()
			{
				return new Pawn(this);
			}
	
			/**
			* @return a List that is either filled with the Squares this Pawn can legally move to, or, if there are
			* no such Squares, empty
			*/
			public override List<Chess.Game.Square> FindAllPossibleLegalMoveDestinations()
			{
				return this.findAllPossibleLegalMoveDestinations();
			}
	
			public override void Move(RankFile destination)  
			{
				//todo add move legality checking
				base.Move(destination);
			}
		}	
	}
}