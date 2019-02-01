using Chess.Utility;
using SFML.Window;

namespace Chess.View
{
    public static class DisplayData 
    {

        public static Vec2<uint> getScreenResolution()
        {
            VideoMode currentVideoMode = VideoMode.DesktopMode;
            return new Vec2<uint>(currentVideoMode.Width, currentVideoMode.Height);
        }

        public static double getScreenRefreshRate()
        {
            //todo: implement
            return 165;
        }

        /// <returns>The display scaling factor.
        /// For example, if the system is running in Retina mode,
        /// this value will be 2.0</returns>
        public static float getDisplayScalingFactor() 
        {
            //todo: implement without hard-coded values
            return 2.0f;
        }

    }
}