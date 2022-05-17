using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using net_verifier.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net_verifier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var issuer_key = new JsonWebKey(Configuration["mmlab.edu.gr"]);
            var tokenValidationParameters = new  TokenValidationParameters
            {

                ValidateIssuer = false,
                ValidateAudience = true,
                ValidAudience = Configuration["aud"],
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                ValidateTokenReplay = false,
                IssuerSigningKey = issuer_key, //https://stackoverflow.com/questions/59974471/does-asp-net-core-jwt-authentication-support-multiple-symmetric-signing-keys
                RequireSignedTokens = true
                /*
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                */

                //ValidAudience = "fotiou"
            };
            services.AddControllersWithViews();
            services.AddAuthentication()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                })
                .AddJwtBearer("DPoP",options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                    options.Events = new JwtBearerEvents
                    {
                        // ...
                        OnMessageReceived = context =>
                        {
                            string authorization = context.Request.Headers["Authorization"];

                            // If no authorization header found, nothing to process further
                            if (string.IsNullOrEmpty(authorization))
                            {
                                context.NoResult();
                                return Task.CompletedTask;
                            }

                            if (authorization.StartsWith("DPoP ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = authorization.Substring("DPoP ".Length).Trim();
                            }

                            // If no token found, no further work possible
                            if (string.IsNullOrEmpty(context.Token))
                            {
                                context.NoResult();
                                return Task.CompletedTask;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("VCAuthorization", policy => {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new VCAuthorizationRequirement());

                 });
                options.AddPolicy("DPoPAuthorization", policy => {
                    policy.AuthenticationSchemes.Add("DPoP");
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new VCAuthorizationRequirement());
                    policy.Requirements.Add(new DPoPAuthorizationRequirement());

                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "jwt-vc",
                    pattern: "secure/jwt-vc",
                    defaults: new { controller = "Secure", action = "JWT_VC" });
                endpoints.MapControllerRoute(
                    name: "jwt-vc-dpop",
                    pattern: "secure/jwt-vc-dpop",
                    defaults: new { controller = "Secure", action = "JWT_VC_DPoP" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
