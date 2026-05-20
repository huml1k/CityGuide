using System;
using System.Collections.Generic;
using System.Text;

namespace ContentService.Application.Common.Exceptions
{
    public class InvalidFileException : Exception
    {
        public InvalidFileException()
            : base("Invalid file.")
        {
        }

        public InvalidFileException(string message)
            : base(message)
        {
        }

        public InvalidFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
