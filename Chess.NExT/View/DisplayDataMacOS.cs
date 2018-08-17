using SFML.Window;
using MonoMac.CoreVideo;
using Chess.Util;

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
            var display = new CVDisplayLink();
            return display.ActualOutputVideoRefreshPeriod;
        }

        /**
         * The display scaling factor.
         * For example, if the system is running in Retina mode,
         * this value will be 2.0
         */
        public static float getDisplayScalingFactor()
        {
            //todo: implement without hard-coded values
            return 2.0f;
        }

    }
}
