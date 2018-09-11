namespace Chess.Util
{
    public struct TrueColor
    {
        
        //array <byte, 4> RGBA;
        private byte[] RGBA;

        public byte R
        {
            get { return RGBA[0]; }
            
            set { RGBA[0] = value; }
            
        }
        
        public byte G
        {
            get { return RGBA[1]; }
            
            set { RGBA[1] = value; }
            
        }
        
        public byte B
        {
            get { return RGBA[2]; }
            
            set { RGBA[2] = value; }
            
        }
        
        public byte A
        {
            get { return RGBA[3]; }
            
            set { RGBA[3] = value; }
            
        }

        public TrueColor (byte R, byte G, byte B, byte A)
        {
            RGBA = new byte[] {R, G, B, A};
        }
        
        public static implicit operator TrueColor(byte[] bytes)
        {
            return new TrueColor(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public static implicit operator byte[](TrueColor color)
        {
            return color.RGBA;
        }

        public ColorType ConvertToOtherColorType<ColorType>() where ColorType : new()
        {
            var color = new ColorType();
            color.GetType().GetFields()[0] = (dynamic) R;
            color.GetType().GetFields()[1] = (dynamic) G;
            color.GetType().GetFields()[2] = (dynamic) B;
            color.GetType().GetFields()[3] = (dynamic) A;

            return color;
        }
    }
}