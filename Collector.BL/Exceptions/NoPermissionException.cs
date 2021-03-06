﻿using System;

namespace Collector.BL.Exceptions
{
    public class NoPermissionException : Exception
    {
        public NoPermissionException()
        {
        }

        public NoPermissionException(string message)
            : base(message)
        {
        }

        public NoPermissionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}