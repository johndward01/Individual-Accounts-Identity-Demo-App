using Individual_Accounts_Identity_Demo.IdentityPolicy;
using Individual_Accounts_Identity_Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Individual_Accounts_Identity_Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordPolicy>();
            services.AddTransient<IUserValidator<AppUser>, CustomUsernameEmailPolicy>();
            services.AddTransient<IProductRepository, ProductRepository>();

            services.AddScoped<IDbConnection>((s) =>
            {
                IDbConnection conn = new MySqlConnection(Configuration.GetConnectionString("bestbuy"));
                conn.Open();
                return conn;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<AppUser, IdentityRole>();

            //services.AddIdentity<AppUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ".AspNetCore.Identity.Application";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AspManager", policy =>
                {
                    policy.RequireRole("Manager");
                    policy.RequireClaim("Coding-Skill", "ASP.NET Core MVC");
                });
            });

            services.AddTransient<IAuthorizationHandler, AllowUsersHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowTom", policy =>
                {
                    policy.AddRequirements(new AllowUserPolicy("tom"));
                });
            });

            services.AddTransient<IAuthorizationHandler, AllowPrivateHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("PrivateAccess", policy =>
                {
                    policy.AddRequirements(new AllowPrivatePolicy());
                });
            });

            //services.AddAuthentication()
            //    .AddGoogle(opts =>
            //    {
            //        opts.ClientId = ""; // Empty
            //        opts.ClientSecret = ""; // Empty
            //        opts.SignInScheme = IdentityConstants.ExternalScheme;
            //    });

            services.AddControllersWithViews();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
