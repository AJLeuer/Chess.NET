using Chess.View;
using static SFML.Window.Mouse;

namespace Chess.Util
{
    using Resolution = Chess.Util.Size;
    
    public static class Config
    {
        public static bool GameActive = false;

        public const Button ButtonMain = Button.Left;

        public const ushort BoardWidth = 8;
        public const ushort BoardHeight = 8;

        public static readonly TrueColor WindowBackgroundColor = new TrueColor(0x18, 0x18, 0x18, 0x7F);
        
        public static readonly TrueColor WindowForegroundColor = new TrueColor(0, 196, 240, 0);
        
        public static Resolution MainWindowSize { get; } = DisplayData.getScreenResolution() / 2;

        public static readonly Resolution BoardResolution = new Resolution {Width = 1440, Height = 1440};

        public const float PieceScaleRelativeToSquare = 0.75f;

        public const string MainFontFilePath = "./Assets/Fonts/RobotoMono-Regular.ttf";
     
        public const uint defaultTextCharacterSize = 60;
    }
}