using MarkdownDeep;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;

namespace reexjungle.xcal.service.validators.concretes
{
    public class LanguageValidator : AbstractValidator<LANGUAGE>
    {
        public LanguageValidator()
        {
            RuleFor(x => x.Tag).Must((x, y) => !string.IsNullOrWhiteSpace(y));
        }
    }

    public class TimeZoneIdValidator : AbstractValidator<TZID>
    {
        public TimeZoneIdValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Prefix).NotNull().When(x => x.GloballyUnique = false);
            RuleFor(x => x.Suffix).NotNull();
        }
    }

    public class DelegateValidator : AbstractValidator<DELEGATE>
    {
        public DelegateValidator()
        {
            RuleFor(x => x.Addresses).SetCollectionValidator(new UriValidator()).When(x => !x.Addresses.NullOrEmpty());
        }
    }

    public class FormatTypeValidator : AbstractValidator<FMTTYPE>
    {
        public FormatTypeValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.TypeName).NotNull().NotEmpty();
            RuleFor(x => x.SubTypeName).NotNull().NotEmpty();
        }
    }

    public class MemberValidator : AbstractValidator<MEMBER>
    {
        public MemberValidator()
        {
            RuleFor(x => x.Addresses).SetCollectionValidator(new UriValidator()).When(x => !x.Addresses.NullOrEmpty());
        }
    }
}