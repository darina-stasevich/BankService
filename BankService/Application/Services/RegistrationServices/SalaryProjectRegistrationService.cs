using System.Runtime.InteropServices.JavaScript;
using BankService.Domain.Entities;
using BankService.Domain.Entities.DTOs;
using BankService.Domain.Enums;
using BankService.Domain.Interfaces.IRepositories;
using BankService.Domain.Interfaces.Services.RegistrationServices;
using BankService.Domain.Results;

namespace BankService.Application.Services;

public class SalaryProjectRegistrationService(
    IUserAccountRepository userAccountRepository,
    IBankAccountRepository bankAccountRepository,
    ISalaryProjectRequestRepository salaryProjectRequestRepository,
    IEnterpriseRepository enterpriseRepository) : ISalaryProjectRegistrationService
{
    private SalaryProjectRequest CreateRequest(Guid employeeAccountId, SalaryProjectRequestDTO salaryProjectRequestDto)
    {
        var request = new SalaryProjectRequest
        {
            Id = new Guid(),
            EmployeeAccountId = employeeAccountId,
            EnterpriseId = salaryProjectRequestDto.EnterpriseId,
            BankId = salaryProjectRequestDto.BankId,
            ProjectId = salaryProjectRequestDto.ProjectId,
            Salary = salaryProjectRequestDto.Amount,
            SalaryAccountId = salaryProjectRequestDto.SalaryAccountId,
            Status = VerificationStatus.Pending
            
        };

        return request;
    }
    
    public Result<Guid> CreateSalaryProjectRequest(Guid employeeUserAccountId, SalaryProjectRequestDTO salaryProjectRequestDto)
    {
        var bank = enterpriseRepository.GetByName(salaryProjectRequestDto.BankName);
        if(bank == null)
            return Error.NotFound(400, $"bank with name {salaryProjectRequestDto.BankName} not found");
        if(!enterpriseRepository.IsBank(salaryProjectRequestDto.BankName))
            return Error.NotFound(400, $"bank with name {salaryProjectRequestDto.BankName} not found");
        var employeeUserAccount = userAccountRepository.GetById(employeeUserAccountId, bank.Id);
        if(employeeUserAccount == null)
            return Error.NotFound(400, $"user account with id: {employeeUserAccountId} not found");
        if(employeeUserAccount.Status != VerificationStatus.Approved)
            return Error.AccessForbidden(403, "invalid status of account to send salary request");
        if (salaryProjectRequestDto.SalaryAccountId == Guid.Empty)
            return Error.Validation(400, "salary account cannot be empty");
        var salaryAccount = bankAccountRepository.GetById(salaryProjectRequestDto.SalaryAccountId, bank.Id);
        if(salaryAccount == null)
            return Error.NotFound(400, "salary account not found");   
        if(salaryAccount.Status != BankAccountStatus.Active)
            return Error.Validation(400, "bank account must be active");
        if(salaryAccount.Type != BankAccountType.Salary)
            return Error.Validation(400, "bank account must be salary");
        if(salaryAccount.UserAccountId != employeeUserAccountId)
            return Error.Failure(400, "user account id must be equal to employee");
        salaryProjectRequestDto.EnterpriseId = salaryAccount.EnterpriseId!.Value;
        salaryProjectRequestDto.BankId = bank.Id;
        var request = CreateRequest(employeeUserAccountId, salaryProjectRequestDto);
        salaryProjectRequestRepository.Add(request);
        return request.Id;
    }
}