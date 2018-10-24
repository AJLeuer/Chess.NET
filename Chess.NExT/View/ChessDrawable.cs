using Chess.Util;
using SFML.Graphics;

namespace Chess.View
{
	public interface ChessDrawable
	{
		Sprite Sprite { get; set; }
		
		Size Size { get; }
		
		Vec2<uint> Coordinates2D { get; set; }
		
		void InitializeGraphicalElements();

		void Initialize2DCoordinates(Vec2<uint> coordinates);

		void Draw(RenderTarget renderer);
	}
}