namespace BankService.Domain.Entities;

public class TransferStatistics
{
    public decimal TotalTransactionAmount { get; set; }
    public decimal TotalTransfersAmount { get; set; }
    public decimal TransfersInsideBankAmount { get; set; }
    public decimal TransfersOutsideBankAmount { get; set; }
    public decimal SalaryProjectsAmount { get; set; }
    
    public int TotalTransaction { get; set; }
    public int TotalTransfers { get; set; }
    public int TransfersInsideBank { get; set; }
    public int SalaryProjects { get; set; }
    public int TransfersOutsideBank { get; set; }

    public override string ToString()
    {
        return $"Transactions: {TotalTransaction} Amount: {TotalTransfersAmount}\n" + 
         $"Transfers: {TotalTransfers} Amount: {TotalTransfersAmount}\n" +
        $"Transfers inside bank: {TransfersInsideBank} Amount: {TransfersInsideBankAmount}\n" +
        $"Transfers inside bank: {TransfersOutsideBank} Amount: {TransfersOutsideBankAmount}\n" +
        $"Salary projects: {SalaryProjects} Amount: {SalaryProjectsAmount}";
        
    }
}