using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Modules;
using Microsoft.AspNetCore.Mvc.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orchard.Environment.Extensions;

namespace Harvest.Demo2
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("logging.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"logging.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddExtensionLocation("CustomModules");
            /* 
              AddModuleServices Is registering default Loaders, Locations, and services for loading up modules.
              Modules are broken down in to what's deemed Extensions, Features and Manifests. Manifests are 
              how a module is described to the system, without it that module wont be found.
            */
            services.AddModuleServices(configure => configure
                .AddMvcModules(services.BuildServiceProvider())
                .AddConfiguration(Configuration)
                .WithAllFeatures() // Tell the system, that all tenants will receive all the same features.
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            loggerFactory.AddConsole(Configuration);

            // Registers the multi tenant middleware, and router middleware.
            app.UseModules(apb =>
            {
                apb.UseStaticFilesModules();
            });
        }
    }
}
