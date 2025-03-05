using DB_Manager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DB_Manager.DBCntxt
{
    class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.ToTable("Task").HasKey(t => t.Id).HasName("PK_Task");

            builder.Property(t => t.Id).HasColumnName("Id");
            builder.Property(t => t.Title).HasColumnName("Name").HasMaxLength(100).IsRequired(true);
            builder.Property(t => t.Description).HasColumnName("Description").HasMaxLength(200).IsRequired(false);
            builder.Property(t => t.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("now()").IsRequired(true);
            builder.Property(t => t.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("now()").IsRequired(true);
            builder.Property(t => t.ProjectId).HasColumnName("ProjectId").IsRequired(true);

            builder.HasOne(t => t.Project).WithMany(p => p.Tasks).HasForeignKey(t => t.ProjectId).HasConstraintName("FK_Project_ProjectId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(t => new { t.ProjectId, t.IsCompleted }).HasDatabaseName("Idx_Task_ProjectId_IsCompleted");
        }
    }
}
