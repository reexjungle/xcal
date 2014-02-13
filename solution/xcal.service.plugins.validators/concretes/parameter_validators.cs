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

    public class TzIdValidator : AbstractValidator<ITZID>
    {
        public TzIdValidator()
            : base()
        {
            RuleFor(x => x).Must(x => x.Prefix != null).When(x => x.GloballyUnique = false);
            RuleFor(x => x).Must(x => x.Suffix != null);
        }
    }
}
