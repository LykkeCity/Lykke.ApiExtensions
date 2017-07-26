using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Lykke.ApiExtensions
{
    public class ClaimsCache
    {
        private readonly int _secondsToExpire;

        public class PrincipalCashItem
        {
            public ClaimsPrincipal ClaimsPrincipal { get; set; }
            public DateTime LastRefresh { get; private set; }

            public static PrincipalCashItem Create(ClaimsPrincipal src)
            {
                return new PrincipalCashItem
                {
                    LastRefresh = DateTime.UtcNow,
                    ClaimsPrincipal = src
                };
            }
        }

        private readonly Dictionary<string, PrincipalCashItem> _claimsCache = new Dictionary<string, PrincipalCashItem>();

        public ClaimsCache(int secondsToExpire = 60)
        {
            _secondsToExpire = secondsToExpire;
        }

        public ClaimsPrincipal Get(string token)
        {
            lock (_claimsCache)
            {
                if (!_claimsCache.ContainsKey(token))
                    return null;

                var result = _claimsCache[token];

                if ((DateTime.UtcNow - result.LastRefresh).TotalSeconds < _secondsToExpire)
                    return result.ClaimsPrincipal;

                _claimsCache.Remove(token);
                return null;
            }
        }

        public void Set(string token, ClaimsPrincipal principal)
        {
            lock (_claimsCache)
            {
                if (_claimsCache.ContainsKey(token))
                    _claimsCache[token] = PrincipalCashItem.Create(principal);
                else
                    _claimsCache.Add(token, PrincipalCashItem.Create(principal));
            }
        }
    }
}