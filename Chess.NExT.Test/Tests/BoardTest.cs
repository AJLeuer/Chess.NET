using Chess.Game;
using Chess.Util;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
    public static class BoardTest
    {
        
        [Test]
        public static void ShouldIdentifyPositionsInsideBoard()
        {
            var board = new Board(squares: Board.DefaultStartingSquares);
            var onBoard1 = new Vec2<int>(1, 7);
            var onBoard2 = new Vec2<int>(7, 0);
            

            board.IsInsideBounds(onBoard1).Should().Be(true);
            board.IsInsideBounds(onBoard2).Should().Be(true);
        }
        
        [Test]
        public static void ShouldIdentifyPositionsOutsideBoard()
        {
            var board = new Board(squares: Board.DefaultStartingSquares);
            var offBoard1 = new Vec2<int>(-1, 7);
            var offBoard2 = new Vec2<int>(0, 8);

            board.IsInsideBounds(offBoard1).Should().Be(false);
            board.IsInsideBounds(offBoard2).Should().Be(false);
        }

        [Test]
        public static void ShouldCalculateTotalValueOfPiecesOfGivenColorOnBoard()
        {
            var squares = new Square[,]
            {
                { new Square('♜', 'a', 1), new Square('♟', 'a', 2), new Square(' ', 'a', 3), new Square(' ', 'a', 4), new Square(' ', 'a', 5), new Square(' ', 'a', 6), new Square(' ', 'a', 7), new Square(' ', 'a', 8) },
                
                { new Square(' ', 'b', 1), new Square(' ', 'b', 2), new Square(' ', 'b', 3), new Square(' ', 'b', 4), new Square(' ', 'b', 5), new Square(' ', 'b', 6), new Square(' ', 'b', 7), new Square(' ', 'b', 8) },
                
                { new Square(' ', 'c', 1), new Square(' ', 'c', 2), new Square(' ', 'c', 3), new Square(' ', 'c', 4), new Square(' ', 'c', 5), new Square(' ', 'c', 6), new Square(' ', 'c', 7), new Square(' ', 'c', 8) },
                
                { new Square('♚', 'e', 1), new Square(' ', 'd', 2), new Square(' ', 'd', 3), new Square(' ', 'd', 4), new Square(' ', 'd', 5), new Square(' ', 'd', 6), new Square(' ', 'd', 7), new Square(' ', 'd', 8) },
                
                { new Square(' ', 'e', 1), new Square(' ', 'e', 2), new Square(' ', 'e', 3), new Square(' ', 'e', 4), new Square(' ', 'e', 5), new Square(' ', 'e', 6), new Square(' ', 'e', 7), new Square('♔', 'e', 8) },
                
                { new Square('♝', 'f', 1), new Square(' ', 'f', 2), new Square(' ', 'f', 3), new Square(' ', 'f', 4), new Square(' ', 'f', 5), new Square(' ', 'f', 6), new Square('♙', 'f', 7), new Square(' ', 'f', 8) },
                
                { new Square(' ', 'g', 1), new Square('♟', 'g', 2), new Square(' ', 'g', 3), new Square(' ', 'g', 4), new Square(' ', 'g', 5), new Square(' ', 'g', 6), new Square('♙', 'g', 7), new Square('♘', 'g', 8) },
                
                { new Square(' ', 'h', 1), new Square(' ', 'h', 2), new Square(' ', 'h', 3), new Square(' ', 'h', 4), new Square(' ', 'h', 5), new Square(' ', 'h', 6), new Square(' ', 'h', 7), new Square(' ', 'h', 8) }
            };
            
            var board = new Board(squares);
            var blackPlayerMock = new Mock<Player>(Color.black, null);
            var whitePlayerMock = new Mock<Player>(Color.white, null);

            blackPlayerMock.Setup(
                    (Player self) => self.Color)
                .Returns(
                    () => Color.black);
            
            whitePlayerMock.Setup(
                    (Player self) => self.Color)
                .Returns(
                    () => Color.white);


            Player blackPlayer = blackPlayerMock.Object;
            Player whitePlayer = whitePlayerMock.Object;
            
            board.CalculateRelativeValue(blackPlayer).Should().Be(5);
            board.CalculateRelativeValue(whitePlayer).Should().Be(-5);
        }

        [Test]
        public static void ShouldIdentifyMatchingPieceOnAnotherBoard()
        {
            Board board = new Board();

            Board boardCopy = new Board(board);

            Queen queen = (Queen) board['d', 8].Piece.Object;
            Queen duplicateQueen = (Queen) boardCopy['d', 8].Piece.Object;

            board.FindMatchingPiece(duplicateQueen).Should().Be(queen);
        }
    }
}