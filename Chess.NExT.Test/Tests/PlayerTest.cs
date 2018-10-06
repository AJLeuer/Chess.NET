using System.Collections.Generic;
using C5;
using Chess.Game;
using Chess.NExT.Test.Util;
using Moq;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
    public static class PlayerTest
    {
        [Test]
        public static void ShouldFindOwnPiecesOnBoard()
        {
                        
            Pawn pawnF2 = (Pawn) Piece.create('♙');
            Pawn pawnG2 = (Pawn) Piece.create('♙');
            Knight knightG1 = (Knight) Piece.create('♘');
            
            var squares = new Square[,]
            {
                { new Square('♜'), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '),    new Square(' ') },
			
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '),    new Square(' ') },
			
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '),    new Square(' ') },
			
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '),    new Square(' ') },
			
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '),    new Square(' ') },
			
                { new Square('♝'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(pawnF2), new Square(' ') },
			
                { new Square(' '), new Square('♟'), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(pawnG2), new Square(knightG1) },
			
                { new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '), new Square(' '),    new Square(' ') }
            };
            
            var mock = new Mock<Board>(MockBehavior.Default);
            var mockBoard = mock.Object;

            mock.Setup(
                    self => self.Squares)
                .Returns(
                    () => squares); 
            
            Player player = new AI(Color.white, mockBoard);
            List<Piece> pieces = player.findOwnPiecesOnBoard(mockBoard);

            //checks that pawnF2, pawnG2, and knightG1 (and nothing else) are in pieces, but in no particular order
            AdditionalCollectionAssertions.AssertContains(actual: pieces, knightG1, pawnF2, pawnG2);
        }
    }
}