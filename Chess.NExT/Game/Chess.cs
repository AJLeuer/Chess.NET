using System;
using System.ComponentModel;
using Chess.Util;
using static Chess.Util.Util;

using File = System.Char;
using Rank = System.UInt16;
	
namespace Chess.Game
{
	public enum Color
	{
		white,
		black
	}
    
	public static class ColorMethods {

		public static Color getOpposite(this Color color)
		{
			switch (color)
			{
				case Color.white:
					return Color.black;
				case Color.black:
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

    public struct RankAndFile {
	    
		private const File firstFile = 'a';

		private const File lastFile = 'h';

		private const Rank firstRank = 1;

		private const Rank lastRank = 8;

		private static File convertToFile(uint x) {
			char c = (char)(x + lowerCaseA);
			return c;
		}
 
		private static Rank convertToRank(uint y) {
			var rank = y + 1;
			return (Rank)rank;
		}

	    private static uint convertToInteger(File file)
	    {
		    int intValue = (int)(file - lowerCaseA);
		    return (uint)intValue;
	    }
	    
	    private static uint convertToInteger(Rank rank)
	    {
		    int intValue = (lastRank - rank) % lastRank;
		    return (uint) intValue;
	    }

		public File file { get; }

		public Rank rank { get; }

	    public RankAndFile(char file, ushort rank)
	    {
		    this.file = file;
		    this.rank = rank;
	    }

	    public RankAndFile(Vec2<uint> position) : 
		    this(convertToFile(position[0]), convertToRank(position[1]))
	    {
		    
	    }

		public static implicit operator RankAndFile(Vec2<uint> position)  
		{
			return new RankAndFile(position);
		}
	    
	    public static implicit operator Vec2<uint>(RankAndFile rankAndFile)  
	    {
		    return rankAndFile.convertToPosition();
	    }

		public Vec2<uint> convertToPosition ()  {

			/* Chess ranks start with 1 at the bottom and increase as we move up the board,
			 but window coordinates start with y = 0 at the top, and y increases with descent.
			 So to convert our rank to a y-coordinate, we'll need to essentially swap each rank
			 value across the horizontal axis, and subtract 1 from it (rank values start at 1, but
			 vec2<int> values start at 0)

			 Chess files can be converted from x-coordinates by subtracting the Unicode decimal value for 'a' -
			 97 - from the decimal value of the char representing the rank. Applied to 'a', this gives an x-coord
			 of 0, 'b' outputs as 1, 'c' is 2, etc.
			 */

			uint y = convertToInteger(rank);

			//convert file to x:
			uint x = convertToInteger(file);

			return new Vec2<uint>(x, y);
		}

	}

	public struct GameRecordEntry {
	
		struct AlgebraicNotation {
		
			char pieceSymbol;
			
			RankAndFile destination;
			
			public AlgebraicNotation(Piece piece,  RankAndFile destination)
			{
				pieceSymbol = piece.symbol;
				this.destination = destination;
			}		
		} 
	
		AlgebraicNotation algrebraicNotation;
		
		RankAndFile startingPosition;

		GameRecordEntry(Piece piece, RankAndFile destination)
		{
			algrebraicNotation = new AlgebraicNotation(piece, destination);
			startingPosition = piece.position;
		}
	}

	public class MoveAction : IComparable, IComparable<MoveAction>
	{
		public Player player { get; }
		
		public Piece piece { get; }
		
		public Square destination { get; }
		
		protected Board board { get { return piece.board; } }
		
		public short value { get; protected set; }

		public MoveAction(Player player, Piece piece, Square destination)
		{
			this.player = player;
			this.piece = piece;
			this.destination = destination;
			calculateValue();
		}
		
		public static bool operator > (MoveAction moveIntent0, MoveAction moveIntent1)
		{
			return moveIntent0.value > moveIntent1.value;
		}

		public static bool operator < (MoveAction moveIntent0, MoveAction moveIntent1)
		{
			return moveIntent0.value < moveIntent1.value;
		}

		public int CompareTo(object @object)
		{
			if (@object.GetType() == typeof(MoveAction))
			{
				return CompareTo((MoveAction) @object);
			}
			else
			{
				throw new NotImplementedException();
			}
		}
		
		public int CompareTo(MoveAction move)
		{
			if (this > move)
			{
				return 1;
			}
			else if (this < move)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}

		protected void calculateValue()
		{
			short startingValue = board.evaluate(player);

			short valueAfterMove = board.evaluateAfterHypotheticalMove(player, piece, destination.rankAndFile);

			value = (short)(valueAfterMove - startingValue);
		}

		public void commit()
		{
			piece.move(destination);
		}

	}
}