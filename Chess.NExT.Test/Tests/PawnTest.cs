using System.Collections.Generic;
using NUnit.Framework;
using Chess.Game;
using Chess.Game.Simulation;
using static Chess.Test.Util.AdditionalCollectionAssertions;

using Board = Chess.Game.Board;
using Square = Chess.Game.Square;

namespace Chess.Test.Tests
{
	public static class PawnTest
	{
		private static Board board;
		private static Pawn pawn;

		[SetUp]
		public static void Setup()
		{
			board = new Game.Simulation.Board(squares: Game.Simulation.Board.DefaultEmptySquares());
			pawn = new Pawn(Color.white);
			board['e', 4].Piece = pawn;
		}
		
		[Test]
		public static void ShouldFindAllValidMoveDestinationsWhenAdjacentSquaresAreUnoccupied()
		{
			List<Square> possibleMoves = pawn.FindAllPossibleLegalMoveDestinations();

			AssertContains(actual: possibleMoves, expectedItems: board['e', 5]);
		}
		
		[Test]
		public static void ShouldFindAllValidMoveDestinationsWhenAdjacentSquaresAreOccupied()
		{
			board['d', 5].Piece = new Rook(Color.black);
			board['e', 5].Piece = new Pawn(Color.black);
			board['g', 5].Piece = new Knight(Color.black);
				
			List<Square> possibleMoves = pawn.FindAllPossibleLegalMoveDestinations();

			AssertContains(actual: possibleMoves, expectedItems: board['d', 5]);
		}
	}
}