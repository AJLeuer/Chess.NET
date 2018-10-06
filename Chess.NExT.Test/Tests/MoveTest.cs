using Chess.Game;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
	public static class MoveTest
	{
		private static BasicGame game;
		private static Board board;
		private static Player whitePlayer;
		private static Knight whiteKnight;

		[SetUp]
		public static void Setup()
		{
			board = new Board
			{
				Squares = new Square[,]
				{
					{ new Square('♜'), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
					{ new Square(' '), new Square(' '), new Square('♘'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
					{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
					{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') },
					{ new Square('♚'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♔') },
					{ new Square('♝'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♙'), new Square(' ') },
					{ new Square(' '), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square('♘') },
					{ new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' ') }
				}
			};
			
			var whitePlayerMock = new Mock<Player>(Color.white, null)
			{
				CallBase = true
			};
			
			var whitePlayerCloneMock = new Mock<Player>(Color.white, null)
			{
				CallBase = true
			};
			
			var gameMock = new Mock<BasicGame>(MockBehavior.Loose, board, whitePlayerMock.Object, null)
			{
				CallBase = true
			};
			
			/* Need to provide an implementation of Clone(), since it's an abstract method */
			whitePlayerMock.Setup((Player self) => self.Clone())
						   .Returns(() => { return whitePlayerCloneMock.Object; });
			
			whitePlayerMock.Setup((Player self) => self.Color)
						   .CallBase();
			
			game = gameMock.Object;
			whitePlayer = whitePlayerMock.Object;
		}

		[Test]
		public static void ShouldCalculateValueToPlayer()
		{
			whiteKnight = (Knight) board['b', 3].Piece.Object;
			
			var move = new Move(whitePlayer, whiteKnight, board['a', 1]);

			move.Value.Should().Be(5);
		}
	}
}