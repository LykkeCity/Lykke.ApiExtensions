using System;
using System.Collections.Generic;
using System.Text;
using AspNetCoreRateLimit;
using Autofac;

namespace Lykke.ApiExtensions
{
    public static class AutofacExtensions
    {
        public static void RegisterApiExtensionDependencies(this ContainerBuilder builder, Func<IComponentContext, ClientAuthSettings> clientAuthSettings)
        {
            builder.RegisterType<MemoryCacheIpPolicyStore>().As<IIpPolicyStore>().SingleInstance();
            builder.RegisterType<MemoryCacheRateLimitCounterStore>().As<IRateLimitCounterStore>().SingleInstance();

            builder.Register(clientAuthSettings).AsSelf().SingleInstance();
        }
    }
}
