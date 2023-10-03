using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WebSocketLib.Middleware;
using WebSocketLib.Services;

namespace WebSocketLib
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddWebSocketDependencies(this IServiceCollection services)
        {
            services.AddTransient<IConnectionService, ConnectionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddScoped<IPayloadService, PayloadService>();

            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) return services;

            foreach (var type in assembly.ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(Hubs.WebSocketHub))
                {
                    services.AddTransient(type);
                }
            }
            return services;
        }

        public static IApplicationBuilder MapWebSocketHub<THub>(this IApplicationBuilder app, PathString path)
        {
            var hub = app.ApplicationServices.GetService<THub>();
            return app.Map("/ws" + path, (x) => x.UseMiddleware<WebSocketMiddleware>(hub));
        }
    }
}
