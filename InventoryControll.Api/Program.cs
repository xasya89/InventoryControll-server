
using Calabonga.AspNetCore.AppDefinitions;
using InventoryControll.Api.filters;
using InventoryControll.DataDb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryControll.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddDbContext<ShopContext>(cfg=>cfg.UseMySql("server=localhost;database=shop;uid=root;pwd=kt38hmapq", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.30-mysql")),
                ServiceLifetime.Scoped);
            builder.Services.AddAutoMapper(typeof(Program).Assembly);
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.AddDefinitions(typeof(Program));


            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseDefinitions();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", async (HttpContext httpContext, [FromServices] ShopContext context) =>
            {
                return await context.Goods.AsNoTracking().Take(50).ToListAsync();
            }).AddEndpointFilter<ChangeTenantFilter>()
            .WithName("GetWeatherForecast")
            .WithOpenApi();

            app.Run();
        }
    }
}
