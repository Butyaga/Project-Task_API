using DB_Manager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB_Manager.DBCntxt;
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("project").HasKey(p => p.Id).HasName("PK_Project");
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Name).HasColumnName("name").IsRequired(true);
        builder.Property(p => p.Description).HasColumnName("description").IsRequired(false);
        builder.Property(p => p.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("now()").IsRequired(true);
        builder.Ignore(project => project.Tasks);
    }
}
