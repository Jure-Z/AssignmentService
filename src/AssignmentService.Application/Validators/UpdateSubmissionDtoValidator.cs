using AssignmentService.Application.DTOs;
using FluentValidation;

namespace AssignmentService.Application.Validators;

public class UpdateSubmissionDtoValidator : AbstractValidator<UpdateSubmissionDto>
{
    public UpdateSubmissionDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Vsebina oddaje ne sme biti prazna.")
            .MaximumLength(10000).WithMessage("Vsebina oddaje ne sme presegati 10000 znakov.");
    }
}
