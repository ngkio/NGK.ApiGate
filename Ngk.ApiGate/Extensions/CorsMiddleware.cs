using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Ocelot.Logging;
using Ocelot.Middleware;

namespace Ngk.ApiGate.Extensions
{
    public class CorsMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;

        public CorsMiddleware(OcelotRequestDelegate next, IOcelotLoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<CorsMiddleware>())
        {
            _next = next;
        }

        public async Task Invoke(DownstreamContext context)
        {
            if (!context.HttpContext.Request.Method.Equals("OPTIONS",
                StringComparison.InvariantCultureIgnoreCase))
            {
                await _next(context);
                return;
            }

            var headers = new List<Header>
            {
                new Header(HeaderNames.AccessControlAllowMethods,
                    context.HttpContext.Request.Headers["Access-Control-Request-Method"]),
                new Header(HeaderNames.AccessControlAllowCredentials, new[] {"true"}),
                new Header(HeaderNames.AccessControlAllowOrigin,
                    new[] {context.HttpContext.Request.Headers["Origin"].FirstOrDefault()}),
                new Header(HeaderNames.AccessControlAllowHeaders,
                    context.HttpContext.Request.Headers["Access-Control-Request-Headers"])
            };
            context.DownstreamResponse = new DownstreamResponse(new StreamContent(new MemoryStream()),
                HttpStatusCode.NoContent, headers, "");
        }
    }
}