using Chess.Game;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
    public static class MoveTest
    {
        [Test]
        public static void ShouldCalculateValueToPlayer()
        {
            
            var board = new Board() {
                Squares = new Square[,]
                {
                    { new Square('♜', 'a', 8), new Square('♟', 'a', 7), new Square(' ', 'a', 6), new Square(' ', 'a', 5), new Square(' ', 'a', 4), new Square(' ', 'a', 3), new Square(' ', 'a', 2), new Square(' ', 'a', 1) },
                    
                    { new Square(' ', 'b', 8), new Square(' ', 'b', 7), new Square('♘', 'b', 6), new Square(' ', 'b', 5), new Square(' ', 'b', 4), new Square(' ', 'b', 3), new Square(' ', 'b', 2), new Square(' ', 'b', 1) },
                    
                    { new Square(' ', 'c', 8), new Square(' ', 'c', 7), new Square(' ', 'c', 6), new Square(' ', 'c', 5), new Square(' ', 'c', 4), new Square(' ', 'c', 3), new Square(' ', 'c', 2), new Square(' ', 'c', 1) },
                    
                    { new Square(' ', 'd', 8), new Square(' ', 'd', 7), new Square(' ', 'd', 6), new Square(' ', 'd', 5), new Square(' ', 'd', 4), new Square(' ', 'd', 3), new Square(' ', 'd', 2), new Square(' ', 'd', 1) },
                    
                    { new Square('♚', 'e', 8), new Square(' ', 'e', 7), new Square(' ', 'e', 6), new Square(' ', 'e', 5), new Square(' ', 'e', 4), new Square(' ', 'e', 3), new Square(' ', 'e', 2), new Square('♔', 'e', 1) },
                    
                    { new Square('♝', 'f', 8), new Square(' ', 'f', 7), new Square(' ', 'f', 6), new Square(' ', 'f', 5), new Square(' ', 'f', 4), new Square(' ', 'f', 3), new Square('♙', 'f', 2), new Square(' ', 'f', 1) },
                    
                    { new Square(' ', 'g', 8), new Square('♟', 'g', 7), new Square(' ', 'g', 6), new Square(' ', 'g', 5), new Square(' ', 'g', 4), new Square(' ', 'g', 3), new Square(' ', 'g', 2), new Square('♘', 'g', 1) },
                    
                    { new Square(' ', 'h', 8), new Square(' ', 'h', 7), new Square(' ', 'h', 6), new Square(' ', 'h', 5), new Square(' ', 'h', 4), new Square(' ', 'h', 3), new Square(' ', 'h', 2), new Square(' ', 'h', 1) }
                }
            };
            
            Knight whiteKnight = (Knight) board['b', 6].Piece.Object;
            var whitePlayerMock = new Mock<Player>(Color.white, null)
            {
                CallBase = true
            };
            whitePlayerMock.Setup(
                (Player self) => self.Clone())
            .Returns(
                () => new Mock<Player>(whitePlayerMock.Object).Object);
            var gameMock = new Mock<BasicGame>(MockBehavior.Loose, board, whitePlayerMock.Object, null)
            {
                CallBase = true
            };
            var mockGame = gameMock.Object;
            
            var move = new Move(mockGame.WhitePlayer, whiteKnight, board['a', 8]);

            move.Value.Should().Be(5);
        }
    }
}