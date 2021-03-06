using System;

namespace Lykke.ApiExtensions
{
    public interface IRequestsLogRecord
    {
        DateTime DateTime { get; set; }
        string Url { get; set; }
        string Request { get; set; }
        string Response { get; set; }
        string UserAgent { get; set; }
    }
}