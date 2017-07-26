using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Lykke.ApiExtensions
{
    public static class HttpRequestExtensions
    {
        public static string GetUserAgent(this HttpRequest request, string name)
        {
            var parametersDict = request.GetUserAgent();
            return parametersDict.ContainsKey(name) ? parametersDict[name] : null;
        }

        public static IDictionary<string, string> GetUserAgent(this HttpRequest request)
        {
            var parameters = request.GetUserAgentHeader().Split(';');
            return parameters.Select(parameter => parameter.Split('=')).Where(kv => kv.Length > 1).ToDictionary(kv => kv[0], kv => kv[1]);
        }

        public static string GetUserAgentHeader(this HttpRequest request)
        {
            return request.Headers["User-Agent"].ToString();
        }
    }
}
