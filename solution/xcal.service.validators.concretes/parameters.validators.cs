using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.service.validators.concretes
{
    public class AltrepValidator: AbstractValidator<IALTREP>
    {
        public AltrepValidator(): base()
        {
            RuleFor(x => x).SetValidator(new UriValidator());
        }
    }

    public class LanguageValidator: AbstractValidator<ILANGUAGE>
    {
        public LanguageValidator(): base()
        {
            RuleFor(x => x).Must(x => x.Tag != null);
        }
    }

    public class TimeZoneIdValidator : AbstractValidator<ITZID>
    {
        public TimeZoneIdValidator()
            : base()
        {
            RuleFor(x => x).Must(x => x.Prefix != null).When(x => x.GloballyUnique = false);
            RuleFor(x => x).Must(x => x.Suffix != null);
        }
    }

    public class DelegateValidator: AbstractValidator<IDELEGATE>
    {
        public DelegateValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Addresses).SetCollectionValidator(new UriValidator()).When(x => !x.Addresses.NullOrEmpty());
            RuleFor(x => x.Addresses.Select(y => y.Path)).SetCollectionValidator(new EmailAddressValidator()).When(x => !x.Addresses.NullOrEmpty());
        }
    }

    public class FormatTypeValidator: AbstractValidator<IFMTTYPE>
    {
        public FormatTypeValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.TypeName).NotNull().NotEmpty();
            RuleFor(x => x.SubTypeName).NotNull().NotEmpty();
        }
    }

}
