using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Microsoft.AspNetCore.Http;

namespace Lykke.ApiExtensions
{
    public class ClientBansMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBannedClientsRepository _bannedClientsRepository;
        private readonly ILog _log;

        public ClientBansMiddleware(RequestDelegate next, IBannedClientsRepository bannedClientsRepository,
            ILog log)
        {
            _bannedClientsRepository = bannedClientsRepository;
            _log = log;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            bool clientBanned = false;
            string clientId = string.Empty;
            try
            {
                clientId = context.User?.Identity?.Name;

                if (!string.IsNullOrEmpty(clientId))
                {
                    clientBanned = await _bannedClientsRepository.IsClientBannedWithCache(clientId, 1);
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(ApiExtensions), nameof(ClientBansMiddleware), clientId, ex);
            }
            finally
            {
                if (!clientBanned)
                    await _next.Invoke(context);
                else
                    context.Response.StatusCode = 403;
            }
        }
    }

    public interface IBannedClientsRepository
    {
        Task BanClient(string clientId);
        Task UnBanClient(string clientId);

        Task<IEnumerable<string>> GetBannedClients();
        Task<bool> IsClientBannedWithCache(string clientId, int cacheMinutes);
        Task<bool> IsClientBanned(string clientId);
    }

}