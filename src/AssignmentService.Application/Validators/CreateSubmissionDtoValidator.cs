using AssignmentService.Application.DTOs;
using FluentValidation;

namespace AssignmentService.Application.Validators;

public class CreateSubmissionDtoValidator : AbstractValidator<CreateSubmissionDto>
{
    public CreateSubmissionDtoValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty().WithMessage("ID naloge je obvezen.");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Identifikator študenta je obvezen.")
            .MaximumLength(100).WithMessage("Identifikator študenta ne sme biti daljši od 100 znakov.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Vsebina oddaje je obvezna.")
            .MaximumLength(10000).WithMessage("Vsebina oddaje ne sme presegati 10000 znakov.");
    }
}