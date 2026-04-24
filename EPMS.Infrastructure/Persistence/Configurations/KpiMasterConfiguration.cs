using EPMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EPMS.Infrastructure.Persistence.Configurations;

public class KpiMasterConfiguration : IEntityTypeConfiguration<KpiMaster>
{
    public void Configure(EntityTypeBuilder<KpiMaster> builder)
    {
        builder.ToTable("KPIs");

        builder.HasKey(k => k.KpiId);

        builder.Property(k => k.KpiName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(k => k.Category)
            .HasMaxLength(100);

        builder.Property(k => k.Unit)
            .HasMaxLength(50);

        builder.Property(k => k.WeightPercent)
            .HasColumnType("decimal(5, 2)")
            .IsRequired();

        builder.Property(k => k.TargetValue)
            .HasColumnType("decimal(18, 2)");

        builder.Property(k => k.PriorityLevel)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(k => k.Direction)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(k => k.IsActive)
            .HasDefaultValue(true);

        builder.HasQueryFilter(k => k.IsActive);

        builder.HasOne(k => k.Position)
            .WithMany()
            .HasForeignKey(k => k.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
