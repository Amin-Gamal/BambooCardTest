using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.BambooCardApi.Services;

using System.Text;

namespace Nop.Plugin.Misc.BambooCardApi.Infrastructure
{
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
            AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = "BambooCard",
                    ValidAudience = "BambooCard Api",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Defaults.SECURITY_KEY)), // Replace with your actual secret key
                };
            });

            services.AddScoped<IBambooCardOrderService, BambooCardOrderService>();

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<MvcNewtonsoftJsonOptions>(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            var rewriteOptions = new RewriteOptions().AddRewrite("api/token", "/token", true);
            app.UseRewriter(rewriteOptions);

            app.UseCors(x => x
                     .AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());

            app.MapWhen(context => context.Request.Path.StartsWithSegments(new PathString("/api")),
              a =>
              {
                  if (environment.IsDevelopment())
                  {
                      a.UseDeveloperExceptionPage();
                  }

                  a.Use(async (context, next) =>
                  {
                      // API Call
                      context.Request.EnableBuffering();
                      await next();
                  });

                  a.UseExceptionHandler(a => a.Run(async context =>
                  {
                      var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                      var exception = exceptionHandlerPathFeature.Error;
                      await context.Response.WriteAsJsonAsync(new { error = exception.Message });
                  }));

                  a.UseRouting();
                  a.UseAuthentication();
                  a.UseAuthorization();
                  a.UseEndpoints(endpoints =>
                  {
                      endpoints.MapControllers();
                  });
              }
            );
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1;
    }
}