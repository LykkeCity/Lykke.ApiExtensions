using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Log;
using Microsoft.AspNetCore.Http;
using Lykke.Service.Session;
using Lykke.WebExtensions;

namespace Lykke.ApiExtensions
{
    public class ClientAuthMiddleware
    {
        private readonly ClaimsCache _claimsCache = new ClaimsCache();
        private readonly IClientsSessionsRepository _sessions;
        private readonly IRequestsLogRepository _requestLogs;

        private readonly RequestDelegate _next;
        private readonly ClientAuthSettings _settings;

        public ClientAuthMiddleware(RequestDelegate next, ClientAuthSettings settings, IClientsSessionsRepository clientsSessionsRepository, IRequestsLogRepository requestLogRepository, ILog log)
        {
            _next = next;
            _settings = settings;
            _sessions = clientsSessionsRepository;
            _requestLogs = requestLogRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            await Authenticate(context);
            await _next.Invoke(context);
        }

        private async Task Authenticate(HttpContext context)
        {
            var header = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(header))
                return;

            var values = header.Split(' ');

            if (values.Length != 2)
                return;

            if (values[0] != "Bearer")
                return;

            if (string.IsNullOrEmpty(values[1]))
                return;

            var principal = await ReadPrincipal(values[1]);
            context.User = principal;

            if (principal != null)
            {

                var request = await ReadRequest(context.Request);

                if (ShouldWeWriteToLog(context.Request))
                    await _requestLogs.WriteAsync(principal.Identity.Name,
                        "[" + context.Request.Method + "]" + context.Request.GetUri().PathAndQuery,
                        request, null, context.Request.GetUserAgentHeader());
            }
        }

        private async Task<ClaimsPrincipal> ReadPrincipal(string token)
        {

            var result = _claimsCache.Get(token);
            if (result != null)
                return result;

            var session = await _sessions.GetAsync(token);
            if (session == null)
                return null;

            if (DateTime.UtcNow - session.LastAction > _settings.SessionRefreshPeriod)
            {
                await _sessions.RefreshSessionAsync(token);
            }

            result = new ClaimsPrincipal(ApiClientIdentity.Create(session.ClientId));
            if (session.PartnerId != null)
            {
                (result.Identity as ClaimsIdentity)?.AddClaim(new Claim("PartnerId", session.PartnerId));
            }
            _claimsCache.Set(token, result);
            return result;
        }

        private async Task<string> ReadRequest(HttpRequest request)
        {
            if (request.Method == "GET")
                return null;

            var url = request.GetUri().PathAndQuery.ToLower();
            if (url.Contains("kycdocumentsbin") || url.Contains("kycdocumentupload"))
                return null;

            try
            {
                using (var ms = new MemoryStream())
                {
                    await request.Body.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var sr = new StreamReader(ms))
                    {
                        return await sr.ReadToEndAsync();
                    }
                }
            }
            finally
            {
                request.Body.Seek(0, SeekOrigin.Begin);
            }
        }

        private static bool ShouldWeWriteToLog(HttpRequest request)
        {
            return !request.GetUri().PathAndQuery.ToLower().Contains("assetpairrates");
        }
    }
}
