using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdeasTracker.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Okta.AspNetCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Security.Claims;
using IdeasTracker.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using IdeasTracker.Attributes;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Business.Uow;

namespace IdeasTracker
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
            var oktaMvcOptions = new OktaMvcOptions();
            Configuration.GetSection("Okta").Bind(oktaMvcOptions);
            oktaMvcOptions.Scope = new List<string> { "openid", "profile", "email", "groups" };
            oktaMvcOptions.GetClaimsFromUserInfoEndpoint = true;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OktaDefaults.MvcAuthenticationScheme;
            })
            .AddCookie()
              /*.AddOpenIdConnect(option =>
              {
                  option.ClientId = oktaMvcOptions.ClientId;
                  option.ClientSecret = oktaMvcOptions.ClientSecret;
                  option.Authority = $"{oktaMvcOptions.OktaDomain}/oauth2/default";
                  option.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                  option.GetClaimsFromUserInfoEndpoint = true;
                  option.SaveTokens = true;
                  option.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      RoleClaimType = ClaimTypes.Role
                  };
                  option.ClaimActions.Add(new CustomJsonClaimAction(ClaimTypes.Role, ClaimTypes.Role, (x) => string.Join(",", x["groups"].Values<string>())));
              });*/
              .AddOktaMvc(oktaMvcOptions);
            services.Configure<CookiePolicyOptions>(options =>
      {
          // This lambda determines whether user consent for non-essential cookies is needed for a given request.
          options.CheckConsentNeeded = context => true;
          options.MinimumSameSitePolicy = SameSiteMode.None;
      });
            services.AddTransient<IClaimsTransformation, ClaimsTransformation>();
            services.AddTransient<IRoleUow, RolesUow>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=IdeasTracker.db"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
      {
          routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
      });
        }
    }
}
