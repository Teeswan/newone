using Dapper;
using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EPMS.Infrastructure.Repositories;

public class EmployeeRepository(AppDbContext context, IConfiguration configuration, IMemoryCache cache) : BaseRepository<Employee, int>(context), IEmployeeRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");
    private readonly IMemoryCache _cache = cache;
    private readonly string _cacheKey = "all_employees_cache";


    public override async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.ReportsToNavigation)
            .Include(e => e.TeamsNavigation)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }

    public override async Task<IEnumerable<Employee>> GetAllAsync()
    {
        if (!_cache.TryGetValue(_cacheKey, out IEnumerable<Employee>? employees))
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            // Explicitly alias every column to match C# property names and avoid case-sensitivity issues
            string sql = @"
                SELECT e.EmployeeId as EmployeeId, 
                       e.EmployeeCode as EmployeeCode, 
                       e.FullName as FullName, 
                       e.Email as Email, 
                       e.DepartmentId as DepartmentId, 
                       e.PositionId as PositionId, 
                       e.ReportsTo as ReportsTo, 
                       e.JoinDate as JoinDate, 
                       e.IsActive as IsActive, 
                       e.IsDeleted as IsDeleted,
                       e.CurrentSalary as CurrentSalary, 
                       e.NRC_Number as NrcNumber, 
                       e.Phone as Phone, 
                       e.Address as Address, 
                       e.DateOfBirth as DateOfBirth, 
                       e.EmploymentStatus as EmploymentStatus,
                       (SELECT STRING_AGG(t.TeamName, ', ') 
                        FROM Teams t 
                        JOIN TeamMembers tm ON t.TeamId = tm.TeamId 
                        WHERE tm.EmployeeId = e.EmployeeId) as TeamNames,
                       d.DepartmentId as DeptId, d.DepartmentName as DeptName, 
                       p.PositionId as PosId, p.PositionTitle as PosTitle, 
                       m.EmployeeId as ManagerId, m.FullName as ManagerName
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentId = d.DepartmentId
                LEFT JOIN Positions p ON e.PositionId = p.PositionId
                LEFT JOIN Employees m ON e.ReportsTo = m.EmployeeId
                WHERE e.IsDeleted = 0";

            var result = await db.QueryAsync<dynamic>(sql);
            var employeeDictionary = new Dictionary<int, Employee>();
            
            foreach (var row in result)
            {
                var rowDict = (IDictionary<string, object>)row;
                
                // Helper to safely get values from the dictionary regardless of DBNull
                T? GetValue<T>(string key)
                {
                    if (rowDict.TryGetValue(key, out var val) && val != null && val != DBNull.Value)
                    {
                        try {
                            if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
                                return (T)(object)Convert.ToInt32(val);
                            if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                                return (T)(object)Convert.ToDecimal(val);
                            if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
                                return (T)(object)Convert.ToBoolean(val);
                            return (T)val;
                        } catch { return default; }
                    }
                    return default;
                }

                int empId = GetValue<int>("EmployeeId");
                if (empId == 0) continue;

                if (!employeeDictionary.TryGetValue(empId, out var employee))
                {
                    employee = new Employee
                    {
                        EmployeeId = empId,
                        EmployeeCode = GetValue<string>("EmployeeCode") ?? "",
                        FullName = GetValue<string>("FullName") ?? "",
                        Email = GetValue<string>("Email"),
                        DepartmentId = GetValue<int?>("DepartmentId"),
                        PositionId = GetValue<int?>("PositionId"),
                        ReportsTo = GetValue<int?>("ReportsTo"),
                        JoinDate = GetValue<DateTime?>("JoinDate"),
                        IsActive = GetValue<bool?>("IsActive"),
                        IsDeleted = GetValue<bool>("IsDeleted"),
                        TeamNames = GetValue<string>("TeamNames"),
                        CurrentSalary = GetValue<decimal?>("CurrentSalary"),
                        NrcNumber = GetValue<string>("NrcNumber"),
                        Phone = GetValue<string>("Phone"),
                        Address = GetValue<string>("Address"),
                        DateOfBirth = GetValue<DateTime?>("DateOfBirth"),
                        EmploymentStatus = GetValue<string>("EmploymentStatus")
                    };

                    // Map Department
                    int dId = GetValue<int>("DeptId");
                    if (dId != 0)
                    {
                        employee.Department = new Department
                        {
                            DepartmentId = dId,
                            DepartmentName = GetValue<string>("DeptName") ?? ""
                        };
                    }

                    // Map Position
                    int pId = GetValue<int>("PosId");
                    if (pId != 0)
                    {
                        employee.Position = new Position
                        {
                            PositionId = pId,
                            PositionTitle = GetValue<string>("PosTitle") ?? ""
                        };
                    }

                    // Map Manager
                    int mId = GetValue<int>("ManagerId");
                    if (mId != 0)
                    {
                        employee.ReportsToNavigation = new Employee
                        {
                            EmployeeId = mId,
                            FullName = GetValue<string>("ManagerName") ?? ""
                        };
                    }

                    employeeDictionary.Add(empId, employee);
                }
            }
            
            employees = employeeDictionary.Values;
            _cache.Set(_cacheKey, employees, TimeSpan.FromMinutes(30));
        }

        return employees ?? [];
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Employees WHERE DepartmentId = @DepartmentId AND IsDeleted = 0";
        return await db.QueryAsync<Employee>(sql, new { DepartmentId = departmentId });
    }

    public async Task<IEnumerable<Employee>> GetDirectReportsAsync(int managerId)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Employees WHERE ReportsTo = @ManagerId AND IsDeleted = 0";
        return await db.QueryAsync<Employee>(sql, new { ManagerId = managerId });
    }

    public async Task<Employee?> GetByCodeAsync(string employeeCode)
    {
        ArgumentNullException.ThrowIfNull(employeeCode);
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT TOP 1 * FROM Employees WHERE EmployeeCode = @EmployeeCode AND IsDeleted = 0";
        return await db.QueryFirstOrDefaultAsync<Employee>(sql, new { EmployeeCode = employeeCode });
    }

    public async Task<Employee?> GetByUsernameAsync(string username)
    {
        ArgumentNullException.ThrowIfNull(username);
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT TOP 1 * FROM Employees WHERE Username = @Username AND IsDeleted = 0";
        return await db.QueryFirstOrDefaultAsync<Employee>(sql, new { Username = username });
    }


    public override async Task<Employee> CreateAsync(Employee entity)
    {
        // To handle many-to-many relationship with Teams (TeamsNavigation)
        // without trying to insert existing teams as new records.
        if (entity.TeamsNavigation != null && entity.TeamsNavigation.Any())
        {
            var selectedTeamIds = entity.TeamsNavigation.Select(t => t.TeamId).ToList();
            entity.TeamsNavigation.Clear();

            foreach (var teamId in selectedTeamIds)
            {
                var team = await _context.Teams.FindAsync(teamId);
                if (team != null)
                {
                    entity.TeamsNavigation.Add(team);
                }
            }
        }

        // Ensure related entities are not treated as new
        entity.Department = null;
        entity.Position = null;
        entity.ReportsToNavigation = null;

        var result = await base.CreateAsync(entity);
        _cache.Remove(_cacheKey);
        return result;
    }

    public override async Task<Employee?> UpdateAsync(Employee entity)
    {
        // For updates, we need to handle the collection carefully
        // 1. Fetch the existing employee with its current teams
        var existingEmployee = await _context.Employees
            .Include(e => e.TeamsNavigation)
            .FirstOrDefaultAsync(e => e.EmployeeId == entity.EmployeeId);

        if (existingEmployee == null) return null;

        // 2. Update scalar properties explicitly to ensure no accidental nulls
        existingEmployee.EmployeeCode = entity.EmployeeCode;
        existingEmployee.FullName = entity.FullName;
        existingEmployee.Email = entity.Email;
        existingEmployee.DepartmentId = entity.DepartmentId;
        existingEmployee.PositionId = entity.PositionId;
        existingEmployee.ReportsTo = entity.ReportsTo;
        existingEmployee.JoinDate = entity.JoinDate;
        existingEmployee.CurrentSalary = entity.CurrentSalary;
        existingEmployee.Phone = entity.Phone;
        existingEmployee.Address = entity.Address;
        existingEmployee.DateOfBirth = entity.DateOfBirth;
        existingEmployee.EmploymentStatus = entity.EmploymentStatus;
        existingEmployee.NrcNumber = entity.NrcNumber;
        existingEmployee.IsActive = entity.IsActive;

        // 3. Update TeamsNavigation (Many-to-Many)
        if (entity.TeamsNavigation != null)
        {
            var selectedTeamIds = entity.TeamsNavigation.Select(t => t.TeamId).ToList();
            existingEmployee.TeamsNavigation.Clear();

            foreach (var teamId in selectedTeamIds)
            {
                var team = await _context.Teams.FindAsync(teamId);
                if (team != null)
                {
                    existingEmployee.TeamsNavigation.Add(team);
                }
            }
        }

        // Ensure related entities are not treated as new or updated
        _context.Entry(existingEmployee).Reference(e => e.Department).IsModified = false;
        _context.Entry(existingEmployee).Reference(e => e.Position).IsModified = false;
        _context.Entry(existingEmployee).Reference(e => e.ReportsToNavigation).IsModified = false;

        existingEmployee.Department = null;
        existingEmployee.Position = null;
        existingEmployee.ReportsToNavigation = null;

        await _context.SaveChangesAsync();
        
        // CRITICAL: Clear cache so the list reflects the new data
        _cache.Remove(_cacheKey);
        
        return existingEmployee;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        var result = await base.DeleteAsync(id);
        if (result)
        {
            _cache.Remove(_cacheKey);

        }
        return result;
    }

}

public class LevelRepository(AppDbContext context) : BaseRepository<Level, string>(context), ILevelRepository
{
}

public class PositionRepository(AppDbContext context, IConfiguration configuration) : BaseRepository<Position, int>(context), IPositionRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");

    public async Task<IEnumerable<Position>> GetPositionsByLevelAsync(int positionId)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Positions WHERE PositionId = @PositionId";
        return await db.QueryAsync<Position>(sql, new { PositionId = positionId });
    }

    public override async Task<IEnumerable<Position>> GetAllAsync()
    {
        return await _dbSet.Include(p => p.Level)
            .AsNoTracking()
            .ToListAsync();
    }

    public override async Task<Position?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(p => p.Level)
            .FirstOrDefaultAsync(p => p.PositionId == id);
    }

    public async Task<IEnumerable<Position>> GetPositionsByLevelAsync(string levelId)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Positions WHERE LevelId = @LevelId";
        return await db.QueryAsync<Position>(sql, new { LevelId = levelId });
    }
}
