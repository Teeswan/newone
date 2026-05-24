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

public class EmployeeRepository : BaseRepository<Employee, int>, IEmployeeRepository
{
    private readonly string _connectionString;
    private readonly IMemoryCache _cache;
    private readonly string _cacheKey = "all_employees_cache";


    public EmployeeRepository(AppDbContext context, IConfiguration configuration, IMemoryCache cache) : base(context)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");
        _cache = cache;
    }

    public override async Task<Employee> CreateAsync(Employee entity)
    {
        var result = await base.CreateAsync(entity);
        _cache.Remove(_cacheKey);
        return result;
    }

    public override async Task<Employee?> UpdateAsync(Employee entity)
    {
        var result = await base.UpdateAsync(entity);
        _cache.Remove(_cacheKey);
        return result;
    }

    public override async Task<bool> DeleteAsync(int id)
    {
        var result = await base.DeleteAsync(id);
        _cache.Remove(_cacheKey);
        return result;
    }

    public override async Task<IEnumerable<Employee>> GetAllAsync()
    {

        if (!_cache.TryGetValue(_cacheKey, out IEnumerable<Employee>? employees))
        {

            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                SELECT e.*, e.NRC_Number as NrcNumber, d.DepartmentName, p.PositionTitle, m.EmployeeId as ManagerId, m.FullName,
                       t.TeamID as TeamId, t.TeamName
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentId = d.DepartmentId
                LEFT JOIN Positions p ON e.PositionId = p.PositionId
                LEFT JOIN Employees m ON e.ReportsTo = m.EmployeeId
                LEFT JOIN TeamMembers tm ON e.EmployeeId = tm.EmployeeID
                LEFT JOIN Teams t ON tm.TeamID = t.TeamID
                WHERE e.IsDeleted = 0";

            var employeeDictionary = new Dictionary<int, Employee>();

            var result = await db.QueryAsync<Employee, Department, Position, Employee, Team, Employee>(
                sql,
                (employee, dept, pos, manager, team) =>
                {
                    if (!employeeDictionary.TryGetValue(employee.EmployeeId, out var employeeEntry))
                    {
                        employeeEntry = employee;
                        employeeEntry.Department = dept;
                        employeeEntry.Position = pos;
                        employeeEntry.ReportsToNavigation = manager;
                        employeeEntry.TeamsNavigation = new List<Team>();
                        employeeDictionary.Add(employeeEntry.EmployeeId, employeeEntry);
                    }

                    if (team != null)
                    {
                        employeeEntry.TeamsNavigation.Add(team);
                    }
                    
                    return employeeEntry;
                },
                splitOn: "DepartmentName,PositionTitle,ManagerId,TeamId");

            employees = employeeDictionary.Values;


            _cache.Set(_cacheKey, employees, TimeSpan.FromMinutes(30));
        }

        return employees ?? new List<Employee>();
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
        // We check against ALL employees (including deleted ones) to prevent UNIQUE constraint violations
        string sql = "SELECT TOP 1 * FROM Employees WHERE EmployeeCode = @EmployeeCode";
        return await db.QueryFirstOrDefaultAsync<Employee>(sql, new { EmployeeCode = employeeCode });
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        ArgumentNullException.ThrowIfNull(email);
        return await _dbSet.FirstOrDefaultAsync(e => e.Email == email && !e.IsDeleted);
    }

    public async Task<Employee?> GetByIdNoTrackingAsync(int id)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Employees WHERE EmployeeId = @Id AND IsDeleted = 0";
        return await db.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
    }


    public override async Task<Employee?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.ReportsToNavigation)
            .Include(e => e.TeamsNavigation)
            .FirstOrDefaultAsync(e => e.EmployeeId == id && !e.IsDeleted);
    }

    public override async Task<Employee?> GetByIdFromDbAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.ReportsToNavigation)
            .Include(e => e.TeamsNavigation)
            .FirstOrDefaultAsync(e => e.EmployeeId == id && !e.IsDeleted);
    }

}

public class LevelRepository : BaseRepository<Level, string>, ILevelRepository
{
    private readonly string _connectionString;
    public LevelRepository(AppDbContext context, IConfiguration configuration) : base(context) 
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");
    }

    public async Task<Level?> GetByIdNoTrackingAsync(string id)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Levels WHERE LevelId = @Id";
        return await db.QueryFirstOrDefaultAsync<Level>(sql, new { Id = id });
    }
}

public class PositionRepository : BaseRepository<Position, int>, IPositionRepository
{
    private readonly string _connectionString;

    public PositionRepository(AppDbContext context, IConfiguration configuration) : base(context)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");
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

    public override async Task<Position?> GetByIdFromDbAsync(int id)
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

    public async Task<Position?> GetByIdNoTrackingAsync(int id)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Positions WHERE PositionId = @Id AND IsDeleted = 0";
        return await db.QueryFirstOrDefaultAsync<Position>(sql, new { Id = id });
    }
}
