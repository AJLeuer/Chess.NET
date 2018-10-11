using Chess.Util;
using SFML.Graphics;

using Position = Chess.Util.Vec2<uint>;

namespace Chess.View
{
	public interface ChessDrawable
	{
		Sprite Sprite { get; set; }
		
		Size Size { get; }
		
		Position Position2D { get; set; }
		
		void InitializeGraphicalElements();

		void Initialize2DPosition(Position position);
	}
}