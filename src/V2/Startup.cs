using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;

namespace V2
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthorization(config =>
                {
                    config.AddPolicy("can-update", p => p.RequireClaim("scopes", "api:update"));
                    config.AddPolicy("can-delete", p => p.RequireClaim("scopes", "api:delete"));
                })
                .AddScoped<IHttpContextAccessor, HttpContextAccessor>()
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(
                        "1.0",
                        new Info
                        {
                            Version = "1.0"

                        });

                    // configure authentication for Swagger schema -
                    // which endpoints to display in Swagger UI for specified auth token
                    var schema = new OAuth2Scheme
                    {
                        Type = "apiKey",
                        Description = "JWT token based authorization"
                    };
                    schema.Extensions.Add("in", "header");
                    schema.Extensions.Add("name", "Authorization");
                    options.AddSecurityDefinition("api_key", schema);

                    // filter non-authorized endpoints
                    options.DocumentFilter<SwaggerAuthorizationFilter>();
                });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app
                .UseJwtBearerAuthentication(new JwtBearerOptions
                {
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = false,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secretprivatekey42"))
                    }
                })
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseMvc()
                .UseSwagger(c => c.RouteTemplate = "swagger/{documentName}/schema.json")
                .UseSwaggerUi(c =>
                {
                    c.InjectOnCompleteJavaScript("/custom-swagger.js");
                    c.SwaggerEndpoint("/swagger/1.0/schema.json", "API version 1.0");
                });
        }
    }
}
