using DB_Manager.DBCntxt;
using DB_Manager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API_Abstract.Managers;

namespace DB_Manager.Managers;
public static class DBManagersRegistrationExtensions
{

    public static IServiceCollection AddDBManagers(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PgSQLContext>(options =>
        {
            options.UseNpgsql(connectionString)
                .UseAsyncSeeding(SeedingAsync);
        });

        services.AddScoped<IProjectManager, ProjectManager>();
        services.AddScoped<ITaskManager, TaskManager>();

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
        int projectCount = 7;
        int taskCountPerProject = 3;

        await AddProjects(projectCount, context, cancellationToken);
        await AddTasks(taskCountPerProject, context, cancellationToken);
    }

    private static async Task AddProjects(int projectCount, DbContext context, CancellationToken cancellationToken)
    {
        string templateProjectName = "Проект {0}";
        string templateProjectDescription = "Описание проета {0}";

        for (int i = 1; i <= projectCount; i++)
        {
            string projectName = string.Format(templateProjectName, i.ToString());
            string projectDescription = string.Format(templateProjectDescription, i.ToString());

            Project project = new() { Name = projectName, Description = projectDescription };
            context.Set<Project>().Add(project);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task AddTasks(int taskCountPerProject, DbContext context, CancellationToken cancellationToken)
    {
        string templateTaskTitle = "Задача {0}-{1}";
        string templateTaskDescription = "Описание задачи {0}-{1}";

        foreach (Project project in context.Set<Project>().Local)
        {
            for (int indexTask = 1; indexTask <= taskCountPerProject; indexTask++)
            {
                string taskTitle = string.Format(templateTaskTitle, project.Id, indexTask.ToString());
                string taskDescription = string.Format(templateTaskDescription, project.Id, indexTask.ToString());
                bool taskIsCompleted = (indexTask % 2) == 0;

                ProjectTask task = new()
                {
                    Title = taskTitle,
                    Description = taskDescription,
                    IsCompleted = taskIsCompleted,
                    ProjectId = project.Id
                };
                context.Set<ProjectTask>().Add(task);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
