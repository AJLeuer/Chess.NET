using Chess.Util;
using SFML.Graphics;
using SFML.Window;
using static Chess.Util.Config;

namespace Chess.View
{

	public class Window : RenderWindow
	{

		protected static readonly VideoMode defaultVideoMode;

		protected static readonly Font defaultFont;

		protected static VideoMode createDefaultVideoMode()
		{
			Vec2<uint> baseWindowSize = MainWindowSize;
			
			float dpiScale = DisplayData.getDisplayScalingFactor();
			
			Vec2 <uint> scaledWindowSize = new Vec2<uint>((uint)(baseWindowSize.X * dpiScale),
				(uint)(baseWindowSize.Y * dpiScale));
			
			return new VideoMode(scaledWindowSize.X, scaledWindowSize.Y);
		}

		static Window()
		{
			defaultVideoMode = createDefaultVideoMode();
			defaultFont = new Font(MainFontFilePath);
		}

		public Text text { get; private set; }

		public TrueColor backgroundColor { get; private set;}

		public Window(string title = "Chess", TrueColor backgroundColor = new TrueColor()) :
			base(defaultVideoMode, title, Styles.Default, new ContextSettings())
		{
			text = new Text
			{
				Font = defaultFont,
				CharacterSize = defaultTextCharacterSize
			};
			this.backgroundColor = backgroundColor;
			this.SetVisible(true);
		}

		public void setBackgroundColor(TrueColor color)
		{
			this.backgroundColor = color;
		}

		public void displayText(string chars, TrueColor color, Vec2<uint> where) {
			text.DisplayedString = chars;
			text.FillColor = color.ConvertToSFMLColorType();

			var textSize = text.GetLocalBounds();

			Vec2<uint> middle = new Vec2<uint>((uint)(textSize.Width / 2), (uint)(textSize.Height / 2));

			Vec2<uint> adjustedPos = where - middle;

			text.Position = adjustedPos;

			Draw(text);
		}
	}

}