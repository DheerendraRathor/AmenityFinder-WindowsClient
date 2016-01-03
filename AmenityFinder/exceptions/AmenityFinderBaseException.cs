using System;

namespace AmenityFinder.exceptions
{
    public class AmenityFinderBaseException: Exception
    {
        public AmenityFinderBaseException()
        { 
        }

        public AmenityFinderBaseException(string message)
            : base(message)
        {
        }

        public AmenityFinderBaseException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
    
}
