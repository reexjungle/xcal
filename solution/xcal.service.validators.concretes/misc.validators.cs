using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;

namespace reexjungle.xcal.service.validators.concretes
{
    public class IANAPropertyValidator : AbstractValidator<IANA_PROPERTY>
    {
        public IANAPropertyValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Token).NotNull().NotEmpty();
            RuleFor(x => x.Value).NotNull();
        }
    }

    public class IANAComponentValidator : AbstractValidator<IANA_COMPONENT>
    {
        public IANAComponentValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.TokenName).NotNull().NotEmpty();
            RuleFor(x => x.ContentLines).SetCollectionValidator(new IANAPropertyValidator()).When(x => !x.ContentLines.NullOrEmpty());
        }
    }

    public class XPropertyValidator : AbstractValidator<X_PROPERTY>
    {
        public XPropertyValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Value).NotNull();
        }
    }

    public class XComponentValidator : AbstractValidator<X_COMPONENT>
    {
        public XComponentValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.TokenName).NotNull().NotEmpty();
        }
    }
}