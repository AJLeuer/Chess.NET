using System.Collections.Generic;
using Chess.Game;
using FluentAssertions;
using FluentAssertions.Collections;
using NUnit.Framework;

using static Chess.NExT.Test.Util.AdditionalCollectionAssertions;

namespace Chess.NExT.Test.Tests
{
	public static class PieceTest
	{
		public static Square[,] squares;
		
		[SetUp]
		public static void Setup()
		{
			squares = new Square[,]
			{
				{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
				
				{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
				
				{ new Square(' '), new Square(' '), new Square('♟'), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
				
				{ new Square(' '), new Square(' '), new Square('♕'), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
				
				{ new Square(' '), new Square(' '), new Square('♟'), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
				
				{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
				
				{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
				
				{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') }
			};
		}

		[Test]
		public static void ShouldFindAllValidMoveDestinations()
		{
			var board = new Board(squares);
			Queen whiteQueen = (Queen) board['d', 3].Piece.Object;

			var moveDestinations = whiteQueen.FindAllPossibleLegalMoveDestinations();

			
			AssertContains(actual: moveDestinations,
									   
						   board['d', 2],
						   board['d', 1],
						   board['d', 4],
						   board['c', 3],
						   board['e', 3],
						   board['c', 2],
						   board['b', 1],
						   board['e', 2],
						   board['f', 1],
						   board['c', 4],
						   board['e', 4]);
		}
	}
}