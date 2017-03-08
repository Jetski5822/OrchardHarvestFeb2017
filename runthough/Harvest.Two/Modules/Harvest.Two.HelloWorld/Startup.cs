using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Orchard.DeferredTasks;
using Orchard.Environment.Extensions.Features.Attributes;

namespace Harvest.Two.HelloWorld
{
    [OrchardFeature("Harvest.Two.HelloWorld.Additional")]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<TestInjection>();
        }

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            app.UseMiddleware<TestMiddleware>();
        }
    }

    
    public class TestInjection
    { }
}
