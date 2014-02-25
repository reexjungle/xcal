using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.service.plugins.validators.concretes
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

    public class UtcOffsetValidator: AbstractValidator<IUTC_OFFSET>
    {
        public UtcOffsetValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.Continue;
            RuleFor(x => x.HOUR).InclusiveBetween(0u, 23u);
            RuleFor(x => x.MINUTE).InclusiveBetween(0u, 59u);
            RuleFor(x => x.SECOND).InclusiveBetween(0u, 59u);
            RuleFor(x => x.Sign).NotEqual(SignType.Neutral);
            RuleFor(x => x.HOUR).NotEqual(0u).When(x => x.MINUTE == 0u && x.SECOND == 0u);
            RuleFor(x => x.MINUTE).NotEqual(0u).When(x => x.HOUR == 0u && x.SECOND == 0u);
            RuleFor(x => x.SECOND).NotEqual(0u).When(x => x.MINUTE == 0u && x.SECOND == 0u);
        }
    }
}
