using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;

namespace SwaggerSecurityTrimming
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = false,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        // ##############################################################
                        // WARNING: this is just for demonstration purpose
                        // the signing key should be stored securely elsewhere
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secretprivatekey42"))
                        // ##############################################################
                    };
                });
            services
                .AddAuthorization(config =>
                {
                    config.AddPolicy("can-update", p => p.RequireClaim("scopes", "api:update"));
                    config.AddPolicy("can-delete", p => p.RequireClaim("scopes", "api:delete"));
                })
                .AddHttpContextAccessor();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API",
                    Version = "v1",
                }); 
                options.DocumentFilter<SecurityTrimming>();
                options.AddSecurityDefinition("BearerDefinition", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "BearerDefinition"
                            }
                        },
                        new List<string>()
                    }});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseSwagger(c => c.RouteTemplate = "openaapi/{documentName}/schema.json")
                .UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "openaapi";
                    c.SwaggerEndpoint($"/openaapi/v1/schema.json", $"API v1");
                    c.InjectJavascript("/swagger-custom.js");
                })
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
