using SFML.Graphics;

namespace Chess.View
{
	public interface ChessDrawable
	{
		Sprite Sprite { get; set; }
		
		void InitializeSprite();
	}
}