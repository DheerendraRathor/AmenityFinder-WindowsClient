using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmenityFinder.exceptions
{
    public class HttpRequestFailedException: AmenityFinderBaseException
    {

        public HttpRequestFailedException()
        {
        }

        public HttpRequestFailedException(string message)
            :base(message)
        {
        }

        public HttpRequestFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
