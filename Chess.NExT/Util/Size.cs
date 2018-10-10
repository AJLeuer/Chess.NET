using SFML.System;

namespace Chess.Util
{
	public struct Size
	{
		private Vec2<uint> value;

		public uint Width { get { return value.X; } }
		
		public uint Height { get { return value.Y; } }

		public Size(uint width, uint height) :
			this(new Vec2<uint>(width, height))
		{
			
		}

		public Size(Vec2<uint> value)
		{
			this.value = value;
		}
		
		public static implicit operator Size (Vector2u sfmlVector)
		{
			return new Size(sfmlVector);
		}
		
		public static implicit operator Vector2u (Size size)
		{
			return size.value;
		}
		
		public static Size operator + (Size size, Vec2<uint> vector)
		{
			Vec2<uint> sum = size.value + vector;
			return new Size(sum);
		}
		
		public static Vec2<uint> operator + (Vec2<uint> vector, Size size)
		{
			return vector + size.value;
		}
	}
}