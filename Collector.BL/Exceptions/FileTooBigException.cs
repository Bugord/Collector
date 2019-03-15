using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Exceptions
{
    public class FileTooBigException : Exception
    {
        public FileTooBigException()
        {
        }

        public FileTooBigException(string message)
            : base(message)
        {
        }

        public FileTooBigException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
