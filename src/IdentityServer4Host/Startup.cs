using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer4Host
{
    public class Startup
    {
        /// <summary>
        /// This method gets called by the runtime.
        /// Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940        
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers();
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()    //Temporary signing credential. Only for development scenarios.
                .AddInMemoryApiResources(Config.ApiResources)   //Adds the ApiResources defined in the Config file.
                .AddInMemoryClients(Config.Clients);            //Adds the Clients defined in the Config file.
        }

        /// <summary>
        /// This method gets called by the runtime.
        /// Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();    //Adds the IdentityServer4 to the HTTP request pipeline.

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
