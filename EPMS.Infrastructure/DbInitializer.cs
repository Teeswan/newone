using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EPMS.Infrastructure;

public static class DbInitializer
{
    public static void InitializeDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
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
            "OrgSecurity_StoredProcedures.sql",
            "EmployeePersonnel_StoredProcedures.sql",
            "AppraisalCycles_StoredProcedures.sql",
            "AppraisalForms_StoredProcedures.sql",
            "AppraisalQuestions_StoredProcedures.sql",
            "AppraisalResponses_StoredProcedures.sql",
            "FormQuestions_StoredProcedures.sql",
            "PerformanceEvaluations_StoredProcedures.sql",
            "PerformanceOutcomes_StoredProcedures.sql"
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

        SeedData(connection);
    }

    private static void SeedData(SqlConnection connection)
    {
        // 1. Seed Permissions
        string seedPermissions = @"
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Employees.View')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Employees.View', 'View Employees');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Employees.Manage')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Employees.Manage', 'Manage Employees');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Departments.View')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Departments.View', 'View Departments');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Departments.Manage')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Departments.Manage', 'Manage Departments');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Teams.View')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Teams.View', 'View Teams');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Teams.Manage')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Teams.Manage', 'Manage Teams');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Levels.View')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Levels.View', 'View Levels');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Levels.Manage')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Levels.Manage', 'Manage Levels');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Positions.View')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Positions.View', 'View Positions');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Positions.Manage')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Positions.Manage', 'Manage Positions');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Security.ViewPermissions')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Security.ViewPermissions', 'View Security Permissions');
            IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'Permissions.Security.Manage')
            INSERT INTO Permissions (PermissionCode, Description) VALUES ('Permissions.Security.Manage', 'Manage Security Settings');
        ";
        using (var cmd = new SqlCommand(seedPermissions, connection)) cmd.ExecuteNonQuery();

        // 2. Seed Positions
        string seedPositions = @"
            IF NOT EXISTS (SELECT 1 FROM Positions WHERE PositionTitle = 'Admin')
            INSERT INTO Positions (PositionTitle) VALUES ('Admin');
        ";
        using (var cmd = new SqlCommand(seedPositions, connection)) cmd.ExecuteNonQuery();

        // 3. Seed PositionPermissions
        string seedPosPermissions = @"
            DECLARE @AdminId INT = (SELECT PositionId FROM Positions WHERE PositionTitle = 'Admin');
            IF @AdminId IS NOT NULL
            BEGIN
                -- Inserting only into existing columns
                INSERT INTO PositionPermissions (PositionId, PermissionId)
                SELECT @AdminId, PermissionId
                FROM Permissions p
                WHERE NOT EXISTS (SELECT 1 FROM PositionPermissions WHERE PositionId = @AdminId AND PermissionId = p.PermissionId);
            END
        ";
        using (var cmd = new SqlCommand(seedPosPermissions, connection)) cmd.ExecuteNonQuery();

        // 4. Seed Employee with login credentials
        string seedUser = @"
            DECLARE @AdminPosId INT = (SELECT PositionId FROM Positions WHERE PositionTitle = 'Admin');
            IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'EMP001')
            BEGIN
                INSERT INTO Employees (EmployeeCode, FullName, PositionId, Username, PasswordHash) 
                VALUES ('EMP001', 'System Admin', @AdminPosId, 'admin', 'admin123');
            END
        ";
        using (var cmd = new SqlCommand(seedUser, connection)) cmd.ExecuteNonQuery();
    }
}
