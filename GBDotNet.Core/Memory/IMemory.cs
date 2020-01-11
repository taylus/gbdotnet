﻿using System.Collections.Generic;

namespace GBDotNet.Core
{
    public interface IMemory : IEnumerable<byte>
    {
        byte this[int index] { get; set; }
    }
}