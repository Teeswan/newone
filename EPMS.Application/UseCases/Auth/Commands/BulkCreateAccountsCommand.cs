using MediatR;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EPMS.Application.UseCases.Auth.Commands;

public class BulkCreateAccountsCommand : IRequest<BulkCreateAccountsResponse>
{
}

public record BulkCreateAccountsResponse(int CreatedCount, int SkippedCount, string Message);

public class BulkCreateAccountsCommandHandler : IRequestHandler<BulkCreateAccountsCommand, BulkCreateAccountsResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAccountInitializationService _accountInitializationService;
    private readonly ILogger<BulkCreateAccountsCommandHandler> _logger;

    public BulkCreateAccountsCommandHandler(
        IEmployeeRepository employeeRepository,
        IAccountInitializationService accountInitializationService,
        ILogger<BulkCreateAccountsCommandHandler> logger)
    {
        _employeeRepository = employeeRepository;
        _accountInitializationService = accountInitializationService;
        _logger = logger;
    }

    public async Task<BulkCreateAccountsResponse> Handle(BulkCreateAccountsCommand request, CancellationToken cancellationToken)
    {
        var allEmployees = await _employeeRepository.GetAllAsync();
        int createdCount = 0;
        int skippedCount = 0;

        foreach (var employee in allEmployees)
        {
            if (string.IsNullOrEmpty(employee.Email))
            {
                skippedCount++;
                continue;
            }

            if (!string.IsNullOrEmpty(employee.PasswordHash))
            {
                skippedCount++;
                continue;
            }

            await _accountInitializationService.InitializeAccountAsync(employee);
            createdCount++;
        }

        _logger.LogInformation("Bulk account creation completed. Created {Created}, Skipped {Skipped}", createdCount, skippedCount);

        return new BulkCreateAccountsResponse(
            createdCount,
            skippedCount,
            $"Bulk account creation completed. Created {createdCount}, Skipped {skippedCount}."
        );
    }
}
