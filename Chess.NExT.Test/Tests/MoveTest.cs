using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Moq;
using NUnit.Framework;

using Chess.Game;
using Board = Chess.Game.Board;
using Square = Chess.Game.Simulation.Square;
using Knight = Chess.Game.Simulation.Knight;
using Rook = Chess.Game.Simulation.Rook;
using Piece = Chess.Game.Piece;

namespace Chess.NExT.Test.Tests
{
	public static class MoveTest
	{
		[SuppressMessage("ReSharper", "NotAccessedField.Local")]
		private static BasicGame game;
		private static Board board;
		private static Player whitePlayer;
		private static Player blackPlayer;
		private static Knight whiteKnight;

		[SetUp]
		public static void Setup()
		{
			board = new Game.Simulation.Board
			{
				Squares = new Square[,]
				{
					{ new Square('♜', 'a', 1), new Square('♟', 'a', 2), new Square(' ', 'a', 3), new Square(' ', 'a', 4), new Square(' ', 'a', 5), new Square(' ', 'a', 6), new Square(' ', 'a', 7), new Square(' ', 'a', 8) },
					
					{ new Square(' ', 'b', 1), new Square(' ', 'b', 2), new Square('♘', 'b', 3), new Square(' ', 'b', 4), new Square(' ', 'b', 5), new Square(' ', 'b', 6), new Square(' ', 'b', 7), new Square(' ', 'b', 8) },
					
					{ new Square(' ', 'c', 1), new Square(' ', 'c', 2), new Square(' ', 'c', 3), new Square(' ', 'c', 4), new Square(' ', 'c', 5), new Square(' ', 'c', 6), new Square(' ', 'c', 7), new Square(' ', 'c', 8) },
					
					{ new Square(' ', 'd', 1), new Square(' ', 'd', 2), new Square(' ', 'd', 3), new Square(' ', 'd', 4), new Square(' ', 'd', 5), new Square(' ', 'd', 6), new Square(' ', 'd', 7), new Square(' ', 'd', 8) },
					
					{ new Square('♚', 'e', 1), new Square(' ', 'e', 2), new Square(' ', 'e', 3), new Square(' ', 'e', 4), new Square(' ', 'e', 5), new Square(' ', 'e', 6), new Square(' ', 'e', 7), new Square('♔', 'e', 8) },
					
					{ new Square('♝', 'f', 1), new Square(' ', 'f', 2), new Square(' ', 'f', 3), new Square(' ', 'f', 4), new Square(' ', 'f', 5), new Square(' ', 'f', 6), new Square('♙', 'f', 7), new Square(' ', 'f', 8) },
					
					{ new Square(' ', 'g', 1), new Square('♟', 'g', 2), new Square(' ', 'g', 3), new Square(' ', 'g', 4), new Square(' ', 'g', 5), new Square(' ', 'g', 6), new Square(' ', 'g', 7), new Square('♘', 'g', 8) },
					
					{ new Square(' ', 'h', 1), new Square(' ', 'h', 2), new Square(' ', 'h', 3), new Square(' ', 'h', 4), new Square(' ', 'h', 5), new Square(' ', 'h', 6), new Square(' ', 'h', 7), new Square(' ', 'h', 8) }
				}	
			};
			
			var whitePlayerMock = new Mock<Player>(Color.white) { CallBase = true };

			var blackPlayerMock = new Mock<Player>(Color.black) { CallBase = true };
			
			var whitePlayerCloneMock = new Mock<Player>(Color.white) { CallBase = true };
			
			var gameMock = new Mock<BasicGame>(MockBehavior.Loose, board, whitePlayerMock.Object, blackPlayerMock.Object) { CallBase = true };

			gameMock.Setup((BasicGame self) => self.Board)
							.Returns(board);
			
			/* Need to provide an implementation of Clone(), since it's an abstract method */
			whitePlayerMock.Setup((Player self) => self.Clone())
						   .Returns(() => { return whitePlayerCloneMock.Object; });
			
			whitePlayerMock.Setup((Player self) => self.Color)
						   .CallBase();
			
			game = gameMock.Object;
			whitePlayer = whitePlayerMock.Object;
			blackPlayer = blackPlayerMock.Object;
		}

		[Test]
		public static void ShouldCalculateValueToPlayer()
		{
			whiteKnight = (Knight) board['b', 3].Piece.Object;
			
			var move = new Move(whitePlayer, whiteKnight, board['a', 1]);

			move.Value.Should().Be(5);
		}

		[Test]
		public static void ShouldCreateMatchingMove()
		{
			Player anotherBlackPlayer = new SimpleAI(color: Color.black);
			Piece anotherRook = new Rook(color: Color.black);
			var startingSquare = new Square(file: 'a', rank: 1);
			startingSquare.Piece = anotherRook;
			var destination = new Square(file: 'c', rank: 1);
			
			var anotherMove = new Move(anotherBlackPlayer, anotherRook, destination);

			Move move = Move.CreateMatchingMoveForGame(anotherMove, game);

			move.Player.Should().Be(blackPlayer);
			move.Piece.Should().Be(board['a', 1].Piece.Object);
			move.Destination.Should().Be(board['c', 1]);
			move.Board.Should().Equal(board);
		}
	}
}