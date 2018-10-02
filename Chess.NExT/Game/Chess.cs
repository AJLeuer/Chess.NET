using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Chess.Util;
using static Chess.Util.Util;

using File = System.Char;
using Rank = System.UInt16;
	
namespace Chess.Game
{
	// public enum Color
	// {
	// 	black,
	// 	white
	// }
	public class Color
	{
		public static readonly Color white = new White();
		public static readonly Color black = new Black();
		
		public class White : Color {}
		public class Black : Color {}
	}
    
	public static class ColorMethods {

		public static Color getOpposite(this Color color)
		{
			switch (color)
			{
				case Color.White white:
					return Color.black;
				case Color.Black black:
					return Color.white;
				default:
					throw new InvalidEnumArgumentException();
			}
		}
	}
	
	public struct Direction 
	{
		public enum LateralValue : short
		{
			right = 1,
			left = -1
		}

		public enum LongitudinalValue : short
		{
			down = 1,
			up = -1
		}

		public static readonly Direction up = new Direction(0, -1);

		public static readonly Direction down = new Direction(0, 1);
		
		public static readonly Direction left = new Direction(-1, 0);
	
		public static readonly Direction right = new Direction(1, 0);
	
		public static readonly Direction upLeft = new Direction(-1, -1);
	
		public static readonly Direction upRight = new Direction(1, -1);
	
		public static readonly Direction downLeft = new Direction(-1, 1);
	
		public static readonly Direction downRight = new Direction(1, 1);
	
		
		public readonly Vec2<short> value;
		
		public Direction(LateralValue lateral, LongitudinalValue longitudinal) :
			this((short)lateral, (short)longitudinal)
		{

		}
		
		public Direction(short lateral, short longitudinal)
		{
			value = new Vec2<short>(lateral, longitudinal);
		}
		
		public static implicit operator Vec2<short>(Direction direction) { return direction.value; }
	}

    public struct RankFile : IEquatable<RankFile> 
    {
	    
	    // ReSharper disable once UnusedMember.Local
	    private const File firstFile = 'a';

	    // ReSharper disable once UnusedMember.Local
	    private const File lastFile = 'h';

	    // ReSharper disable once UnusedMember.Local
	    private const Rank firstRank = 1;

		private const Rank lastRank = 8;

		public static File convertToFile(uint boardPositionX) 
		{
			var firstFileCharCode = (Int32)firstFile;
			char c = (char)(boardPositionX + firstFileCharCode);
			return c;
		}
 
		public static Rank convertToRank(uint boardPositionY) 
		{
			var rank = firstRank + boardPositionY;
			return (Rank)rank;
		}

	    private static uint convertToBoardPositionIntegerX(File file)
	    {
		    var firstFileCharCode = (Int32)firstFile;
		    int intValue = file - firstFileCharCode;
		    return (uint)intValue;
	    }
	    
	    private static uint convertToBoardPositionIntegerY(Rank rank)
	    {
		    return (uint)(rank - firstRank);
	    }
	    
	    public static Vec2<uint> ConvertToBoardPosition (RankFile rankAndFile)  
	    {

		    /* Chess ranks start with 1 at the bottom and increase as we move up the board,
		     but window coordinates start with y = 0 at the top, and y increases with descent.
		     So to convert our rank to a y-coordinate, we'll need to essentially swap each rank
		     value across the horizontal axis, and subtract 1 from it (rank values start at 1, but
		     vec2<int> values start at 0)

		     Chess files can be converted from x-coordinates by subtracting the Unicode decimal value for 'a' -
		     97 - from the decimal value of the char representing the rank. Applied to 'a', this gives an x-coord
		     of 0, 'b' outputs as 1, 'c' is 2, etc.
		     */

		    uint y = convertToBoardPositionIntegerY(rankAndFile.rank);

		    //convert file to x:
		    uint x = convertToBoardPositionIntegerX(rankAndFile.file);

		    return new Vec2<uint>(x, y);
	    }

		public File file { get; }

		public Rank rank { get; }

	    public RankFile(char file, ushort rank)
	    {
		    this.file = file;
		    this.rank = rank;
	    }

	    public RankFile(Vec2<uint> position) : 
		    this(convertToFile(position[0]), convertToRank(position[1]))
	    {
		    
	    }

		public static implicit operator RankFile(Vec2<uint> position)  
		{
			return new RankFile(position);
		}
	    
	    public static implicit operator Vec2<uint>(RankFile boardRankAndFile)  
	    {
		    return ConvertToBoardPosition(boardRankAndFile);
	    }

	    public static Boolean operator == (RankFile boardPosition0, RankFile boardPosition1)
	    {
		    return (boardPosition0.rank == boardPosition1.rank) &&
		           (boardPosition0.file == boardPosition1.file);
	    }

	    public static bool operator != (RankFile boardPosition0, RankFile boardPosition1)
	    {
		    return !(boardPosition0 == boardPosition1);
	    }

	    public override Boolean Equals(object @object)
	    {
		    if (@object?.GetType() == this.GetType())
		    {
			    return this.Equals((RankFile) @object);
		    }
		    else
		    {
			    throw new NotImplementedException();
		    }
	    }

	    public bool Equals(RankFile other)
	    {
		    return this == other;
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
			    return (file.GetHashCode() * 397) ^ rank.GetHashCode();
		    }
	    }
    }

	[SuppressMessage("ReSharper", "NotAccessedField.Local")]
	public struct GameRecordEntry 
	{
	
		public struct AlgebraicNotation 
		{
		
			public char pieceSymbol;
			
			public RankFile destination;
			
			public AlgebraicNotation(Piece piece,  RankFile destination)
			{
				pieceSymbol = piece.Symbol;
				this.destination = destination;
			}		
		} 
	
		public AlgebraicNotation algrebraicNotation;		

		// ReSharper disable once UnusedMember.Local
		public GameRecordEntry(Piece piece, RankFile destination)
		{
			algrebraicNotation = new AlgebraicNotation(piece, destination);
		}
	}
}