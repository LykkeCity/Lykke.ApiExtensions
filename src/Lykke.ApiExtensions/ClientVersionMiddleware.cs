using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Http;
using Lykke.WebExtensions;

namespace Lykke.ApiExtensions
{
    public class ClientVersionMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientVersionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext ctx)
        {
            bool notSupported = false;
            try
            {
                //e.g. "User-Agent: DeviceType=iPhone;AppVersion=112"
                var userAgent = ctx.Request.GetUserAgent().ToString();

                if (userAgent.Contains("DeviceType") && userAgent.Contains("AppVersion"))
                {
                    var parameters = userAgent.Split(';');

                    var parametersDict = parameters.Select(parameter => parameter.Split('=')).ToDictionary(kv => kv[0], kv => kv[1]);

                    if (parametersDict["DeviceType"].ToLowCase() == "IPhone".ToLowCase())
                    {
                        if (parametersDict["AppVersion"] == "2222")
                        {
                            await ReturnNotSupported(ctx);
                            notSupported = true;
                        }

                        //...
                    }
                }
            }
            finally
            {
                if (!notSupported)
                {
                    await _next.Invoke(ctx);
                }
            }
        }

        private async Task ReturnNotSupported(HttpContext ctx)
        {
            var content = ResponseModel.CreateFail(ErrorCodes.VersionNotSupported, Phrases.VersionNotSupported).ToJson();

            var encoding = Encoding.UTF8;
            ctx.Response.StatusCode = 400;
            ctx.Response.ContentType = $"application/json; charset={encoding.WebName}";
            await ctx.Response.WriteAsync(content, encoding);
        }
    }
}