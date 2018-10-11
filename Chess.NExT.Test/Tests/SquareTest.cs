using Chess.Game;
using Chess.Util;
using FluentAssertions;
using NUnit.Framework;

using Position = Chess.Util.Vec2<uint>;
using File = System.Char;
using Rank = System.UInt16;

namespace Chess.NExT.Test.Tests
{
	public static class SquareTest
	{
		[TestCase('c', (Rank)8)]
		[TestCase('d', (Rank)5)]
		[TestCase('h', (Rank)7)]
		public static void WhiteSquaresShouldDetermineOwnColorBasedOnPositionOnBoard(File file, Rank rank)
		{
			var square = new Square();
			square.BoardPosition = new Position(file, rank);			

			square.Color.Should().Be(Color.white);
		}
		
		[TestCase('a', (Rank)1)]
		[TestCase('b', (Rank)2)]
		[TestCase('g', (Rank)7)]
		public static void BlackSquaresShouldDetermineOwnColorBasedOnPositionOnBoard(File file, Rank rank)
		{
			var square = new Square();
			square.BoardPosition = new Position(file, rank);		

			square.Color.Should().Be(Color.black);
		}
	}
}