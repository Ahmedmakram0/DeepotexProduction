using CloudinaryDotNet;
using Deepotex.core.Models;
using Deepotex.core.Repositories;
using Deepotex.EF;
using Deepotex.EF.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Deepotex_V2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    b=>b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            builder.Services.AddScoped<IProductRepo, ProductRepo>();

            builder.Services.AddSingleton<PhotoServices>(provider =>
                new PhotoServices(
                    builder.Configuration["Cloudinary:CloudName"],
                    builder.Configuration["Cloudinary:ApiKey"],
                    builder.Configuration["Cloudinary:ApiSecret"]
                )
            );

            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Account/Login";
                });


            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
