using System;

namespace Collector.BL.Exceptions
{
    public class NoPermissionException : Exception
    {
        public NoPermissionException(string message)
            : base(message)
        { }
    }
}
