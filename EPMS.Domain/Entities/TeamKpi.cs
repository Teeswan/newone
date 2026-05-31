using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPMS.Domain.Entities
{
    [Table("TeamKPIs")]
    public class TeamKpi
    {
        [Key]
        public int TeamKpiId { get; set; }

        public int? TeamId { get; set; }

        public int? DeptKpiId { get; set; }

        public decimal? TeamTarget { get; set; }

        public decimal? Weight { get; set; }

        public bool? IsActive { get; set; }

        public decimal? ActualValue { get; set; }

        public int? VersionNo { get; set; }

        public decimal? SnapshotTarget { get; set; }

        public decimal? SnapshotWeight { get; set; }

        public string? ReasonForRevision { get; set; }

        public string? Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? ScorePercent { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? WeightedScore { get; private set; }

        // --- Navigation Properties ---

        [ForeignKey("TeamId")]
        public virtual Team? Team { get; set; }

        [ForeignKey("DeptKpiId")]
        public virtual DepartmentKpi? DepartmentKpi { get; set; }

        public virtual ICollection<EmployeeKpi> EmployeeKpis { get; set; } = new List<EmployeeKpi>();

        // Static factory method
        public static TeamKpi Create(int teamId, int deptKpiId, decimal teamTarget, decimal weight, decimal actualValue = 0)
        {
            return new TeamKpi
            {
                TeamId = teamId,
                DeptKpiId = deptKpiId,
                TeamTarget = teamTarget,
                Weight = weight,
                ActualValue = actualValue,
                IsActive = true,
                VersionNo = 1,
                Status = "Draft"
            };
        }

        public void Update(decimal teamTarget, decimal weight, decimal actualValue)
        {
            TeamTarget = teamTarget;
            Weight = weight;
            ActualValue = actualValue;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
