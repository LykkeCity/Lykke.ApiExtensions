using Lykke.WebExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.HttpOverrides;

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
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });
        }
    }
}
