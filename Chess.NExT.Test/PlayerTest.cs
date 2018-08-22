using System.Collections.Generic;
using NUnit.Framework;
using Moq;

using Chess.Game;
using Chess.Util;
using FluentAssertions;

namespace Chess.NExT.Test
{
    public static class PlayerTest
    {
        [Test]
        public static void ShouldFindOwnPiecesOnBoard()
        {
                        
            Pawn pawnF2 = (Pawn) Piece.create('♙');
            Pawn pawnG2 = (Pawn) Piece.create('♙');
            Knight knightG1 = (Knight) Piece.create('♘');
            
            var squares = new List<List<Square>>
            {
                new List<Square> { new Square('♜', 'a', 8), new Square('♟', 'a', 7)},
                new List<Square> { new Square('♝', 'f', 8), new Square(' ', 'f', 6), new Square(' ', 'f', 4), new Square(pawnF2, 'f', 2)},
                new List<Square> { new Square('♟', 'g', 7), new Square(pawnG2, 'g', 2), new Square(knightG1, 'g', 1)}
            };
            
            var mock = new Mock<Board>(MockBehavior.Default, args: squares);
            var mockBoard = mock.Object;

            mock.Setup(self =>
                    self.GetEnumerator())
                .Returns(() =>
                    mockBoard.Squares.GetEnumerator()); 
            
            Player player = new AI(Color.white, mockBoard);
            List<Piece> pieces = player.findOwnPiecesOnBoard(mockBoard);

            //checks that pawnF2, pawnG2, and knightG1 (and nothing else) are in pieces, but in no particular order
            CollectionAssert.AreEquivalent(new Piece[] {knightG1, pawnF2, pawnG2}, pieces);
            pieces.Should().BeEquivalentTo(pawnG2, pawnF2, knightG1);
        }
    }
}