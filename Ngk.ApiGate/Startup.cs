using Ngk.ApiGate.Extensions;
using log4net;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Cache.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.Provider.Consul;
using Ocelot.Request.Middleware;
using Ocelot.Requester.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using Thor.Framework.Common.Helper;
using Thor.Framework.Service.WebApi.Middleware;

namespace Ngk.ApiGate
{
    public class Startup
    {
        private static ILoggerRepository Repository { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //初始化log4net
            Repository = LogManager.CreateRepository("NgkRepository");
            Log4NetHelper.SetConfig(Repository, "log4net.config");
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
//            services.AddAuthentication()
//                .AddIdentityServerAuthentication("WalletKey", option =>
//                {
//                    option.Authority = Configuration["IdentityService:Uri"];
//                    option.ApiName = "Wallet";
//                    option.RequireHttpsMetadata = Convert.ToBoolean(Configuration["IdentityService:UseHttps"]);
//                    option.SupportedTokens = SupportedTokens.Both;
//                    option.ApiSecret = Configuration["IdentityService:ApiSecrets:Wallet"];
//                });
//            Console.WriteLine(Configuration["IdentityService:Uri"]);
//            Console.WriteLine(Configuration["GlobalConfiguration:ServiceDiscoveryProvider:Host"]);

            // Ocelot
            services.AddOcelot(Configuration).AddConsul();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseGlobalExceptionHandler();
//            app.UseOcelot().Wait();
            app.UseOcelot((builder, pipeConfig) =>
            {
                builder.UseExceptionHandlerMiddleware();
                // Allow the user to respond with absolutely anything they want.
                if (pipeConfig.PreErrorResponderMiddleware != null)
                {
                    builder.Use(pipeConfig.PreErrorResponderMiddleware);
                }

                // This is registered first so it can catch any errors and issue an appropriate response
                builder.UseResponderMiddleware();
                builder.UseDownstreamRouteFinderMiddleware();
                builder.UseDownstreamRequestInitialiser();
                builder.UseRequestIdMiddleware();
                builder.UseMiddleware<ClaimsToHeadersMiddleware>();
                builder.UseLoadBalancingMiddleware();
                builder.UseDownstreamUrlCreatorMiddleware();
                builder.UseOutputCacheMiddleware();
                // cors headers
                builder.UseMiddleware<CorsMiddleware>();
                builder.UseMiddleware<HttpRequesterMiddleware>();
            }).Wait();
        }
    }
}