using System.ComponentModel.DataAnnotations;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities;

public class Enterprise
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Enterprise type must be specified")] public EnterpriseType Type { get; set; }

    [Required(ErrorMessage = "Name can not be empty")]
    [MinLength(10)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Address can not be empty")]
    [MinLength(10)]
    public string Address { get; set; } = null!;
    
    // navigation properties
    
    public List<UserAccount>? SpecialistsAccounts { get; set; }
}