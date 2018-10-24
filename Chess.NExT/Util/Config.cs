using Chess.View;
using static SFML.Window.Mouse;

namespace Chess.Util
{
    using Resolution = Chess.Util.Size;
    
    public static class Config
    {
        public static bool GameActive = false;

        public const Button ButtonMain = Button.Left;

        public static Vec2<uint> MainWindowSize { get; set; } = DisplayData.getScreenResolution();

        public static readonly TrueColor WindowBackgroundColor = new TrueColor(0x18, 0x18, 0x18, 0x7F);
        
        public static readonly TrueColor WindowForegroundColor = new TrueColor(0, 196, 240, 0);

        public static readonly Resolution BoardResolution = new Resolution {Width = 2560, Height = 2560};

        public const string MainFontFilePath = "./Assets/Fonts/RobotoMono-Regular.ttf";
     
        public const uint defaultTextCharacterSize = 60;
    }
}