using System;
using System.Linq;
using FluentValidation;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace Scraper.Service.Validators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(_ =>
            {
                if (string.IsNullOrWhiteSpace(_))
                {
                    return false;
                }

                return Uri.TryCreate(_, UriKind.Absolute, out var outUri)
                       && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
            }).WithMessage("{PropertyName} is not a valid URL");
        }

        public static IRuleBuilderOptions<T, Any> IsType<T, TType>(this IRuleBuilder<T, Any> ruleBuilder, MessageDescriptor descriptor) where TType : IMessage, new()
        {
            return ruleBuilder.Must(_ => _.Is(descriptor) && _.TryUnpack<TType>(out var _)).WithMessage("{PropertyName} is not assignable to " + descriptor.FullName);
        }

        public static IRuleBuilderOptions<T, Any> UnpacksAs<T>(this IRuleBuilder<T, Any> ruleBuilder, params MessageDescriptor[] descriptors)
        {
            return ruleBuilder.Must(_ => descriptors.Any(descriptor => _.Is(descriptor)));
        }
    }
}