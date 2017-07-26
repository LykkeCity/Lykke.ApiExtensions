using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.ApiExtensions
{
    public static class ControllerExtensions
    {
        public static string GetClientId(this Controller controller)
        {
            return controller.User?.Identity?.Name ?? throw new NotAuthenticatedException(Phrases.NotAuthenticated);
        }
    }
}
