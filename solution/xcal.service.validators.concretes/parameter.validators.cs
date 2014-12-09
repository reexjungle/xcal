using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexjungle.foundation.essentials.concretes;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;

namespace reexjungle.xcal.service.validators.concretes
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
            RuleFor(x => x.Addresses).SetCollectionValidator(new UriValidator()).When(x => !x.Addresses.NullOrEmpty());
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
