using System;

namespace Collector.BL.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message)
            : base(message)
        { }
    }
}
