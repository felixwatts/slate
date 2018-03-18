using System;

namespace Slate.Core
{
    public static class Color
    {
        public static readonly uint Black = FromArgb(255, 0, 0, 0);

        public static uint FromArgb(byte a, byte r, byte g, byte b)
        {
            return BitConverter.ToUInt32(new byte[]{ a, r, g, b }, 0);
        }
    }
}