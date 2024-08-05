using HR.Context;
using Microsoft.EntityFrameworkCore;

namespace HR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();      
            builder.Services.AddDbContext<HrContext>(options => options.UseNpgsql());
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            /*builder.Services.AddMvc()
                .AddJsonOptions(
                opt => opt.JsonSerializerOptions.ReferenceHandler = Newtonsoft.Json.ReferenceLoopHandling.Ignore);*/

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Division}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
