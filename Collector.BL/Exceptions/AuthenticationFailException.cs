using System;

namespace Collector.BL.Exceptions
{
    public class AuthenticationFailException : Exception
    {
        public AuthenticationFailException(string message)
            : base(message)
        { }
    }
}
