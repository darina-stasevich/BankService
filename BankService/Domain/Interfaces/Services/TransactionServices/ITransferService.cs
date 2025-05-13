using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.TransactionServices;

public interface ITransferService
{
    public Result PerformTransfer(Guid userAccountId, TransferRequest request, string senderBankName, string receiverBankName);
    public Result TopUp(Guid userAccountId, TransferRequest request, string bankName);
    public Result PerformWithdraw(Guid userAccountId,TransferRequest request, string bankName);

    public Result PerformSalaryProjectTransaction(Guid specialistAccountId, string bankName, Guid enterpriseAccountId,
        Guid projectId);

    public Result<Guid> CancelTransaction(Guid userAccountId, string bankName, Guid transactionId);
}