using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.service.plugins.validators.concretes
{
    public class UriValidator : AbstractValidator<IURI>
    {
        public UriValidator()
            : base()
        {
            RuleFor(x => x).Must(x => Uri.IsWellFormedUriString(x.Path, UriKind.RelativeOrAbsolute)).When(x => x.Path != null);
        }
    }

    public class WeekDayNumValidator : AbstractValidator<IWEEKDAYNUM>
    {
        public WeekDayNumValidator()
            : base()
        {
            RuleFor(x => x.OrdinalWeek).GreaterThan(0);
            RuleFor(x => x.OrdinalWeek).LessThan(54);
            RuleFor(x => x.Weekday).NotEqual(WEEKDAY.UNKNOWN);
        }
    }

    public class RecurrenceValidator: AbstractValidator<IRECUR>
    {
        public RecurrenceValidator()
            : base()
        {
            RuleFor(x => x.FREQ).NotEqual(FREQ.UNKNOWN);
            RuleFor(x => x.UNTIL).NotNull().When(x => x != null && x.Format == RecurFormat.DateTime);
            RuleFor(x => x.UNTIL).Must((x, y) => x.UNTIL == null).When(x => x.Format == RecurFormat.Range);
            RuleFor(x => x.BYDAY).SetCollectionValidator(new WeekDayNumValidator()).When(x => !x.BYDAY.NullOrEmpty());
        }
    }
}
