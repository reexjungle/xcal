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
            RuleFor(x => x).Must(x => Uri.IsWellFormedUriString(x.Path, UriKind.RelativeOrAbsolute)).When(x => x.Path != null);
        }
    }

    public class LanguageValidator: AbstractValidator<ILANGUAGE>
    {
        public LanguageValidator(): base()
        {
            RuleFor(x => x).Must(x => x.Tag != null);
        }
    }
}
