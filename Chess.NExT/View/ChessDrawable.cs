using Chess.Util;
using SFML.Graphics;

namespace Chess.View
{
	public interface ChessDrawable
	{
		Sprite Sprite { get; set; }
		
		Size Size { get; }
		
		Vec2<uint> Position2D { get; set; }
		
		void InitializeGraphicalElements();

		void Initialize2DPosition(Vec2<uint> position);
	}
}