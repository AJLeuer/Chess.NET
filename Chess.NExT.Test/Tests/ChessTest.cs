using Chess.Game;
using Chess.Util;
using FluentAssertions;
using NUnit.Framework;

namespace Chess.NExT.Test.Tests
{
    public static class ChessTest
    {
        [Test]
        public static void ShouldConvertCoordinatePositionToRankAndFile()
        {
            Vec2<uint> position = new Vec2<uint>(3, 3);
            
            RankFile boardPosition = position; //invokes conversion operator

            boardPosition.Should().BeEquivalentTo(new RankFile('d', 4));
        }
        
        [Test]
        public static void ShouldConvertRankAndFileToPosition()
        {
            RankFile g7 = new RankFile('g', 7);
            
            Vec2<uint> position = g7; //invokes conversion operator

            position.Should().BeEquivalentTo(new Vec2<uint>(6, 6));
        }

        [Test]
        public static void RankAndFileConversionShouldBeReversible()
        {
            RankFile a8 = new RankFile('a', 8);
            
            Vec2<uint> position = a8; //invokes conversion operator
            RankFile reverseConvertedPosition = position; //invokes conversion operator
            
            reverseConvertedPosition.Should().BeEquivalentTo(a8);
        }
    }
}