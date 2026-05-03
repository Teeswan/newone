using EPMS.Shared.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Validation
{
    public class UpdatePipObjectiveValidator : AbstractValidator<UpdatePipObjectiveRequest>
    {
        public UpdatePipObjectiveValidator()
        {
            RuleFor(x => x.ObjectiveId).GreaterThan(0);

            RuleFor(x => x.ReviewComments)
                .MaximumLength(4000).When(x => x.ReviewComments != null);

            RuleFor(x => x.SuccessCriteria)
                .MaximumLength(2000).When(x => x.SuccessCriteria != null);
        }
    }
}
