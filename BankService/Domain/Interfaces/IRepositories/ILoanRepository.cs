using BankService.Domain.Entities.Loans;

namespace BankService.Domain.Interfaces.IRepositories;

public interface ILoanRepository
{
    public Loan? GetLoanById(Guid id);
    public Installment? GetInstallmentById(Guid id);
    public Credit? GetCreditById(Guid id);
    
    public IEnumerable<Loan> GetLoansByUserAccountId(Guid userAccountId);
    public void Add(Loan loan);
    public void Update(Loan loan);
}