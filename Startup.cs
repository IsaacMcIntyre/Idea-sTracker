using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Okta.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using IdeasTracker.Attributes;
using IdeasTracker.Business.Uows.Interfaces;
using IdeasTracker.Business.Uow;
using IdeasTracker.Database.Context;
using IdeasTracker.Business.Converters.Interfaces;
using IdeasTracker.Business.Converters;
using IdeasTracker.Business.Uows;
using IdeasTracker.Business.Email;
using IdeasTracker.Business.Email.Interfaces;

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
              .AddOktaMvc(oktaMvcOptions);
            services.Configure<CookiePolicyOptions>(options =>
      {
          // This lambda determines whether user consent for non-essential cookies is needed for a given request.
          options.CheckConsentNeeded = context => true;
          options.MinimumSameSitePolicy = SameSiteMode.None;
      });
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=IdeasTracker.db"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<IClaimsTransformation, ClaimsTransformation>();
            services.AddTransient<IUserUow, UserUow>();
            services.AddTransient<IBackLogUow, BackLogUow>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IUserToUserModelConverter, UserToUserModelConverter>();
            services.AddTransient<IBacklogToAdoptRequestModelConverter, BacklogToAdoptRequestModelCoverter>();
            services.AddTransient<IBacklogToBackLogModelConverter, BacklogToBackLogModelConverter>();
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
