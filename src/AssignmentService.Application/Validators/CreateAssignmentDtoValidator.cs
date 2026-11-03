using AssignmentService.Application.DTOs;
using FluentValidation;

namespace AssignmentService.Application.Validators;

public class CreateAssignmentDtoValidator : AbstractValidator<CreateAssignmentDto>
{
    public CreateAssignmentDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Naslov naloge je obvezen.")
            .MaximumLength(200).WithMessage("Naslov ne sme biti daljši od 200 znakov.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Opis naloge je obvezen.");

        RuleFor(x => x.Deadline)
            .NotEmpty().WithMessage("Rok oddaje je obvezen.")
            .GreaterThan(_ => DateTime.UtcNow).WithMessage("Rok oddaje mora biti v prihodnosti.");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("Največje število točk mora biti večje od 0.");
    }
}