using System.ComponentModel.DataAnnotations;
using BankService.Domain.Enums;

namespace BankService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Last name is not specified")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "First name is not specified")]
    public required string FirstName { get; set; }

    public string? SecondName { get; set; }

    [Required(ErrorMessage = "Email is not specified")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Phone number is not specified")]
    public required string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Nationality is not specified")]
    public required bool IsResident { get; set; }

    public string? NationalPassportNumber { get; set; }

    public string? NationalPassportID { get; set; }

    public string? ForeignPassportNumber { get; set; }
    public string? ForeignPassportID { get; set; }

    public VerificationStatus Status { get; set; }
    
    // Navigation properties
    
    public List<UserAccount>? UserAccounts { get; set; }
}