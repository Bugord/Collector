using System;

namespace Collector.BL.Exceptions
{
    public class AuthenticationFailException : Exception
    {
        public AuthenticationFailException()
        {
        }
        public AuthenticationFailException(string message)
            : base(message)
        { }
        public AuthenticationFailException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
