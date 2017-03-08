using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Orchard.Environment.Shell;
using Orchard.Hosting;

namespace Orchard.DeferredTasks
{
    /// <summary>
    /// Executes any deferred tasks when the request is terminated.
    /// </summary>
    public class TestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOrchardHost _orchardHost;

        public TestMiddleware(RequestDelegate next, IOrchardHost orchardHost)
        {
            _next = next;
            _orchardHost = orchardHost;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next.Invoke(httpContext);
        }
    }
}
