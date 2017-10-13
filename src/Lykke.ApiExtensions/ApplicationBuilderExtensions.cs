using System;
using Lykke.WebExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.ApiExtensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseApiExtensions(this IApplicationBuilder app)
        {
            app.UseCors("Lykke");
            app.Use(next => context =>
            {
                context.Request.EnableRewind();

                return next(context);
            });

            app.UseWebExtensions();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseMiddleware<ClientAuthMiddleware>();
            app.UseMiddleware<ThrottlingMiddleware>();
            app.UseMiddleware<ClientBansMiddleware>();
        }

        public static void AddApiExtensions(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer();
        }
    }
}
