using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EPMS.Application.Interfaces;
using EPMS.Domain.Entities;
using EPMS.Domain.Enums;
using EPMS.Domain.Interfaces;
using EPMS.Shared.Common;
using EPMS.Shared.Requests;
using MediatR;

namespace EPMS.Application.UseCases.Appraisal.Commands;

public record SubmitAppraisalBatchCommand : IRequest<Result<bool>>
{
    public List<CreateAppraisalResponseRequest> Responses { get; init; } = new();
}

public class SubmitAppraisalBatchCommandHandler : IRequestHandler<SubmitAppraisalBatchCommand, Result<bool>>
{
    private readonly IAppraisalResponseRepository _repository;
    private readonly IPerformanceEvaluationRepository _evaluationRepository;
    private readonly IMapper _mapper;

    public SubmitAppraisalBatchCommandHandler(
        IAppraisalResponseRepository repository,
        IPerformanceEvaluationRepository evaluationRepository,
        IMapper mapper)
    {
        _repository = repository;
        _evaluationRepository = evaluationRepository;
        _mapper = mapper;
    }

    public async Task<Result<bool>> Handle(SubmitAppraisalBatchCommand request, CancellationToken cancellationToken)
    {
        if (request.Responses == null || request.Responses.Count == 0)
        {
            return Result<bool>.Failure("No responses provided.");
        }

        foreach (var respRequest in request.Responses)
        {
            if (!respRequest.EvalId.HasValue)
            {
                return Result<bool>.Failure("Evaluation ID is required for each response.");
            }

            var evaluation = await _evaluationRepository.GetByIdAsync(respRequest.EvalId.Value);
            if (evaluation == null)
            {
                return Result<bool>.Failure($"Evaluation with ID {respRequest.EvalId} not found.");
            }

            // Validation logic based on RespondentRole
            if (Enum.TryParse<RespondentRole>(respRequest.RespondentRole, true, out var role))
            {
                if (role == RespondentRole.Self)
                {
                    if (respRequest.RespondentEmployeeId != evaluation.EmployeeId)
                    {
                        return Result<bool>.Failure("Self-assessment must be submitted by the employee being reviewed.");
                    }
                }
                else if (role == RespondentRole.Peer || role == RespondentRole.Manager || role == RespondentRole.Subordinate)
                {
                    if (respRequest.RespondentEmployeeId == evaluation.EmployeeId)
                    {
                        return Result<bool>.Failure($"{role} feedback cannot be submitted by the employee being reviewed.");
                    }
                }
            }
            else
            {
                return Result<bool>.Failure($"Invalid RespondentRole: {respRequest.RespondentRole}");
            }

            var entity = _mapper.Map<AppraisalResponse>(respRequest);
            await _repository.CreateAsync(entity);
        }

        return Result<bool>.Success(true);
    }
}
