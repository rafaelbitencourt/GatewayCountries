using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using HotChocolate.AspNetCore.Subscriptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace GatewayCountries
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://securetoken.google.com/customcountries-c4790";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/customcountries-c4790",
                        ValidateAudience = true,
                        ValidAudience = "customcountries-c4790",
                        ValidateLifetime = true
                    };
                });

            services.AddAuthorization();

            services.AddHttpClient("countries", (sp, client) =>
            {
                client.BaseAddress = new Uri("https://countries-274616.ew.r.appspot.com");
            });
            services.AddHttpClient("customcountries", (sp, client) =>
            {
                client.BaseAddress = new Uri("https://customcountries.herokuapp.com/graphql");
            });

            services.AddGraphQLSubscriptions();

            services
                .AddRouting()
                .AddGraphQLServer()
                .AddRemoteSchema("countries")
                .AddRemoteSchema("customcountries");
                //.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UsePlayground(new PlaygroundOptions
            {
                QueryPath = "/graphql",
                Path = "/playground"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapGraphQL());
        }
    }
}
