using System;

namespace GBDotNet.Core
{
    [Flags]
    public enum Button
    {
        Start = 128,
        Select = 64,
        B = 32,
        A = 16,
        Down = 8,
        Up = 4,
        Left = 2,
        Right = 1
    }
}
