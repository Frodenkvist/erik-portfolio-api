using System;

namespace ErikPortfolioApi.exceptions
{
    public class FailedLoginException : Exception
    {
        public FailedLoginException(string message) : base(message)
        {
        }
    }
}
