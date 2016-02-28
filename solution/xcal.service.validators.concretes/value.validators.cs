using System;
using System.Linq;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;

namespace reexjungle.xcal.service.validators.concretes
{
    public class BinaryValidator : AbstractValidator<BINARY>
    {
        public BinaryValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.Value).Must((x, value) => !string.IsNullOrEmpty(value));
        }
    }

    public class DateValidator : AbstractValidator<DATE>
    {
        public DateValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.FULLYEAR).InclusiveBetween(1u, 10000u);
            RuleFor(x => x.MONTH).InclusiveBetween(1u, 12u);
            RuleFor(x => x.MDAY).InclusiveBetween(1u, 31u);
        }
    }

    public class DateTimeValidator : AbstractValidator<DATE_TIME>
    {
        public DateTimeValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.FULLYEAR).InclusiveBetween(1u, 10000u);
            RuleFor(x => x.MONTH).InclusiveBetween(1u, 12u);
            RuleFor(x => x.MDAY).InclusiveBetween(1u, 31u);
            RuleFor(x => x.HOUR).InclusiveBetween(0u, 23u);
            RuleFor(x => x.MINUTE).InclusiveBetween(0u, 59u);
            RuleFor(x => x.SECOND).InclusiveBetween(0u, 60u);
            RuleFor(x => x.Type).NotEqual(TimeType.Utc).When(x => x.TimeZoneId != null);
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator())
                .When(x => x.TimeZoneId != null || x.Type == TimeType.Utc);
        }
    }

    public class TimeValidator : AbstractValidator<TIME>
    {
        public TimeValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.HOUR).InclusiveBetween(0u, 23u);
            RuleFor(x => x.MINUTE).InclusiveBetween(0u, 59u);
            RuleFor(x => x.SECOND).InclusiveBetween(0u, 60u);
            RuleFor(x => x.Type).NotEqual(TimeType.Utc).When(x => x.TimeZoneId != null);
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator())
                .When(x => x.TimeZoneId != null || x.Type == TimeType.Utc);
        }
    }


    public class PeriodValidator : AbstractValidator<PERIOD>
    {
        public PeriodValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            var validator = new DateTimeValidator();
            RuleFor(x => x.Start).SetValidator(validator)
                .When(x => x.Start != default(DATE_TIME));
            RuleFor(x => x.End).SetValidator(validator)
                .Unless(x => x.Duration != default(DURATION)).When(x => x.End != default(DATE_TIME));
        }
    }

    public class CalendarAddressValidator:AbstractValidator<CAL_ADDRESS>
    {
        public CalendarAddressValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.LocalPath).EmailAddress().When(x => x.IsAbsoluteUri);

        }
        
    }

    public class WeekDayNumValidator : AbstractValidator<WEEKDAYNUM>
    {
        public WeekDayNumValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.NthOccurrence).InclusiveBetween(1, 53).When(x => x.NthOccurrence != 0);
            RuleFor(x => x.Weekday).NotEqual(WEEKDAY.NONE);
        }
    }

    public class RecurrenceValidator : AbstractValidator<RECUR>
    {
        public RecurrenceValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.FREQ).NotEqual(FREQ.NONE);
            RuleFor(x => x.INTERVAL).GreaterThan(0u);
            RuleFor(x => x.BYSECOND).Must((x, y) => y.Max() <= 60u).When(x => !x.BYSECOND.NullOrEmpty());
            RuleFor(x => x.BYMINUTE).Must((x, y) => y.Max() <= 59u).When(x => !x.BYMINUTE.NullOrEmpty());
            RuleFor(x => x.BYHOUR).Must((x, y) => y.Max() <= 23u).When(x => !x.BYHOUR.NullOrEmpty());
            RuleFor(x => x.BYDAY).SetCollectionValidator(new WeekDayNumValidator()).When(x => !x.BYDAY.NullOrEmpty());
            RuleFor(x => x.BYMONTHDAY).Must((x, y) => y.Min() >= -31 && y.Max() <= 31).When(x => !x.BYMONTHDAY.NullOrEmpty());
            RuleFor(x => x.BYYEARDAY).Must((x, y) => y.Min() >= -366 && y.Max() <= 366).When(x => !x.BYYEARDAY.NullOrEmpty());
            RuleFor(x => x.BYWEEKNO).Must((x, y) => y.Min() >= -53 && y.Max() <= 53).When(x => !x.BYWEEKNO.NullOrEmpty());
            RuleFor(x => x.BYMONTH).Must((x, y) => y.Min() >= 1 && y.Max() <= 12).When(x => !x.BYMONTH.NullOrEmpty());
            RuleFor(x => x.WKST).NotEqual(WEEKDAY.NONE);
            RuleFor(x => x.BYSETPOS).Must((x, y) => y.Min() >= -366 && y.Max() <= 366).When(x => !x.BYSETPOS.NullOrEmpty());
        }
    }

    public class UtcOffsetValidator : AbstractValidator<UTC_OFFSET>
    {
        public UtcOffsetValidator(CascadeMode mode = CascadeMode.StopOnFirstFailure)
        {
            CascadeMode = mode;
            RuleFor(x => x.HOUR).InclusiveBetween(-23, 23);
            RuleFor(x => x.MINUTE).InclusiveBetween(-59, 59);
            RuleFor(x => x.SECOND).InclusiveBetween(-59, 59);
            RuleFor(x => x.HOUR).NotEqual(0).When(x => x.MINUTE == 0 && x.SECOND == 0);
            RuleFor(x => x.MINUTE).NotEqual(0).When(x => x.HOUR == 0 && x.SECOND == 0);
            RuleFor(x => x.SECOND).NotEqual(0).When(x => x.MINUTE == 0 && x.SECOND == 0);
        }
    }
}