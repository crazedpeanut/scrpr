using FluentValidation;

namespace Scraper.Service.Validators
{
    public class CreateSourceRequestValidator : AbstractValidator<CreateSourceRequest>
    {
        public CreateSourceRequestValidator()
        {
            RuleFor(reg => reg.Collector)
                .NotNull()
                .SetValidator(new CollectorValidator());
        }
    }
}