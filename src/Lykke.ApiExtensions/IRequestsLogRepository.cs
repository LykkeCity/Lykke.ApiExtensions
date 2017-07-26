using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.ApiExtensions
{
    public interface IRequestsLogRepository
    {
        Task WriteAsync(string clientId, string url, string request, string response, string userAgent);
        Task<IEnumerable<IRequestsLogRecord>> GetRecords(string clientId, DateTime from, DateTime to);
    }
}