using System;

namespace Collector.BL.Exceptions
{
    public class ServerFailException : Exception
    {
        public ServerFailException(string message)
            : base(message)
        { }
    }
}
