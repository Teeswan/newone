using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure.Repositories;

public class SystemSettingRepository : BaseRepository<SystemSetting, int>, ISystemSettingRepository
{
    public SystemSettingRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }

    public async Task SetValueAsync(string key, string value)
    {
        // Find the record by Key
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);

        if (setting != null)
        {
            // Update existing record
            setting.Value = value;
            setting.UpdatedAt = DateTime.UtcNow;
            _context.SystemSettings.Update(setting); // Explicitly mark as updated
        }
        else
        {
            // Create new only if it truly doesn't exist
            var newSetting = new SystemSetting
            {
                Key = key,
                Value = value,
                Description = "Default password",
                UpdatedAt = DateTime.UtcNow
            };
            _context.SystemSettings.Add(newSetting);
        }

        await _context.SaveChangesAsync();
    }
}
