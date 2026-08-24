using FluentValidation;
using LibraryInventory.Dtos.Members;

namespace LibraryInventory.Validators.Members;

public class CreateMemberDtoValidator : AbstractValidator<CreateMemberDto>
{
    public CreateMemberDtoValidator()
    {
        RuleFor(x => x.MemberName)
            .NotEmpty().WithMessage("Member name is required")
            .MaximumLength(255);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(150);
    }
}
