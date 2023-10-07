using Dapper;
using EFCorePerformance;
using EFCorePerformance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContext<DataBaseContext>(
    x => x.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPut("increase-salaries", async (int companyId, DataBaseContext db) =>
{
    var company = await db
        .Set<Company>()
        .Include(c => c.Employees)
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company == null)
    {
        return Results.NotFound($"The company with id '{companyId}' was not found.");
    }

    foreach (var employee in company.Employees)
    {
        employee.Salary *= 1.1m;
    }

    company.LastSalaryUpdateUtc = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("increase-salaries-sql", async (int companyId, DataBaseContext db) =>
{
    var company = await db
        .Set<Company>()
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company == null)
    {
        return Results.NotFound($"The company with id '{companyId}' was not found.");
    }

    await db.Database.BeginTransactionAsync();

    await db.Database.ExecuteSqlInterpolatedAsync
        ($"update employees set salary = salary * 1.1 where companyId = {company.Id}");

    company.LastSalaryUpdateUtc = DateTime.UtcNow;

    await db.SaveChangesAsync();

    await db.Database.CommitTransactionAsync();

    return Results.NoContent();
});

app.MapPut("increase-salaries-dapper", async (int companyId, DataBaseContext db) =>
{
    var company = await db
        .Set<Company>()
        .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company == null)
    {
        return Results.NotFound($"The company with id '{companyId}' was not found.");
    }

    var transaction = await db.Database.BeginTransactionAsync();

    await db.Database.GetDbConnection().ExecuteAsync
        ("UPDATE Employees SET SALARY = SALARY * 1.1 WHERE CompanyId = @CompanyId",
        new { CompanyId = company.Id }, 
        transaction.GetDbTransaction());

    company.LastSalaryUpdateUtc = DateTime.UtcNow;

    await db.SaveChangesAsync();

    await db.Database.CommitTransactionAsync();

    return Results.NoContent();
});



app.Run();