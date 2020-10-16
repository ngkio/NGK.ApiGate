using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ngk.ApiGate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("ocelot.json", true, true)
                        .AddJsonFile($"ocelot.{env.EnvironmentName}.json", true, true);
                })
                .UseUrls("http://*:29890")
                .UseStartup<Startup>()
                .Build();
        }
    }
}