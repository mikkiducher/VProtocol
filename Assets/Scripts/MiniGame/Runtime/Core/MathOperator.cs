using System;

namespace VProtocol.MiniGame.Runtime.Core
{
    [Flags]
    public enum MathOperator
    {
        None = 0,
        Add = 1 << 0,
        Subtract = 1 << 1,
        Multiply = 1 << 2,
        Divide = 1 << 3
    }
}
