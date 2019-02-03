using System.Collections.Generic;
using Chess.Game;
using Chess.Test.Util;
using Moq;
using NUnit.Framework;

using AI = Chess.Game.Real.AI;
using Square = Chess.Game.Simulation.Square;
using Piece = Chess.Game.Simulation.Piece;
using Knight = Chess.Game.Simulation.Knight;
using Pawn = Chess.Game.Simulation.Pawn;

namespace Chess.Test.Tests
{
    public static class PlayerTest
    {
        [Test]
        public static void ShouldFindOwnPiecesOnBoard()
        {
                        
            Pawn pawnF2 = (Pawn) Piece.Create('♙');
            Pawn pawnG2 = (Pawn) Piece.Create('♙');
            Knight knightG1 = (Knight) Piece.Create('♘');
            
            var squares = new Square[,]
            {
                { new Square('♜', 'a', 1), new Square('♟', 'a', 2), new Square(' ', 'a', 3), new Square(' ', 'a', 4), new Square(' ', 'a', 5), new Square(' ', 'a', 6), new Square(' ', 'a', 7),    new Square(' ', 'a', 8) },
			
                { new Square(' ', 'b', 1), new Square(' ', 'b', 2), new Square(' ', 'b', 3), new Square(' ', 'b', 4), new Square(' ', 'b', 5), new Square(' ', 'b', 6), new Square(' ', 'b', 7),    new Square(' ', 'b', 8) },
			
                { new Square(' ', 'c', 1), new Square(' ', 'c', 2), new Square(' ', 'c', 3), new Square(' ', 'c', 4), new Square(' ', 'c', 5), new Square(' ', 'c', 6), new Square(' ', 'c', 7),    new Square(' ', 'c', 8) },
			
                { new Square(' ', 'd', 1), new Square(' ', 'd', 2), new Square(' ', 'd', 3), new Square(' ', 'd', 4), new Square(' ', 'd', 5), new Square(' ', 'd', 6), new Square(' ', 'd', 7),    new Square(' ', 'd', 8) },
			
                { new Square(' ', 'e', 1), new Square(' ', 'e', 2), new Square(' ', 'e', 3), new Square(' ', 'e', 4), new Square(' ', 'e', 5), new Square(' ', 'e', 6), new Square(' ', 'e', 7),    new Square(' ', 'e', 8) },
			
                { new Square('♝', 'f', 1), new Square(' ', 'f', 2), new Square(' ', 'f', 3), new Square(' ', 'f', 4), new Square(' ', 'f', 5), new Square(' ', 'f', 6), new Square(pawnF2, 'f', 7), new Square(' ', 'f', 8) },
			
                { new Square(' ', 'g', 1), new Square('♟', 'g', 2), new Square(' ', 'g', 3), new Square(' ', 'g', 4), new Square(' ', 'g', 5), new Square(' ', 'g', 6), new Square(pawnG2, 'g', 7), new Square(knightG1, 'g', 8) },
			
                { new Square(' ', 'h', 1), new Square(' ', 'h', 2), new Square(' ', 'h', 3), new Square(' ', 'h', 4), new Square(' ', 'h', 5), new Square(' ', 'h', 6), new Square(' ', 'h', 7),    new Square(' ', 'h', 8) }
            };
            
            var mock = new Mock<Board>(MockBehavior.Default);
            var mockBoard = mock.Object;

            mock.Setup(
                    self => self.Squares)
                .Returns(
                    () => squares); 
            
            Player player = new AI(Color.white);
			player.Board = mockBoard;
            List<IPiece> pieces = player.findOwnPiecesOnBoard(mockBoard);

            //checks that pawnF2, pawnG2, and knightG1 (and nothing else) are in pieces, but in no particular order
            AdditionalCollectionAssertions.AssertContains(actual: pieces, knightG1, pawnF2, pawnG2);
        }
    }
}