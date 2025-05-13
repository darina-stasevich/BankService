using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;

namespace BankService.Domain.Interfaces.IRepositories;

public interface ILoanRequestRepository
{
    public LoanRequest? GetById(Guid id, Guid bankId);

    public void Add(LoanRequest loanRequest);

    public void Update(LoanRequest loanRequest);

    public void Delete(Guid id);
    public IEnumerable<LoanRequest> GetLoanRequests(Guid bankId, VerificationStatus status);
}