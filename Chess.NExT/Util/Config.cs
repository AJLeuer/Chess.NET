using Chess.View;
using static SFML.Window.Mouse;

namespace Chess.Util
{

    public static class Config
    {
        public static bool gameActive = false;
        
        public static Button buttonMain = Button.Left;
        
        public static Vec2<uint> mainWindowSize { get; set; } = DisplayData.getScreenResolution();

        public static readonly TrueColor windowBackgroundColor = new TrueColor(0x18, 0x18, 0x18, 0x7F);
        
        public static readonly TrueColor windowForegroundColor = new TrueColor(0, 196, 240, 0);
        
        public static string mainFontFilePath = "./Assets/Fonts/RobotoMono-Regular.ttf";
    }
}