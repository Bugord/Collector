using System;

namespace Collector.BL.Exceptions
{
    public class ServerFailException : Exception
    {
        public ServerFailException()
        {
        }
        public ServerFailException(string message)
            : base(message)
        { }
        public ServerFailException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
