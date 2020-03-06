using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Br.Scania.ExternalAGV.Dapper
{
    public class Startup
    {
 
        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(
                _ => Configuration);
            services.AddTransient<AgvDAO>();
 
        }
 
    }
}