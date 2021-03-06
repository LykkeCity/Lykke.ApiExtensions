﻿using System;
using AspNetCoreRateLimit;
using Common.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lykke.ApiExtensions
{
    public class ThrottlingMiddleware : IpRateLimitMiddleware
    {
        private readonly ILog _log;

        public ThrottlingMiddleware(RequestDelegate next, IOptions<IpRateLimitOptions> options, IRateLimitCounterStore counterStore, IIpPolicyStore policyStore, ILogger<IpRateLimitMiddleware> logger, ILog log, IIpAddressParser ipParser = null) 
            : base(next, options, counterStore, policyStore, logger, ipParser)
        {
            _log = log;
        }

        public override void LogBlockedRequest(HttpContext httpContext, ClientRequestIdentity identity, RateLimitCounter counter, RateLimitRule rule)
        {
            _log?.WriteWarningAsync(nameof(ApiExtensions), nameof(ThrottlingMiddleware), identity.ClientIp,
                $"{DateTime.UtcNow} Request {httpContext.TraceIdentifier} from {identity.ClientIp} to endpoint {rule.Endpoint} has been throttled (blocked), quota {rule.Limit}/{rule.Period} exceeded by {counter.TotalRequests}");
        }
    }
}