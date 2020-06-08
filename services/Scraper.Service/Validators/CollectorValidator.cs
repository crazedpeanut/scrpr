using FluentValidation;
using Google.Protobuf.WellKnownTypes;

namespace Scraper.Service.Validators
{
    public class WebCollectorValidator : AbstractValidator<WebCollector>
    {
        public WebCollectorValidator()
        {
            RuleFor(_ => _.Target).IsUrl();
        }
    }

    public class CollectorValidator : AbstractValidator<Any>
    {
        public CollectorValidator()
        {
            RuleFor(_ => _)
                .UnpacksAs(WebCollector.Descriptor)
                .DependentRules(() => RuleFor(_ => _.Unpack<WebCollector>()).SetValidator(new WebCollectorValidator()).When(_ => _.Is(WebCollector.Descriptor)));
        }
    }
}