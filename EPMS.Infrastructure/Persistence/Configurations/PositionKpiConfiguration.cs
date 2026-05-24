using EPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPMS.Infrastructure.Persistence.Configurations;

public class PositionKpiConfiguration : IEntityTypeConfiguration<PositionKpi>
{
    public void Configure(EntityTypeBuilder<PositionKpi> builder)
    {
        builder.ToTable("PositionKPIs");

        builder.HasKey(k => k.PositionKpiId);
        builder.Property(k => k.PositionKpiId).HasColumnName("PositionKPIID");

        builder.Property(k => k.PositionId).HasColumnName("PositionID");
        builder.Property(k => k.KpiId).HasColumnName("KPI_ID");

        builder.Property(k => k.DefaultWeightPercent)
            .HasColumnType("decimal(5, 2)")
            .IsRequired();

        builder.Property(k => k.IsRequired)
            .HasDefaultValue(true);

        builder.HasOne(k => k.Position)
            .WithMany()
            .HasForeignKey(k => k.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.Kpi)
            .WithMany()
            .HasForeignKey(k => k.KpiId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
