using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPMS.Domain.Entities
{
    [Table("EmployeeKpi")]
    public class EmployeeKpi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeKpiId { get; set; }

        public int EmployeeId { get; set; }

        public int CycleId { get; set; }

        public int KpiId { get; set; }

        public int? PositionKpiId { get; set; } 

        public int? TeamKpiId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal? Weight { get; set; }

        public decimal? TargetValue { get; set; }

        public decimal? ActualValue { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? Score { get; private set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? WeightedScore { get; private set; }

        // --- Navigation Properties ---

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [ForeignKey("CycleId")]
        public virtual AppraisalCycle? AppraisalCycle { get; set; }

        [ForeignKey("KpiId")]
        public virtual Kpi? Kpi { get; set; }

        [ForeignKey("PositionKpiId")]
        public virtual PositionKpi? PositionKpi { get; set; }

        [ForeignKey("TeamKpiId")]
        public virtual TeamKpi? TeamKpi { get; set; } 
    }
}
