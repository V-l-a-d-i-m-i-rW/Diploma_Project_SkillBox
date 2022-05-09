using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkillProfi_WebSite.Classes;
using SkillProfi_WebSite.Interfaces;
using SkillProfi_WebSite.UserAuthorization;
using System;

namespace SkillProfi_WebSite
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
            //������ ����������� � �� � ��������������
            string connection = Configuration.GetConnectionString("UserConnection");
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
            //TODO: ��������
            services.AddTransient<ISkillProfi, WebAPISkillProfi>();
            services.AddControllersWithViews();  // ��������� ������� MVC

            #region �����������
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6; // ����������� ���������� ������ � ������
                options.Lockout.MaxFailedAccessAttempts = 3; // ���������� ������� �� ����������
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.AllowedForNewUsers = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // ������������ Cookie � ����� ������������� �� ��� �������� �����������
                options.Cookie.HttpOnly = true;
                //options.Cookie.Expiration = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.SlidingExpiration = true;
            });

            #endregion
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
                app.UseExceptionHandler("/BidData/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();    // ����������� ��������������
            app.UseAuthorization();     // ����������� �����������

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=BidData}/{action=Index}/{id?}");
            });
        }
    }
}
