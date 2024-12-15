using DotaRPG.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace DotaRPG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotaRPG API", Version = "v1" });
            });

            var app = builder.Build();

			// «аполн€ем базу данными, если она пуста€
			using (var scope = app.Services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
				DataInitializer.Initialize(context);
			}

			if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    //c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotaRPG API v1");
                    c.RoutePrefix = "swagger";
				});

			}

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
