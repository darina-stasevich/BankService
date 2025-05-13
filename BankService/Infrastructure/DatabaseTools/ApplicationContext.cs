using BankService.Domain.Entities;
using BankService.Domain.Entities.BankAccounts;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Entities.Loans;
using BankService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankService.Infrastructure.Database;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserAccount> UserAccounts { get; set; } = null!;
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;
    public DbSet<Loan> Loans { get; set; } = null!;
    public DbSet<Enterprise> Enterprises { get; set; } = null!;
    public DbSet<LoanRequest> LoanRequests { get; set; } = null!;
    public DbSet<SalaryProject> SalaryProjects { get; set; } = null!;
    public DbSet<SalaryProjectRequest> SalaryProjectRequests { get; set; } = null!;
  
    public DbSet<Transaction> Transactions { get; set; } = null!;
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(x => x.Status).HasConversion<string>();

        modelBuilder.Entity<BankAccount>().Property(x => x.Status).HasConversion<string>();

        modelBuilder.Entity<UserAccount>().Property(x => x.UserRole).HasConversion<string>();

        modelBuilder.Entity<UserAccount>().Property(x => x.Status).HasConversion<string>();

        modelBuilder.Entity<LoanRequest>().Property(x => x.LoanType).HasConversion<string>();
        
        modelBuilder.Entity<LoanRequest>().Property(x => x.VerificationStatus).HasConversion<string>();
        
        modelBuilder.Entity<SalaryProject>().Property(x => x.Status).HasConversion<string>();
        
        modelBuilder.Entity<SalaryProjectRequest>().Property(x => x.Status).HasConversion<string>();
            
        modelBuilder.Entity<Transaction>().Property(x => x.Status).HasConversion<string>();
        modelBuilder.Entity<Transaction>().Property(x => x.Type).HasConversion<string>();
        modelBuilder.Entity<Enterprise>()
            .HasDiscriminator<string>("EnterpriseType")
            .HasValue<Enterprise>("Enterprise")
            .HasValue<Bank>("Bank");
        
        modelBuilder.Entity<BankAccount>()
            .HasDiscriminator<string>("AccountType")
            .HasValue<CurrentAccount>("Current")
            .HasValue<DepositAccount>("Deposit")
            .HasValue<SalaryAccount>("Salary")
            .HasValue<EnterpriseAccount>("Enterprise");

        modelBuilder.Entity<Loan>()
            .HasDiscriminator<string>("LoanType")
            .HasValue<Credit>("Credit")
            .HasValue<Installment>("Installment");
    }
}