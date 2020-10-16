using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ngk.IdentityService.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ngk.IdentityService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = env;
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var isBuilder = services.AddIdentityServer();

            if (Environment.IsProduction())
            {
                isBuilder.AddSigningCredential(new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(),
                        Configuration["Certificates:Path"]),
                    Configuration["Certificates:Password"]));
            }
            else
            {
                isBuilder.AddDeveloperSigningCredential(); //测试的时候可使用临时的证书
            }

            isBuilder.AddInMemoryClients(OAuth2Config.GetClients(Configuration))
                .AddInMemoryApiResources(OAuth2Config.GetApiResources())
                .AddInMemoryIdentityResources(OAuth2Config.GetIdentityResources())
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>() //User验证接口
                .AddProfileService<ProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseIdentityServer();
        }
    }
}