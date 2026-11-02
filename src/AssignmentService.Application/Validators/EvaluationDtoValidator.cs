using AssignmentService.Application.DTOs;
using FluentValidation;

namespace AssignmentService.Application.Validators;

public class UpsertEvaluationDtoValidator : AbstractValidator<UpsertEvaluationDto>
{
    public UpsertEvaluationDtoValidator()
    {
        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Ocena ne more biti manjša od 0.");

        RuleFor(x => x.Feedback)
            .MaximumLength(2000).WithMessage("Povratna informacija ne sme presegati 2000 znakov.");
    }
}