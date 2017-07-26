using System;
using Newtonsoft.Json;

namespace Lykke.ApiExtensions
{
    public class ClientAuthSettings
    {
        [JsonProperty(Required = Required.Always)]
        public TimeSpan SessionRefreshPeriod { get; set; }
    }
}