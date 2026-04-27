using EPMS.Shared.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMS.Shared.Validation
{
    public class CreatePipMeetingValidator : AbstractValidator<CreatePipMeetingRequest>
    {
        private static readonly string[] AllowedStatuses =
            ["On Track", "At Risk", "Behind Schedule", "Completed"];

        public CreatePipMeetingValidator()
        {
            RuleFor(x => x.PipId)
                .GreaterThan(0).WithMessage("PIP reference is required.");

            RuleFor(x => x.MeetingDate)
                .NotEmpty().WithMessage("Meeting date is required.");

            RuleFor(x => x.ProgressStatus)
                .NotEmpty().WithMessage("Progress status is required.")
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}");

            RuleFor(x => x.DiscussionPoints)
                .MaximumLength(4000).When(x => x.DiscussionPoints != null);

            RuleFor(x => x.NextSteps)
                .MaximumLength(4000).When(x => x.NextSteps != null);
        }
    }
}
