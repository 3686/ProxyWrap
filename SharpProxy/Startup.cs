using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharpReverseProxy;

namespace SharpProxy
{
    public class Startup
    {
        private readonly Uri _realUri;
        private readonly string _match;

        public Startup(IConfiguration configuration)
        {
            _realUri = new Uri(configuration["real.url"]);
            _match = configuration["match"];
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Add(ServiceDescriptor.Singleton<ILogger>(new ConsoleLogger()));
            services.Add(ServiceDescriptor.Singleton(new History()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.GetUri().ToString().Contains("__admin/history"))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.ContentType = "application/json";
                    var history = (History)app.ApplicationServices.GetService(typeof(History));
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(history.Get()));
                    return;
                }

                await next.Invoke();
            });
            app.UseProxy(
                new List<ProxyRule> {
                    new ProxyRule {
                        Matcher = uri =>
                        {
                            return Regex.IsMatch(uri.AbsoluteUri, _match, RegexOptions.Compiled);
                        },
                        Modifier = (req, user) =>
                        {
                            var uriBuilder = new UriBuilder(req.RequestUri.ToString())
                            {
                                Host = _realUri.Host,
                                Scheme = _realUri.Scheme,
                                Port = _realUri.Port
                            };
                            req.RequestUri = uriBuilder.Uri;
                            req.Headers.Add("X-SharpProxy-Identifier", new []{ Guid.NewGuid().ToString() });
                        }
                    }
                },
                r =>
                {
                    var history = (History) app.ApplicationServices.GetService(typeof(History));
                    history.Add(new HistoryItem
                    {
                        ProxyResult = r,
                        When = DateTime.UtcNow
                    });
                });
        }
    }
}
