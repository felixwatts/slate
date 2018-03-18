using System;

namespace Slate.Core
{
    public static class Color
    {
        public static readonly uint Black = FromArgb(255, 0, 0, 0);
        public static readonly uint White = FromArgb(255, 255, 255, 255);

        public static uint FromArgb(byte a, byte r, byte g, byte b)
        {
            return BitConverter.ToUInt32(new byte[]{ r, g, b, a }, 0);
        }
    }
}