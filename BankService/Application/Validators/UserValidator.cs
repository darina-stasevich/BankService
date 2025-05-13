using BankService.Domain.Entities;
using BankService.Domain.Interfaces.IValidators;
using FluentValidation;

namespace BankService.Application.Validators;

public class UserValidator : AbstractValidator<User>, IUserValidator
{
    public UserValidator()
    {
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid Email Address");
        RuleFor(x => x.LastName).Length(2, 25).WithMessage("Last name must be between 2 and 25 characters");
        RuleFor(x => x.FirstName).Length(1, 25).WithMessage("First name must be between 1 and 25 characters");
        RuleFor(x => x.SecondName).Length(0, 25).WithMessage("Second name must be not longer than 30 characters");
        When(x => x.IsResident, () =>
            {
                RuleFor(x => x.NationalPassportNumber).Matches(@"^[A-Z]{2}\d{7}$")
                    .WithMessage("Invalid National Passport format");
                RuleFor(x => x.NationalPassportID).Matches("^[1-6][0-9]{6}[ABCKEMH][0-9]{3}(PB|BI|BA)[0-9]$")
                    .WithMessage("Invalid National Passport ID");
                RuleFor(x => x.PhoneNumber).Matches(@"^\+375\d{9}$").WithMessage("Invalid Phone Number");
                RuleFor(x => x.ForeignPassportNumber).Empty().WithMessage("Foreign Passport Number must be empty");
                RuleFor(x => x.ForeignPassportID).Empty().WithMessage("Foreign Passport ID must be empty");
            })
            .Otherwise(() =>
            {
                RuleFor(x => x.NationalPassportNumber).Empty().WithMessage("National Passport Number must be empty");
                RuleFor(x => x.NationalPassportID).Empty().WithMessage("National Passport ID must be empty");
                RuleFor(x => x.ForeignPassportNumber).NotEmpty()
                    .WithMessage("Foreign Passport Number must not be empty");
                RuleFor(x => x.ForeignPassportID).NotEmpty().WithMessage("Foreign Passport ID must not be empty");
            });
    }
}