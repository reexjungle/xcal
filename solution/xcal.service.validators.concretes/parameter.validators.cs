using MarkdownDeep;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;

namespace reexjungle.xcal.service.validators.concretes
{
    public class LanguageValidator : AbstractValidator<LANGUAGE>
    {
        public LanguageValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.Tag).Must((x, y) => !string.IsNullOrWhiteSpace(y));
        }
    }

    public class TimeZoneIdValidator : AbstractValidator<TZID>
    {
        public TimeZoneIdValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.Prefix).NotNull().When(x => x.GloballyUnique = false);
            RuleFor(x => x.Suffix).NotNull().NotEmpty().When(x => x.Suffix != null);

        }
    }

    public class DelegateValidator : AbstractValidator<DELEGATE>
    {
        public DelegateValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.Addresses).SetCollectionValidator(new CalendarAddressValidator()).When(x => !x.Addresses.NullOrEmpty());
        }
    }

    public class FormatTypeValidator : AbstractValidator<FMTTYPE>
    {
        public FormatTypeValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.TypeName).NotNull().NotEmpty();
            RuleFor(x => x.SubTypeName).NotNull().NotEmpty();
        }
    }

    public class MemberValidator : AbstractValidator<MEMBER>
    {
        public MemberValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.Addresses).SetCollectionValidator(new CalendarAddressValidator()).When(x => !x.Addresses.NullOrEmpty());
        }
    }
}