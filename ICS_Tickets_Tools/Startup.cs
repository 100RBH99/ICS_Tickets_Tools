using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICS_Tickets_Tools.DB_Context;
using ICS_Tickets_Tools.Repositories.Interfaces;
using ICS_Tickets_Tools.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ICS_Tickets_Tools.Services;
using ICS_Tickets_Tools.Models;
using ICS_Tickets_Tools.Hubs;

namespace ICS_Tickets_Tools
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
            services.AddControllersWithViews();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITicketsRepository, TicketsRepository>();
            services.AddScoped<IAssignTicketsRepository, AssignTicketsRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddHttpClient();
            services.AddScoped<GoogleReCaptchaService>();

            services.AddControllers().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddDbContext<TicketsDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews();
            services.AddSignalR(); // Register SignalR
            services.AddMemoryCache();

            services.AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<TicketsDBContext>()
              .AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
               // app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

                app.UseHsts();
            }

            app.UseStatusCodePages(context =>
            {
                var response = context.HttpContext.Response;

                if (response.StatusCode == 404)
                    response.Redirect("/Home/StatusCode?code=404");
                else if (response.StatusCode == 403)
                    response.Redirect("/Home/StatusCode?code=403");

                return Task.CompletedTask;
            });


            app.UseHttpsRedirection();
            app.UseStaticFiles();         
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
            {
                //endpoints.MapDefaultControllerRoute();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");            
            });
        }
    }
}
