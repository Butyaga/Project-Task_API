using DB_Manager.DBCntxt;
using DB_Manager.Managers;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Web_API;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        string connectionString = GetConnectionString(builder.Configuration);
        builder.Services.AddDBManagers(connectionString);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

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
