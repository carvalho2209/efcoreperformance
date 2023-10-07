using EFCorePerformance.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCorePerformance;

public class DataBaseContext : DbContext
{
    public DataBaseContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(builder =>
        {
            builder.ToTable("Companies");

            builder
                .HasMany(c => c.Employees)
                .WithOne()
                .HasForeignKey(c => c.CompanyId)
                .IsRequired();

            builder.HasData(new Company
            {
                Id = 1,
                Name = "Alex Company",
            });
        });

        modelBuilder.Entity<Employee>(builder =>
        {
            builder.ToTable("Employees");

            var employees = Enumerable.Range(1, 1000)
            .Select(id => new Employee
            {
                Id = id,
                Name = $"Employee #{id}",
                Salary = 100.0m,
                CompanyId = 1
            }).ToList();

            builder.HasData(employees);
        });

        base.OnModelCreating(modelBuilder);
    }
}