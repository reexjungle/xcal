using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.validators.concretes
{

    public class LanguageValidator: AbstractValidator<LANGUAGE>
    {
        public LanguageValidator(): base()
        {
            RuleFor(x => x.Tag).NotNull();
        }
    }

    public class TimeZoneIdValidator : AbstractValidator<TZID>
    {
        public TimeZoneIdValidator()
            : base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x =>x.Prefix).NotNull().When(x => x.GloballyUnique = false);
            RuleFor(x => x.Suffix).NotNull();
        }
    }

    public class DelegateValidator: AbstractValidator<DELEGATE>
    {
        public DelegateValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Addresses).SetCollectionValidator(new UriValidator()).When(x => !x.Addresses.NullOrEmpty());
            RuleFor(x => x.Addresses.Select(y => y.Path)).SetCollectionValidator(new EmailAddressValidator()).When(x => !x.Addresses.NullOrEmpty());
        }
    }

    public class FormatTypeValidator: AbstractValidator<FMTTYPE>
    {
        public FormatTypeValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.TypeName).NotNull().NotEmpty();
            RuleFor(x => x.SubTypeName).NotNull().NotEmpty();
        }
    }

    public class MemberValidator: AbstractValidator<MEMBER>
    {
        public MemberValidator(): base()
        {
            RuleFor(x => x.Addresses).SetCollectionValidator(new UriValidator()).When(x => !x.Addresses.NullOrEmpty());
        }
    }
}
