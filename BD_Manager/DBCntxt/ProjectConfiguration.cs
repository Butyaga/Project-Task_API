using DB_Manager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB_Manager.DBCntxt;
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Project").HasKey(p => p.Id).HasName("PK_Project");

        builder.Property(p => p.Id).HasColumnName("Id");
        builder.Property(p => p.Name).HasColumnName("Name").HasMaxLength(100).IsRequired(true);
        builder.Property(p => p.Description).HasColumnName("Description").HasMaxLength(200).IsRequired(false);
        builder.Property(p => p.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("now()").IsRequired(true);
    }
}
