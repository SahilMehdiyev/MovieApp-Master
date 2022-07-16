using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MovieApp.Web.Data;
using MovieApp.Web.Entity;
using MovieApp.Web.Identity;
using MovieApp.Web.Infrastructure;
using MovieApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDbContext<MovieContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MsSQLConnection")));

           

            services.AddDbContext<AppIdentityDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("MsSQLConnection")));

            services.AddIdentity<AppIdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            //services.ConfigureApplicationCookie(opt => opt.LoginPath = "Security/Login");

            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequireDigit = true;
            //    options.Password.RequireLowercase = true;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequireNonAlphanumeric = true;
            //    options.Password.RequireUppercase = true;

            //    options.Lockout.MaxFailedAccessAttempts = 5;
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //    options.Lockout.AllowedForNewUsers = true;

            //    options.User.RequireUniqueEmail = true;

            //    options.SignIn.RequireConfirmedEmail = true;
            //    options.SignIn.RequireConfirmedPhoneNumber = false;
            //});

            services.AddTransient<IPasswordValidator<AppIdentityUser>, CustomPasswordValidator>();
            services.ConfigureApplicationCookie(options =>
            {

                options.LoginPath = "/Security/Login";
                options.LogoutPath = "/Security/Logout";
                options.AccessDeniedPath = "/Security/AccessDenied";
                options.SlidingExpiration = true;
            }); // sistem yeniden girme kapalı
                                                  //    options.Cookie = new CookieBuilder
                                                  //    {
                                                  //        HttpOnly = true,
                                                  //        Name = ".AspNetCoreDemo.Access.Cookie",
                                                  //        Path = "/",
                                                  //        SameSite = SameSiteMode.Lax,
                                                  //        SecurePolicy = CookieSecurePolicy.SameAsRequest
                                                  //    };



                //});

                services.AddControllersWithViews()
                .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled=true);

            services.Configure<SMTPConfigModel>(Configuration.GetSection("SMTPConfig"));
        
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                DataSeeding.Seed(app);
            }

            app.UseStaticFiles(); // wwwroot

            app.UseRouting();
            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });

        }
    }
}
