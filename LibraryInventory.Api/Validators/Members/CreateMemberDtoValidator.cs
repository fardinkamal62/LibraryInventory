using FluentValidation;
using LibraryInventory.Api.Dtos.Members;

namespace LibraryInventory.Api.Validators.Members;

public class CreateMemberDtoValidator : AbstractValidator<CreateMemberDto>
{
    public CreateMemberDtoValidator()
    {
        RuleFor(x => x.MemberName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
    }
}