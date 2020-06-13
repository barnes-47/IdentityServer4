using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiHost
{
    public class Startup
    {
        #region Private Constants
        private const string _Audience = "api_host";
        private const string _Authority = "https://localhost:5001";
        private const string _DefaultScheme = "Bearer";
        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers();

            services
                .AddAuthentication(_DefaultScheme)

                //Install Microsoft.AspNetCore.Authentication.JwtBearer nuget package.
                .AddJwtBearer(_DefaultScheme, options =>
                {
                    options.Audience = _Audience;
                    options.Authority = _Authority;
                    options.RequireHttpsMetadata = false;       //Only for development scenarios.
                });

            services
                .AddCustomAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting();

            app
                .UseAuthentication()
                .UseAuthorization();

            app
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
