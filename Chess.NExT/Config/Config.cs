﻿using SFML.Window;

using Chess.Utility;
using Chess.View;

namespace Chess.Configuration
{
    using Resolution = Utility.Size;
    
    public static class Config
    {
        public const Mouse.Button ButtonMain = Mouse.Button.Left;

        public static readonly TrueColor WindowBackgroundColor = new TrueColor(0x18, 0x18, 0x18, 0x7F);
        
        public static readonly TrueColor WindowForegroundColor = new TrueColor(0, 196, 240, 0);
        
        public static Resolution MainWindowSize { get; } = DisplayData.getScreenResolution() / 2;

        public static readonly Resolution BoardResolution = new Resolution {Width = 1400, Height = 1400};

        public const ushort DefaultDisplayRefreshInterval = 8;

        public const float PieceScaleRelativeToSquare = 0.75f;

        public const string MainFontFilePath = "./Assets/Fonts/RobotoMono-Regular.ttf";
     
        public const uint DefaultTextCharacterSize = 60;
    }
}