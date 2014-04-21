using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack.FluentValidation;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.validators.concretes
{
    public class EmailAddressValidator : AbstractValidator<string>
    {
        public EmailAddressValidator()
        {
            Func<string, string, bool> matches = (x, y) =>
            {
                return Regex.IsMatch(x, y, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
            };

            var pattern = @"(?<value>(\w+)((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?))";

            RuleFor(x => x).Must(x => matches(x, pattern));

        }
    }

    public class BinaryValidator: AbstractValidator<BINARY>
    {
        public BinaryValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().NotEmpty();
            RuleFor(x => x.Encoding).NotEqual(ENCODING.UNKNOWN);
        }
    }

    public class DateValidator: AbstractValidator<DATE>
    {
        public DateValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.FULLYEAR).InclusiveBetween(1u, 10000u);
            RuleFor(x => x.MONTH).InclusiveBetween(1u, 12u);
            RuleFor(x => x.MDAY).InclusiveBetween(1u, 31u);
        }
    }

    public class DateTimeValidator: AbstractValidator<DATE_TIME>
    {
        public DateTimeValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.FULLYEAR).InclusiveBetween(1u, 10000u);
            RuleFor(x => x.MONTH).InclusiveBetween(1u, 12u);
            RuleFor(x => x.MDAY).InclusiveBetween(1u, 31u);
            RuleFor(x => x.HOUR).InclusiveBetween(0u, 23u);
            RuleFor(x => x.MINUTE).InclusiveBetween(0u, 59u);
            RuleFor(x => x.SECOND).InclusiveBetween(0u, 60u);
            RuleFor(x => x.TimeFormat).NotEqual(TimeFormat.Utc).When(x => x.TimeZoneId != null);
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null || x.TimeFormat == TimeFormat.Utc);
        }
    }

    public class TimeValidator: AbstractValidator<TIME>
    {
        public TimeValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.HOUR).InclusiveBetween(0u, 23u);
            RuleFor(x => x.MINUTE).InclusiveBetween(0u, 59u);
            RuleFor(x => x.SECOND).InclusiveBetween(0u, 60u);
            RuleFor(x => x.TimeFormat).NotEqual(TimeFormat.Utc).When(x => x.TimeZoneId != null);
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null || x.TimeFormat == TimeFormat.Utc);
        }
    }

    public class DurationValidator: AbstractValidator<DURATION>
    {
        public DurationValidator()
        {
            RuleFor(x => x.Sign).NotEqual(SignType.Neutral);
        }
    }

    public class PeriodValidator: AbstractValidator<PERIOD>
    {
        public PeriodValidator()
        {
            var dtvalidator = new DateTimeValidator();
            RuleFor(x => x.Start).SetValidator(dtvalidator).When(x => x.Start != null);
            RuleFor(x => x.End).SetValidator(dtvalidator).Unless(x => x.Duration != null).When(x => x.End != null);
            RuleFor(x => x.Duration).SetValidator(new DurationValidator()).Unless(x => x.End != null).When(x => x.Duration != null);
        }
    }

    public class WeekDayNumValidator: AbstractValidator<WEEKDAYNUM>
    {
        public WeekDayNumValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.OrdinalWeek).InclusiveBetween(1, 54);
            RuleFor(x => x.Weekday).NotEqual(WEEKDAY.UNKNOWN);
        }
    }

    public class UriValidator : AbstractValidator<URI>
    {
        public UriValidator()
            : base()
        {
            RuleFor(x => x).Must(x => Uri.IsWellFormedUriString(x.Path, UriKind.RelativeOrAbsolute)).When(x => x.Path != null);
        }
    }

    public class RecurrenceValidator: AbstractValidator<RECUR>
    {
        public RecurrenceValidator()
            : base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.FREQ).NotEqual(FREQ.UNKNOWN);
            RuleFor(x => x.UNTIL).NotNull().When(x => x != null && x.Format == RecurFormat.DateTime);
            RuleFor(x => x.UNTIL).Must((x, y) => x.UNTIL == null).When(x => x.Format == RecurFormat.Range);
            RuleFor(x => x.BYDAY).SetCollectionValidator(new WeekDayNumValidator()).When(x => !x.BYDAY.NullOrEmpty());
        }
    }

    public class UtcOffsetValidator : AbstractValidator<UTC_OFFSET>
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
