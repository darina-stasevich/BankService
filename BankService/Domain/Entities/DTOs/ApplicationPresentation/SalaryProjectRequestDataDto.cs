namespace BankService.Domain.Entities.DTOs;

public class SalaryProjectRequestDataDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string PassportNumber { get; set; }
    public string ProjectId { get; set; }
    public decimal Amount { get; set; }
}