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
            .HasColumnType("decimal(18, 4)")
            .IsRequired();

        builder.Property(e => e.ActualValue)
            .HasColumnType("decimal(18, 4)");

        builder.Property(e => e.KpiScore)
            .HasColumnType("decimal(18, 4)")
            .HasComputedColumnSql(@"
                CASE 
                    WHEN ActualValue IS NULL OR TargetValue = 0 THEN NULL
                    WHEN Direction = 1 THEN 
                        ROUND((ActualValue / TargetValue) * 100, 4)
                    ELSE 
                        CASE 
                            WHEN ActualValue = 0 THEN 100
                            WHEN (TargetValue / ActualValue) * 100 > 100 THEN 100
                            ELSE ROUND((TargetValue / ActualValue) * 100, 4)
                        END
                END", stored: true);

        builder.Property(e => e.WeightedScore)
            .HasColumnType("decimal(18, 4)")
            .HasComputedColumnSql(@"
                CASE 
                    WHEN KpiScore IS NULL THEN NULL
                    ELSE ROUND(KpiScore * (WeightPercent / 100), 4)
                END", stored: true);

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
        
        builder.Property(e => e.KpiId).HasColumnName("KpiID");
    }
}
