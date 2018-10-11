using Chess.Game;
using Chess.Util;
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
            Position position = new Position(3, 3);
            
            RankFile boardPosition = position; //invokes conversion operator

            boardPosition.Should().BeEquivalentTo(new RankFile('d', 4));
        }
        
        [Test]
        public static void ShouldConvertRankAndFileToPosition()
        {
            RankFile g7 = new RankFile('g', 7);
            
            Position position = g7; //invokes conversion operator

            position.Should().BeEquivalentTo(new Position(6, 6));
        }

        [Test]
        public static void RankAndFileConversionShouldBeReversible()
        {
            RankFile a8 = new RankFile('a', 8);
            
            Position position = a8; //invokes conversion operator
            RankFile reverseConvertedPosition = position; //invokes conversion operator
            
            reverseConvertedPosition.Should().BeEquivalentTo(a8);
        }
    }
}