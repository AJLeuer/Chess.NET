using Chess.Game;
using FluentAssertions;
using NUnit.Framework;

using Position = Chess.Util.Vec2<uint>;

namespace Chess.NExT.Test.Tests
{
    public static class ChessTest
    {
        [Test]
        public static void ShouldConvertCoordinatePositionToRankAndFile()
        {
            var position = new Position(3, 3);
            
            RankFile boardPosition = position; //invokes conversion operator

            boardPosition.Should().BeEquivalentTo(new RankFile(file: 'd',rank: 4));
        }
        
        [Test]
        public static void ShouldConvertRankAndFileToPosition()
        {
            var g7 = new RankFile(file: 'g', rank: 7);
            
            Position position = g7; //invokes conversion operator

            position.Should().BeEquivalentTo(new Position(6, 6));
        }

        [Test]
        public static void RankAndFileConversionShouldBeReversible()
        {
            var a8 = new RankFile(file: 'a', rank: 8);
            
            Position position = a8; //invokes conversion operator
            RankFile reverseConvertedPosition = position; //invokes conversion operator
            
            reverseConvertedPosition.Should().BeEquivalentTo(a8);
        }
    }
}