using DB_Manager.DBCntxt;
using DB_Manager.Models;
using API_Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DB_Manager.Managers
{
    public static class DBManagerRegistrationExtension
    {

        public static IServiceCollection AddDBManagers(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PgSQLContext>(opions =>
            {
                opions.UseNpgsql(connectionString)
                    .UseAsyncSeeding(SeedingAsync);
            });

            services.AddScoped<IProjectManager, ProjectManager>();

            return services;
        }

        public static async Task MigrateDatabase(this IHost host)
        {
            using IServiceScope scope = host.Services.CreateScope();
            using PgSQLContext context = scope.ServiceProvider.GetRequiredService<PgSQLContext>();

            IEnumerable<string> pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
            }
        }

        private static async Task SeedingAsync(DbContext context, bool _, CancellationToken cancellationToken)
        {
            (string, string)[] projects = [ ("Проект 1", "Описание проета 1"),
                                            ("Проект 2", "Описание проета 2"),
                                            ("Проект 3", "Описание проета 3") ];
            foreach (var project in projects)
            {
                await AddProject(project, context, cancellationToken);
            }
            await context.SaveChangesAsync(cancellationToken);
        }

        private static async Task AddProject((string name, string description) project, DbContext context, CancellationToken cancellationToken)
        {
            Project? dbProject = await context.Set<Project>().FirstOrDefaultAsync(p => p.Name == project.name, cancellationToken);
            if (dbProject is null)
            {
                context.Set<Project>().Add(new Project { Name = project.name, Description = project.description });
            }
        }
    }
}
