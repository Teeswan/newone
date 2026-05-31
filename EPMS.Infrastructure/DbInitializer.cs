using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EPMS.Infrastructure.Contexts;
using EPMS.Infrastructure.Repositories;
using EPMS.Infrastructure.Services;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EPMS.Infrastructure;

public static class DbInitializer
{
    public static async Task InitializeDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<AppDbContext>();
        
        // Ensure tables are created before running stored procedures
        dbContext.Database.EnsureCreated();

        var configuration = services.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        var sqlScriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "EPMS.Infrastructure", "SqlScripts");
        
        if (!Directory.Exists(sqlScriptsPath))
        {
            sqlScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "EPMS.Infrastructure", "SqlScripts");
        }

        if (!Directory.Exists(sqlScriptsPath))
        {
             var current = Directory.GetCurrentDirectory();
             while (current != null && !Directory.Exists(Path.Combine(current, "EPMS.Infrastructure", "SqlScripts")))
             {
                 current = Directory.GetParent(current)?.FullName;
             }
             if (current != null)
             {
                 sqlScriptsPath = Path.Combine(current, "EPMS.Infrastructure", "SqlScripts");
             }
        }

        if (!Directory.Exists(sqlScriptsPath))
        {
            Console.WriteLine($"SQL Scripts directory not found at {sqlScriptsPath}");
            return;
        }
        else
        {
            Console.WriteLine($"Found SQL Scripts directory at {sqlScriptsPath}");
        }

        // Define specific order for scripts to ensure dependencies are met
        var orderedScripts = new List<string>
        {
            "010_AddTeamIdToEmployees.sql",
            "OrgSecurity_StoredProcedures.sql",
            "EmployeePersonnel_StoredProcedures.sql",
            "AppraisalCycles_StoredProcedures.sql",
            "AppraisalForms_StoredProcedures.sql",
            "AppraisalQuestions_StoredProcedures.sql",
            "AppraisalResponses_StoredProcedures.sql",
            "FormQuestions_StoredProcedures.sql",
            "PerformanceEvaluations_StoredProcedures.sql",
            "PerformanceOutcomes_StoredProcedures.sql",
            "sp_GetKpiByPosition.sql",
            "sp_GetEmployeeKpiAssignment.sql",
            "sp_GetKpiScoreSummary.sql",
            "sp_GetDepartmentKpiSummary.sql",
            "sp_GetTeamKpiSummary.sql",
            "sp_GetPositionKpiSummary.sql",
            "sp_GetKpiAuditLog.sql"
        };

        var allFiles = Directory.GetFiles(sqlScriptsPath, "*.sql");
        var scriptFiles = orderedScripts
            .Select(name => allFiles.FirstOrDefault(f => f.EndsWith(name)))
            .Where(f => f != null)
            .ToList();

        // Add any remaining scripts not in the ordered list
        var remainingFiles = allFiles.Where(f => !orderedScripts.Any(os => f.EndsWith(os))).OrderBy(f => f);
        scriptFiles.AddRange(remainingFiles);

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        foreach (var file in scriptFiles)
        {
            if (file == null) continue;
            var script = File.ReadAllText(file);
            
            // Split script by GO command on its own line
            // More robust way to split by GO regardless of line endings
            var commands = script.Split(new[] { "\nGO\n", "\r\nGO\r\n", "\nGO\r\n", "\r\nGO\n" }, StringSplitOptions.RemoveEmptyEntries);

            // Fallback for GO without surrounding newlines (e.g., at end of file)
            if (commands.Length == 1)
            {
                commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            }

            foreach (var commandText in commands)
            {
                var trimmedCommand = commandText.Trim();
                if (string.IsNullOrWhiteSpace(trimmedCommand)) continue;

                using var command = new SqlCommand(trimmedCommand, connection);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Error executing command from {Path.GetFileName(file)}: {ex.Message}");
                    // Log the command that failed (truncated)
                    var shortCommand = trimmedCommand.Length > 100 ? trimmedCommand.Substring(0, 100) + "..." : trimmedCommand;
                    Console.WriteLine($"Failed command: {shortCommand}");
                }
            }
            Console.WriteLine($"Processed script: {Path.GetFileName(file)}");
        }

        connection.Close();

        await SeedDataWithEfAsync(scope);
    }

    private static async Task SeedDataWithEfAsync(IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // 1. Seed Permissions (already in SQL, but ensure)
        // 2. Seed System Setting - Default Password
        if (!dbContext.SystemSettings.Any(s => s.Key == "DefaultPassword"))
        {
            var defaultPasswordHashed = passwordHasher.HashPassword("admin123");
            dbContext.SystemSettings.Add(new SystemSetting
            {
                Key = "DefaultPassword",
                Value = defaultPasswordHashed,
                Description = "Default password for new employee accounts",
                UpdatedAt = DateTime.UtcNow
            });
        }

        // 3. Seed Admin Employee
        var adminPosition = dbContext.Positions.FirstOrDefault(p => p.PositionTitle == "Admin");
        var accountInitializationService = scope.ServiceProvider.GetRequiredService<IAccountInitializationService>();
        
        if (adminPosition != null)
        {
            var adminEmployee = dbContext.Employees.FirstOrDefault(e => e.Email == "admin@example.com");
            var adminPasswordHashed = passwordHasher.HashPassword("admin123");
            if (adminEmployee == null)
            {
                dbContext.Employees.Add(new Employee
                {
                    EmployeeCode = "EMP001",
                    FullName = "System Admin",
                    PositionId = adminPosition.PositionId,
                    PasswordHash = adminPasswordHashed,
                    IsFirstLogin = false,
                    IsActive = true,
                    Email = "admin@example.com"
                });
            }
            else
            {
                // Update password hash if it's not the correct hashed version
                // Check if password is plaintext "admin123" - if yes, update to hashed
                var isPlainText = adminEmployee.PasswordHash == "admin123";
                var isCorrectHash = passwordHasher.VerifyPassword(adminEmployee.PasswordHash, "admin123");
                
                if (isPlainText || !isCorrectHash)
                {
                    adminEmployee.PasswordHash = adminPasswordHashed;
                }
            }
        }
        
        await dbContext.SaveChangesAsync();
        
        // 4. Initialize all existing employees with NULL PasswordHash
        var allEmployees = dbContext.Employees.ToList();
        foreach (var employee in allEmployees)
        {
            await accountInitializationService.InitializeAccountAsync(employee);
        } 
    }
}
