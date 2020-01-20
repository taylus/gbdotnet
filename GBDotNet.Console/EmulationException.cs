﻿using System;

namespace GBDotNet.ConsoleApp
{
    [Serializable]
    public class EmulationException : Exception
    {
        public EmulationException() { }
        public EmulationException(string message) : base(message) { }
        public EmulationException(string message, Exception inner) : base(message, inner) { }
    }
}
