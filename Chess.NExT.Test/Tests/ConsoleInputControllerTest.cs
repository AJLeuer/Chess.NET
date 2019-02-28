using System;
using Chess.Game;
using Chess.Game.Simulation;
using Chess.Input;
using Moq;
using NUnit.Framework;
using Chess.Utility;
using Square = Chess.Game.Simulation.Square;

namespace Chess.Test.Tests
{
	public static class ConsoleInputControllerTest
	{
		private static ConsoleInputController ConsoleInputController;
		static BasicGame Game;
		static Mock<TextIOInterface> TextIOMock = new Mock<TextIOInterface>();
		private static SimpleAI Player = new SimpleAI(Color.white);

		[SetUp]
		public static void Setup()
		{
			ConsoleInputController = new ConsoleInputController(Player);
			ConsoleInputController.IOInterface = TextIOMock.Object;
			
			var board = new Game.Simulation.Board
			{
				Squares = new Square[,]
				{
					{ new Square(' ', 'a', 1), new Square(' ', 'a', 2), new Square(' ', 'a', 3), new Square(' ', 'a', 4), new Square(' ', 'a', 5), new Square(' ', 'a', 6), new Square(' ', 'a', 7), new Square(' ', 'a', 8) },
					
					{ new Square(' ', 'b', 1), new Square(' ', 'b', 2), new Square(' ', 'b', 3), new Square(' ', 'b', 4), new Square(' ', 'b', 5), new Square(' ', 'b', 6), new Square(' ', 'b', 7), new Square(' ', 'b', 8) },
					
					{ new Square(' ', 'c', 1), new Square(' ', 'c', 2), new Square(' ', 'c', 3), new Square(' ', 'c', 4), new Square(' ', 'c', 5), new Square(' ', 'c', 6), new Square(' ', 'c', 7), new Square(' ', 'c', 8) },
					
					{ new Square(' ', 'd', 1), new Square(' ', 'd', 2), new Square(' ', 'd', 3), new Square(' ', 'd', 4), new Square(' ', 'd', 5), new Square(' ', 'd', 6), new Square(' ', 'd', 7), new Square(' ', 'd', 8) },
					
					{ new Square(' ', 'e', 1), new Square(' ', 'e', 2), new Square(' ', 'e', 3), new Square(' ', 'e', 4), new Square(' ', 'e', 5), new Square(' ', 'e', 6), new Square(' ', 'e', 7), new Square(' ', 'e', 8) },
					
					{ new Square(' ', 'f', 1), new Square(' ', 'f', 2), new Square(' ', 'f', 3), new Square(' ', 'f', 4), new Square(' ', 'f', 5), new Square('♘', 'f', 6), new Square(' ', 'f', 7), new Square(' ', 'f', 8) },
					
					{ new Square(' ', 'g', 1), new Square(' ', 'g', 2), new Square(' ', 'g', 3), new Square(' ', 'g', 4), new Square(' ', 'g', 5), new Square(' ', 'g', 6), new Square(' ', 'g', 7), new Square(' ', 'g', 8) },
					
					{ new Square(' ', 'h', 1), new Square(' ', 'h', 2), new Square(' ', 'h', 3), new Square(' ', 'h', 4), new Square(' ', 'h', 5), new Square(' ', 'h', 6), new Square(' ', 'h', 7), new Square(' ', 'h', 8) }
				}	
			};

			Game = new Game.Simulation.Game(board, new SimpleAI(Color.black), Player);
		}
		
		[Test]
		public static void ShouldCreateMoveWithCorrectDestinationFromTextInput()
		{
			String playerInput = "F6 E4";
			TextIOMock.Setup((TextIOInterface self) => self.ReadLine())
					  .Returns(playerInput);

			Move nextMove = ConsoleInputController.NextMove;

			RankFile expectedMoveDestination = ('E', 4);
			Assert.AreEqual(expectedMoveDestination, nextMove.Destination.RankAndFile);
		}
		
		[Test]
		public static void ShouldCreateMoveWithCorrectPieceFromTextInput()
		{
			String playerInput = "F6 E4";
			TextIOMock.Setup((TextIOInterface self) => self.ReadLine())
					  .Returns(playerInput);

			Move nextMove = ConsoleInputController.NextMove;

			IPiece movingPiece = (Knight) Game.Board[('F', 6)].Piece.Object;
			Assert.AreEqual(expected: movingPiece, actual: nextMove.Piece);
		}
	}
}