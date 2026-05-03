using EPMS.Shared.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Validation
{
    public class CreatePipPlanValidator : AbstractValidator<CreatePipPlanRequest>
    {
        public CreatePipPlanValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Employee is required.");

            RuleFor(x => x.ManagerId)
                .GreaterThan(0).WithMessage("Manager is required.")
                .NotEqual(x => x.EmployeeId).WithMessage("Manager cannot be the same as employee.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.")
                .Must((req, endDate) => endDate <= req.StartDate.AddMonths(6))
                .WithMessage("PIP duration cannot exceed 6 months.");

            RuleFor(x => x.OverallGoal)
                .MaximumLength(2000).WithMessage("Overall goal cannot exceed 2000 characters.")
                .When(x => x.OverallGoal != null);

            RuleFor(x => x.Objectives)
                .NotEmpty().WithMessage("At least one objective is required.")
                .Must(o => o.Count <= 10).WithMessage("Cannot exceed 10 objectives per PIP.");

            RuleForEach(x => x.Objectives)
                .SetValidator(new CreatePipObjectiveValidator());
        }
    }
}
