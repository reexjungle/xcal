using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.crosscut.operations.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.validators.concretes
{

    public class TextValidator: AbstractValidator<TEXTUAL>
    {
        public TextValidator() : base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Text).Must((x,y) =>!string.IsNullOrEmpty(y));
            RuleFor(x => x.AlternativeText).SetValidator(new UriValidator()).When(x => x.AlternativeText != null);
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class RecurrenceIdValidator: AbstractValidator<RECURRENCE_ID>
    {
        public RecurrenceIdValidator(): base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).SetValidator(new DateTimeValidator());
            RuleFor(x => x.Range).NotEqual(RANGE.UNKNOWN);
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
        }
    }


    public class OrganizerValidator: AbstractValidator<ORGANIZER>
    {
        public OrganizerValidator(): base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Address).SetValidator(new UriValidator()).When(x => x.Address != null);
            RuleFor(x => x.CN).Must((x, y) => !string.IsNullOrEmpty(y));
            RuleFor(x => x.Directory).SetValidator(new UriValidator()).When(x => x.Directory != null);
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
            RuleFor(x => x.SentBy).SetValidator(new UriValidator()).When(x => x.SentBy != null);

        }
    }

    public class AttendeeValidator : AbstractValidator<ATTENDEE>
    {
        public AttendeeValidator()
            : base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Address).SetValidator(new UriValidator()).When(x => x.Address != null);
            RuleFor(x => x.CN).Must((x, y) => !string.IsNullOrEmpty(y));
            RuleFor(x => x.Directory).SetValidator(new UriValidator()).When(x => x.Directory != null);
            RuleFor(x => x.Delegatee).SetValidator(new DelegateValidator()).When(x => x.Delegatee != null);
            RuleFor(x => x.Delegator).SetValidator(new DelegateValidator()).When(x => x.Delegator != null);
            RuleFor(x => x.CalendarUserType).NotEqual(CUTYPE.UNKNOWN);
            RuleFor(x => x.Participation).NotEqual(PARTSTAT.UNKNOWN);
            RuleFor(x => x.Role).NotEqual(ROLE.UNKNOWN);
            RuleFor(x => x.Member).SetValidator(new MemberValidator()).When(x => x.Member != null);
            RuleFor(x => x.Rsvp).NotEqual(BOOLEAN.UNKNOWN);
            RuleFor(x => x.SentBy).SetValidator(new UriValidator()).When(x => x.SentBy != null);
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public abstract class AttachmentBaseValidator<T> : AbstractValidator<T>
        where T : IATTACH
    {
        public AttachmentBaseValidator()
            : base()
        {
            RuleFor(x => x.FormatType)
                .Must((x, y) => !string.IsNullOrEmpty(y.TypeName) && !string.IsNullOrEmpty(y.SubTypeName))
                .When(x => x.FormatType != null);
        }
    }

    public class AttachmentValidator : AttachmentBaseValidator<IATTACH>
    {
        public AttachmentValidator()
            : base()
        {
            RuleFor(x => x.FormatType).SetValidator(new FormatTypeValidator()).When(x => x.FormatType != null);
        }
    }

    public class AttachmentBinaryValidator : AttachmentBaseValidator<ATTACH_BINARY>
    {
        public AttachmentBinaryValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Content).NotNull().SetValidator(new BinaryValidator());
        }
    }

    public class AttachmentUriValidator : AttachmentBaseValidator<ATTACH_URI>
    {
        public AttachmentUriValidator(): base()
        {
            RuleFor(x => x.Content).NotNull().SetValidator(new UriValidator());
        }
    }

    public class ExceptionDateValidator: AbstractValidator<EXDATE>
    {
        public ExceptionDateValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.DateTimes).Must((x,y) => !y.NullOrEmpty());
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
            RuleFor(x => x.ValueType).Must((x, y) => x.ValueType == VALUE.DATE_TIME || x.ValueType == VALUE.DATE);
        }
    }

    public class RecurrenceDateValidator : AbstractValidator<RDATE>
    {
        public RecurrenceDateValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.DateTimes).SetCollectionValidator(new DateTimeValidator()).When(x => !x.DateTimes.NullOrEmpty());
            RuleFor(x => x.Periods).SetCollectionValidator(new PeriodValidator()).When(x => !x.Periods.NullOrEmpty());
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
            RuleFor(x => x.ValueType).Must((x, y) => x.ValueType == VALUE.DATE_TIME || x.ValueType == VALUE.DATE);
        }
    }

    public class PriorityValidator : AbstractValidator<PRIORITY>
    {
        public PriorityValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).InclusiveBetween(0,9).When(x => x.Format == PriorityType.Integral);
            RuleFor(x => x.Level).NotEqual(PRIORITYLEVEL.UNKNOWN).When(x => x.Format == PriorityType.Level);
            RuleFor(x => x.Schema).NotEqual(PRIORITYSCHEMA.UNKNOWN).When(x => x.Format == PriorityType.Schema);

        }
    }

    public class RelatedToValidator: AbstractValidator<RELATEDTO>
    {
        public RelatedToValidator()
        {
            RuleFor(x => x.RelationshipType).NotEqual(RELTYPE.UNKNOWN);
        }
    }

    public class ResourcesValidator: AbstractValidator<RESOURCES>
    {
        public ResourcesValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Values).NotNull();
            RuleFor(x => x.AlternativeText).SetValidator(new UriValidator()).When(x => x.AlternativeText != null);
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class TimeZoneNameValidator: AbstractValidator<TZNAME>
    {
        public TimeZoneNameValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Text).NotNull().NotEmpty();
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class ObservanceValidator: AbstractValidator<OBSERVANCE>
    {
        public ObservanceValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Start).SetValidator(new DateTimeValidator());
            RuleFor(x => x.TimeZoneOffsetFrom).SetValidator(new UtcOffsetValidator()).When(x => x.TimeZoneOffsetFrom != null);
            RuleFor(x => x.TimeZoneOffsetTo).SetValidator(new UtcOffsetValidator()).When(x => x.TimeZoneOffsetTo != null);
            RuleFor(x => x.RecurrenceRule).SetValidator(new RecurrenceValidator()).When(x => x.RecurrenceRule != null);
            RuleFor(x => x.RecurrenceDates).SetCollectionValidator(new RecurrenceDateValidator()).
                Must((x, y) => y.AreUnique()).
                When(x => x.RecurrenceRule != null && !x.RecurrenceDates.NullOrEmpty());
            RuleFor(x => x.Comments).SetCollectionValidator(new TextValidator()).
                Must((x, y) => y.AreUnique()).
                When(x => !x.Comments.NullOrEmpty());
            RuleFor(x => x.TimeZoneNames).SetCollectionValidator(new TimeZoneNameValidator()).
                Must((x, y) => x.TimeZoneNames.AreUnique()).
                When(x => !x.TimeZoneNames.NullOrEmpty());
        }
    }

    public class TriggerValidator: AbstractValidator<TRIGGER>
    {
        public TriggerValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Duration).SetValidator(new DurationValidator()).When(x => x.Duration != null);
            RuleFor(x => x.DateTime).SetValidator(new DateTimeValidator()).When(x => x.DateTime != null);
            RuleFor(x => x.ValueType).NotEqual(VALUE.UNKNOWN);
        }
    }

}
