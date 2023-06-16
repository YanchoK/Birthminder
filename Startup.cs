using Birthminder.Controllers;
using Birthminder.Data;
using Birthminder.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Birthminder
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
            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());


            services.AddHangfireServer();
            services.AddScoped<ISendEmails, SendEmails>();

            







            var connection = Configuration.GetConnectionString("birthminderDB");
            services.AddDbContext<birthminderContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<User, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<birthminderContext>();
            services.ConfigureApplicationCookie(options => {
                options.AccessDeniedPath = new PathString("/UserRoles/AccessDenied");
                options.LoginPath = new PathString("/UserRoles/AccessDenied");
                options.LogoutPath = new PathString("/UserRoles/AccessDenied");
            });
            services.AddControllersWithViews();



            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Or you can also register as follows

            services.AddHttpContextAccessor();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient, IServiceProvider serviceProvider)
        {
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHangfireDashboard();

            recurringJobManager.AddOrUpdate(
               "Send Emails",
               () => serviceProvider.GetService<ISendEmails>().checkUsersDates(),
                "0 0 7 ? * *"//"* * * * *" 
               );
        }
    }
}
