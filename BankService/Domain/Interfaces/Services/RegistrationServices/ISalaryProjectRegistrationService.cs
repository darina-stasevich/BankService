using BankService.Domain.Entities.DTOs;
using BankService.Domain.Results;

namespace BankService.Domain.Interfaces.Services.RegistrationServices;

public interface ISalaryProjectRegistrationService
{
    public Result<Guid> CreateSalaryProjectRequest(Guid employeeUserAccountId,
        SalaryProjectRequestDTO salaryProjectRequestDto);
}