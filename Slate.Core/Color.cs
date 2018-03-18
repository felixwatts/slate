using System;

namespace Slate.Core
{
    public static class Color
    {
        public static readonly uint Black = FromRgba(0, 0, 0, 255);
        public static readonly uint White = FromRgba(255, 255, 255, 255);

        public static uint FromRgba(byte r, byte g, byte b, byte a)
        {
            return BitConverter.ToUInt32(new byte[]{ r, g, b, a }, 0);
        }

        public static byte[] ToRgba(uint color)
        {
            return BitConverter.GetBytes(color);
        }

        public static uint ContrastingColor(uint color) 
        {  
            var rgba = ToRgba(color);
            var luminance = 0.2126*rgba[0] + 0.7152*rgba[1] + 0.0722*rgba[2];
            return luminance > 128 ? Black : White;
        }
    }
}