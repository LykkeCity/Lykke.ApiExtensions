using System;

namespace Lykke.ApiExtensions
{
    public class NotAuthenticatedException : InvalidOperationException
    {
        public NotAuthenticatedException(string message) : base(message) { }
    }
}