namespace EFCorePerformance.Entities;

public class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Salary { get; set; }
    public int CompanyId { get; set; } 
}