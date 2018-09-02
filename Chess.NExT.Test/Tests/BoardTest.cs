using System.Management.Instrumentation;
using C5;
using Chess.Game;
using Chess.Util;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
    public class BoardTest
    {
        
        [Test]
        public void ShouldCorrectlyIdentifyPositionsInsideBoard()
        {
            var board = new Board(squares: Board.DefaultStartingSquares);
            var onBoard1 = new Vec2<int>(1, 7);
            var onBoard2 = new Vec2<int>(7, 0);
            

            board.IsInsideBounds(onBoard1).Should().Be(true);
            board.IsInsideBounds(onBoard2).Should().Be(true);
        }
        
        [Test]
        public void ShouldCorrectlyIdentifyPositionsOutsideBoard()
        {
            var board = new Board(squares: Board.DefaultStartingSquares);
            var offBoard1 = new Vec2<int>(-1, 7);
            var offBoard2 = new Vec2<int>(0, 8);

            board.IsInsideBounds(offBoard1).Should().Be(false);
            board.IsInsideBounds(offBoard2).Should().Be(false);
        }

        [Test]
        public void ShouldCorrectlyCalculateTotalValueOfPiecesOfGivenColorOnBoard()
        {
            var squares = new ArrayList<ArrayList<Square>>
            {
                new ArrayList<Square> { new Square('♜', 'a', 8), new Square('♟', 'a', 7)},
                new ArrayList<Square> { new Square('♚', 'e', 8), new Square('♔', 'e', 1)},
                new ArrayList<Square> { new Square('♝', 'f', 8), new Square(' ', 'f', 6), new Square(' ', 'f', 4), new Square('♙', 'f', 2)},
                new ArrayList<Square> { new Square('♟', 'g', 7), new Square('♙', 'g', 2), new Square('♘', 'g', 1)}
            };
            
            var board = new Board(squares);
            var blackPlayerMock = new Mock<Player>(Color.black, null);
            var whitePlayerMock = new Mock<Player>(Color.white, null);

            blackPlayerMock.Setup(
                    (Player self) => self.color)
                .Returns(
                    () => Color.black);
            
            whitePlayerMock.Setup(
                    (Player self) => self.color)
                .Returns(
                    () => Color.white);


            Player blackPlayer = blackPlayerMock.Object;
            Player whitePlayer = whitePlayerMock.Object;
            
            board.CalculateRelativeValue(blackPlayer).Should().Be(5);
            board.CalculateRelativeValue(whitePlayer).Should().Be(-5);
        }
    }
}