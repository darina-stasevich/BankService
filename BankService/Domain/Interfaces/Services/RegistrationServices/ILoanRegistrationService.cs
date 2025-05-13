using BankService.Domain.Entities.DTOs;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.RegistrationServices;

public interface ILoanRegistrationService
{
    public Result<Guid> RegisterLoan(Guid userAccountId, string bankName, LoanRequestDTO loanRequestDto);
}