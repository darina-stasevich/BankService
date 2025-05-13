using System.ComponentModel.DataAnnotations.Schema;

namespace BankService.Domain.Entities.DTOs;

public class TransferRequest
{
    public Guid? SenderAccountId { get; set; }
    public Guid? ReceiverAccountId { get; set; }
    public decimal Amount { get; set; }
}