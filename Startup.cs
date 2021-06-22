using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC_BugTracker.Data;
using MVC_BugTracker.Models;
using MVC_BugTracker.Services;
using MVC_BugTracker.Services.Factories;
using MVC_BugTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MVC_BugTracker
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
            // [DEFAULT] services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));


            //[DEVELOPMENT] services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseNpgsql(
            //        Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Connection.GetConnectionString(Configuration)));

            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<BTUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddClaimsPrincipalFactory<BTUserClaimsPrincipalFactory>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            //services.AddControllersWithViews();
            services.AddMvc();

            services.AddScoped<IBTRolesService, BTRolesService>();
            services.AddScoped<IBTProjectService, BTProjectService>();
            services.AddScoped<IBTTicketService, BTTicketService>();
            services.AddScoped<IBTCompanyInfoService, BTCompanyInfoService>();
            services.AddScoped<IBTHistoryService, BTHistoryService>();
            services.AddScoped<IBTInviteService, BTInviteService>();
            services.AddScoped<IBTNotificationService, BTNotificationService>();
            services.AddScoped<IBTFileService, BTFileService>();

            // Data Utility
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseNpgsql(DataUtility.GetConnectionString(Configuration)));

            // Add service using an existing interface
            services.AddScoped<IEmailSender, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Landing}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
