using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(BugTracker.Areas.Identity.IdentityHostingStartup))]

namespace BugTracker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}