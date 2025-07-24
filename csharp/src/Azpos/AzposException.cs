using System;

namespace Azpos
{
    public class AzposException : Exception
    {
        public AzposException(string message) : base(message) { }
    }
}
