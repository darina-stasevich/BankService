using System.ComponentModel.DataAnnotations.Schema;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BankService.Domain.Entities;

public class SalaryProjectRequest
{
    public Guid Id { get; set; }
    
    [ForeignKey("EmployeeAccount")]
    public Guid EmployeeAccountId { get; set; } // UserAccount of employee
    public Guid? EnterpriseId { get; set; }   // предприятие сотрудника
    
    [ForeignKey("SalaryAccount")]
    public Guid SalaryAccountId { get; set; } // счет, куда будет начисляться зарплата
    
    [ForeignKey("Bank")]
    public Guid BankId { get; set; }         // банк, куда подается заявка
    public string ProjectId { get; set; }      // просто id проекта
    public decimal Salary { get; set; } 
    public VerificationStatus Status { get; set; }
    
    // navigation properties
    
    public UserAccount? EmployeeAccount { get; set; }
    public Bank? Bank { get; set; }
    public BankAccount? SalaryAccount { get; set; }
    
}