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

    public class HttpRequestInvalidDataException : AmenityFinderBaseException
    {

        public HttpRequestInvalidDataException()
        {
        }

        public HttpRequestInvalidDataException(string message)
            : base(message)
        {
        }

        public HttpRequestInvalidDataException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

    public class ForbiddenRequestException : AmenityFinderBaseException
    {
        public ForbiddenRequestException()
            : base("This request is forbidden")
        {
        }

        public ForbiddenRequestException(string message)
            : base(message)
        {
        }

        public ForbiddenRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class BadRequestException : AmenityFinderBaseException
    {
        public BadRequestException()
            : base("There is some error in data posted")
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class ServerException : AmenityFinderBaseException
    {
        public ServerException()
            : base("Server Error. Please try again some time later")
        {
        }

        public ServerException(string message)
            : base(message)
        {
        }

        public ServerException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

}
