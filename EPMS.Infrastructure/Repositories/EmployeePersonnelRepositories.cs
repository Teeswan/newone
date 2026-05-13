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

    public override async Task<IEnumerable<Employee>> GetAllAsync()
    {

        if (!_cache.TryGetValue(_cacheKey, out IEnumerable<Employee>? employees))
        {

            using IDbConnection db = new SqlConnection(_connectionString);
            string sql = @"
                SELECT e.*, d.DepartmentName, p.PositionTitle, m.FullName as ManagerName
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentId = d.DepartmentId
                LEFT JOIN Positions p ON e.PositionId = p.PositionId
                LEFT JOIN Employees m ON e.ReportsTo = m.EmployeeId
                WHERE e.IsActive = 1";

            employees = await db.QueryAsync<Employee, Department, Position, Employee, Employee>(
                sql,
                (employee, dept, pos, manager) =>
                {
                    employee.Department = dept;
                    employee.Position = pos;
                    employee.ReportsToNavigation = manager;
                    return employee;
                },
                splitOn: "DepartmentName,PositionTitle,ManagerName");


            _cache.Set(_cacheKey, employees, TimeSpan.FromMinutes(30));
        }

        return employees ?? new List<Employee>();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Employees WHERE DepartmentId = @DepartmentId AND IsActive = 1";
        return await db.QueryAsync<Employee>(sql, new { DepartmentId = departmentId });
    }

    public async Task<IEnumerable<Employee>> GetDirectReportsAsync(int managerId)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Employees WHERE ReportsTo = @ManagerId AND IsActive = 1";
        return await db.QueryAsync<Employee>(sql, new { ManagerId = managerId });
    }

    public async Task<Employee?> GetByCodeAsync(string employeeCode)
    {
        ArgumentNullException.ThrowIfNull(employeeCode);
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT TOP 1 * FROM Employees WHERE EmployeeCode = @EmployeeCode";
        return await db.QueryFirstOrDefaultAsync<Employee>(sql, new { EmployeeCode = employeeCode });
    }

    public async Task<Employee?> GetByUsernameAsync(string username)
    {
        ArgumentNullException.ThrowIfNull(username);
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT TOP 1 * FROM Employees WHERE Username = @Username";
        return await db.QueryFirstOrDefaultAsync<Employee>(sql, new { Username = username });
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
        if (result)
        {
            _cache.Remove(_cacheKey);

        }
        return result;
    }

}

public class LevelRepository : BaseRepository<Level, string>, ILevelRepository
{
    public LevelRepository(AppDbContext context) : base(context) { }
}

public class PositionRepository : BaseRepository<Position, int>, IPositionRepository
{
    private readonly string _connectionString;

    public PositionRepository(AppDbContext context, IConfiguration configuration) : base(context)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is missing from configuration.");
    }

    public async Task<IEnumerable<Position>> GetPositionsByLevelAsync(string levelId)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sql = "SELECT * FROM Positions WHERE LevelId = @LevelId";
        return await db.QueryAsync<Position>(sql, new { LevelId = levelId });
    }
}
