using EPMS.Shared.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Validation
{
    public class CreatePipObjectiveValidator : AbstractValidator<CreatePipObjectiveRequest>
    {
        public CreatePipObjectiveValidator()
        {
            RuleFor(x => x.ObjectiveDescription)
                .NotEmpty().WithMessage("Objective description is required.")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

            RuleFor(x => x.SuccessCriteria)
                .MaximumLength(2000).WithMessage("Success criteria cannot exceed 2000 characters.")
                .When(x => x.SuccessCriteria != null);
        }
    }
}
