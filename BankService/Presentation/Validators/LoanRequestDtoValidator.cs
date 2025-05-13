using BankService.Domain.Entities.DTOs;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Presentation.Validators;

public class LoanRequestDtoValidator : AbstractValidator<LoanRequestDTO>, ILoanRequestDtoValidator
{
    public LoanRequestDtoValidator()
    {
        RuleFor(x => x.TermMonths).GreaterThan(0).Must(term => term == 3 || term == 6 || term == 12 || term % 24 == 0)
            .WithMessage("Possible terms are: 3, 6, 12 months or positive integer amount of years ");
        RuleFor(x => x.TotalAmount).GreaterThan(0).WithMessage("Total amount must be greater than 0");
        RuleFor(x => x.InterestRate).GreaterThan(0).WithMessage("Interest rate must be greater than 0");
    }
}