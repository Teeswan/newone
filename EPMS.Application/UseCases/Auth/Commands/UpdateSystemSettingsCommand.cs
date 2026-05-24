using MediatR;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EPMS.Application.UseCases.Auth.Commands;

public class UpdateSystemSettingsCommand : IRequest<bool>
{
    public string NewDefaultPassword { get; set; } = string.Empty;
}

public class UpdateSystemSettingsCommandHandler : IRequestHandler<UpdateSystemSettingsCommand, bool>
{
    private readonly ISystemSettingRepository _systemSettingRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UpdateSystemSettingsCommandHandler> _logger;

    public UpdateSystemSettingsCommandHandler(
        ISystemSettingRepository systemSettingRepository,
        IPasswordHasher passwordHasher,
        ILogger<UpdateSystemSettingsCommandHandler> logger)
    {
        _systemSettingRepository = systemSettingRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateSystemSettingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var hashedPassword = _passwordHasher.HashPassword(request.NewDefaultPassword);

            // Ensure this operation is awaited and check for potential exceptions
            await _systemSettingRepository.SetValueAsync("DefaultPassword", hashedPassword);

            _logger.LogInformation("System settings updated: DefaultPassword changed");
            return true;
        }
        catch (Exception ex)
        {
            // Log the actual error so you can see it in the terminal
            _logger.LogError(ex, "Failed to update DefaultPassword in repository.");

            // Return false so the API returns a 400 Bad Request to the UI
            return false;
        }
    }
}
