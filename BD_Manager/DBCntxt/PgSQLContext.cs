using DB_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace DB_Manager.DBCntxt;
public class PgSQLContext : DbContext
{
    public DbSet<Project> Projects { get; set; }

    public PgSQLContext() : base()
    {
        //Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=usersdb;Username=postgres;Password=qwe")
            .UseAsyncSeeding((async (context, _, cancellationToken) =>
            {
                var addProject = async (string name, string description) =>
                {
                    Project? project = await context.Set<Project>().FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
                    if (project is null)
                    {
                        context.Set<Project>().Add(new Project { Name = name, Description = description });
                        await context.SaveChangesAsync(cancellationToken);
                    }
                };

                await addProject("Проект 1", "Описание проета 1");
                await addProject("Проект 2", "Описание проета 2");
                await addProject("Проект 3", "Описание проета 3");
            }));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
    }
}
