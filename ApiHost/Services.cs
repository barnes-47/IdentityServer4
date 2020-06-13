using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ApiHost
{
    public static class Services
    {
        #region Public Methods
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policy.AdminAccess, builder => { builder.RequireScope(Scope.ApiHost_FullAccess); });
                options.AddPolicy(Policy.DeleteAccess, builder => { builder.RequireScope(Scope.ApiHost_FullAccess, Scope.ApiHost_DeleteAccess); });
                options.AddPolicy(Policy.ReadAccess, builder => { builder.RequireScope(Scope.ApiHost_FullAccess, Scope.ApiHost_ReadAccess); });
                options.AddPolicy(Policy.WriteAccess, builder => { builder.RequireScope(Scope.ApiHost_FullAccess, Scope.ApiHost_WriteAccess); });
            });

            return services;
        }
        #endregion
    }
}
