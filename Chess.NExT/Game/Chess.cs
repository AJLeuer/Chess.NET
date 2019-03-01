using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Chess.Utility;

using Position = Chess.Utility.Vec2<uint>;
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
		private Color() {}
		
		public static readonly Color white = new White();
		public static readonly Color black = new Black();
		
		public class White : Color {}
		public class Black : Color {}
	}
    
	public static class ColorMethods {

		public static Color GetOpposite(this Color color)
		{
			switch (color)
			{
				case Color.White _:
					return Color.black;
				case Color.Black _:
					return Color.white;
				default:
					throw new InvalidEnumArgumentException();
			}
		}
	}
	
	public class Direction
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

		public static readonly Direction up = new Direction(0, 1);

		public static readonly Direction down = new Direction(0, -1);
		
		public static readonly Direction left = new Direction(-1, 0);
	
		public static readonly Direction right = new Direction(1, 0);
	
		public static readonly Direction upLeft = new Direction(-1, 1);
	
		public static readonly Direction upRight = new Direction(1, 1);
	
		public static readonly Direction downLeft = new Direction(-1, -1);
	
		public static readonly Direction downRight = new Direction(1, -1);
	
		
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

		// ReSharper disable once UnusedMember.Local
		private const Rank lastRank = 8;

		public static File ConvertToFile(uint boardPositionX) 
		{
			var firstFileCharCode = (Int32)firstFile;
			char c = (char)(boardPositionX + firstFileCharCode);
			return c;
		}
 
		public static Rank ConvertToRank(uint boardPositionY) 
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
	    
	    public static Position ConvertToBoardPosition (RankFile rankAndFile)  
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

		    uint y = convertToBoardPositionIntegerY(rankAndFile.Rank);

		    //convert file to x:
		    uint x = convertToBoardPositionIntegerX(rankAndFile.File);

		    return new Position(x, y);
	    }

		private File file;
		
		public File File
		{
			get { return file; }
			
			private set { file = Char.ToLower(value); }
		}

		public Rank Rank { get; }

	    public RankFile(char file, ushort rank) : this()
		{
		    File = file;
		    Rank = rank;
	    }

	    public RankFile(Position position) : 
		    this(ConvertToFile(position[0]), ConvertToRank(position[1]))
	    {
		    
	    }

		public static RankFile CreateRankFileFromString(String rankFileString)
		{
			char file = rankFileString[0];
			ushort rank = UInt16.Parse(rankFileString[1].ToString()); //rankFileString[1];
			return new RankFile(file, rank);
		}
	    
		public static implicit operator RankFile (ValueTuple<char, uint> tuple)
		{
			return new RankFile(tuple.Item1, (ushort) tuple.Item2);
		}
		
		public static implicit operator (char, uint) (RankFile rankAndFile)
		{
			return (rankAndFile.file, rankAndFile.Rank);
		}

		public static implicit operator RankFile(Position position)  
		{
			return new RankFile(position);
		}
	    
	    public static implicit operator Position(RankFile rankAndFile)  
	    {
		    return ConvertToBoardPosition(rankAndFile);
	    }

	    public static Boolean operator == (RankFile boardPosition0, RankFile boardPosition1)
	    {
		    return (boardPosition0.Rank == boardPosition1.Rank) &&
		           (boardPosition0.File == boardPosition1.File);
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
			else if (@object?.GetType() == typeof(ValueTuple<char, uint>))
			{
				ValueTuple<char, uint> tuple = (ValueTuple<char, uint>) @object;
				RankFile other = tuple;
				return this.Equals(other);
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
			    return (File.GetHashCode() * 397) ^ Rank.GetHashCode();
		    }
	    }
	    
		public override string ToString()
		{
			return $"{File}{Rank}";
		}
    }

	[SuppressMessage("ReSharper", "NotAccessedField.Local")]
	public struct GameRecordEntry 
	{
	
		public struct AlgebraicNotation 
		{
		
			public char pieceSymbol;
			
			public RankFile destination;
			
			public AlgebraicNotation(IPiece piece,  RankFile destination)
			{
				pieceSymbol = piece.Symbol;
				this.destination = destination;
			}		
		} 
	
		public AlgebraicNotation AlgrebraicNotation;		

		// ReSharper disable once UnusedMember.Local
		public GameRecordEntry(IPiece piece, RankFile destination)
		{
			AlgrebraicNotation = new AlgebraicNotation(piece, destination);
		}
	}
}