using Microsoft.EntityFrameworkCore;
using DB_Manager.Managers;
using Serilog;
using Web_API.ExceptionHandler;
using FluentValidation;
using CacheRedis;

namespace Web_API;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        string connectionString = GetConnectionString(builder.Configuration);
        builder.Services.AddDBManagers(connectionString);

        string? redisConfiguration = builder.Configuration.GetConnectionString("RedisConnection");
        builder.Services.AddRedisCaching(redisConfiguration);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Scoped);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Configuration.AddJsonFile("serilogsettings.json");
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            //configuration.ReadFrom.Services(services);
            configuration.ReadFrom.Configuration(context.Configuration);
        });

        var app = builder.Build();

        app.UseExceptionHandler();

        await app.MigrateDatabase();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }

    private static string GetConnectionString(ConfigurationManager configuration)
    {
        string? result = configuration["PGSQLCONNECTIONSTRING"];
        result ??= configuration.GetConnectionString("PgSQLConnection");
        result ??= "Host=localhost;Port=5433;Database=usersdb;Username=postgres;Password=qwe";
        return result;
    }
}
