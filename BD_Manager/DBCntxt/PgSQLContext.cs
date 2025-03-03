using DB_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace DB_Manager.DBCntxt;
public class PgSQLContext(DbContextOptions<PgSQLContext> options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
    }
}
