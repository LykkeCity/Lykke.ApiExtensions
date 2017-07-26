using System;
using System.Security.Principal;

namespace Lykke.ApiExtensions
{
    public class ApiClientIdentity : IIdentity
    {
        public string Name { get; private set; }
        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public DateTime Created { get; private set; }

        public static ApiClientIdentity Create(string clientId)
        {
            return new ApiClientIdentity
            {
                Name = clientId,
                AuthenticationType = "Token",
                Created = DateTime.UtcNow,
                IsAuthenticated = true
            };
        }
    }
}
