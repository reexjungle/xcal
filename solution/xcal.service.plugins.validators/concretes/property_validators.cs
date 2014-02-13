using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.plugins.validators.concretes
{

    public class TextValidator: AbstractValidator<ITEXT>
    {
        public TextValidator() : base()
        {
            RuleFor(x => x.Text).NotNull().When( x => x != null);
            RuleFor(x => x.AlternativeText).SetValidator(new AltrepValidator()).When(x => x.AlternativeText != null);
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class RecurrenceIdValidator: AbstractValidator<IRECURRENCE_ID>
    {
        public RecurrenceIdValidator(): base()
        {
            RuleFor(x => x.Value).NotNull().When(x => x != null);
            RuleFor(x => x.Range).NotEqual(RANGE.UNKNOWN).When(x => x != null);
            RuleFor(x => x.TimeZoneId).SetValidator(new TzIdValidator()).When(x => x.TimeZoneId != null);
        }
    }
}
