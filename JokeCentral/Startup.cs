using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryExtensions.HttpClient;

namespace JokeCentral
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {   
            
            // If your application is .NET Standard 2.1 or above, and you are using an insecure (http) endpoint,
            // the following switch must be set before adding Exporter.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            var resource = ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector();
            
            services.AddControllersWithViews();
            services.AddOpenTelemetryTracing(builder =>
                builder
                    .SetResourceBuilder(resource)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation(options =>
                        options.Enrich = ActivityNameEnricher.Enrich)
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(System.Environment.GetEnvironmentVariable(
                            "OTEL_EXPORTER_OTLP_ENDPOINT") ?? "http://localhost:4317");
                    })
                    .AddConsoleExporter()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Api}/{action=Index}/{id?}");
            });
        }
    }
}
