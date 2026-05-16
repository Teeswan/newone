using EPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPMS.Infrastructure.Persistence.Configurations;

public class EmployeeKpiAssignmentConfiguration : IEntityTypeConfiguration<EmployeeKpiAssignment>
{
    public void Configure(EntityTypeBuilder<EmployeeKpiAssignment> builder)
    {
        builder.ToTable("EmployeeKpiAssignment");

        builder.HasKey(e => e.AssignmentId);

        builder.Property(e => e.KpiNameSnapshot)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.CategorySnapshot)
            .HasMaxLength(100);

        builder.Property(e => e.UnitSnapshot)
            .HasMaxLength(50);

        builder.Property(e => e.WeightPercent)
            .HasColumnType("decimal(5, 2)")
            .IsRequired();

        builder.Property(e => e.TargetValue)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(e => e.ActualValue)
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.KpiScore)
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.WeightedScore)
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Direction)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(e => new { e.EmployeeId, e.CycleId });

        builder.HasOne(e => e.Employee)
            .WithMany()
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Cycle)
            .WithMany()
            .HasForeignKey(e => e.CycleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Kpi)
            .WithMany()
            .HasForeignKey(e => e.KpiId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
