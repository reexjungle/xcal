using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;

namespace reexjungle.xcal.service.validators.concretes
{
    public class TextValidator : AbstractValidator<TEXTUAL>
    {
        public TextValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Text).Must((x, y) => !string.IsNullOrWhiteSpace(y));
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class RecurrenceIdValidator : AbstractValidator<RECURRENCE_ID>
    {
        public RecurrenceIdValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).SetValidator(new DateTimeValidator());
            RuleFor(x => x.Range).NotEqual(RANGE.NONE);
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
        }
    }

    public class OrganizerValidator : AbstractValidator<ORGANIZER>
    {
        public OrganizerValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CN).Must((x, y) => !string.IsNullOrEmpty(y));
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class AttendeeValidator : AbstractValidator<ATTENDEE>
    {
        public AttendeeValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CN).Must((x, y) => !string.IsNullOrEmpty(y));
            RuleFor(x => x.Delegatee).SetValidator(new DelegateValidator()).When(x => x.Delegatee != null);
            RuleFor(x => x.Delegator).SetValidator(new DelegateValidator()).When(x => x.Delegator != null);
            RuleFor(x => x.CalendarUserType).NotEqual(CUTYPE.UNKNOWN);
            RuleFor(x => x.Participation).NotEqual(PARTSTAT.NONE);
            RuleFor(x => x.Role).NotEqual(ROLE.NONE);
            RuleFor(x => x.Member).SetValidator(new MemberValidator()).When(x => x.Member != null);
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public abstract class AttachmentBaseValidator<T> : AbstractValidator<T>
        where T : IATTACH
    {
        protected AttachmentBaseValidator()
        {
            RuleFor(x => x.FormatType)
                .Must((x, y) => !string.IsNullOrEmpty(y.TypeName) && !string.IsNullOrEmpty(y.SubTypeName))
                .When(x => x.FormatType != null);
        }
    }

    public class CategoriesValidator : AbstractValidator<CATEGORIES>
    {
        public CategoriesValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
            RuleFor(x => x.Values).NotNull().NotEmpty();
        }
    }

    public class AttachmentValidator : AttachmentBaseValidator<IATTACH>
    {
        public AttachmentValidator()
        {
            RuleFor(x => x.FormatType).SetValidator(new FormatTypeValidator()).When(x => x.FormatType != null);
        }
    }

    public class AttachmentBinaryValidator : AttachmentBaseValidator<ATTACH_BINARY>
    {
        public AttachmentBinaryValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Content).NotNull().SetValidator(new BinaryValidator());
        }
    }

    public class AttachmentUriValidator : AttachmentBaseValidator<ATTACH_URI>
    {
        public AttachmentUriValidator()
        {
        }
    }

    public class ExceptionDateValidator : AbstractValidator<EXDATE>
    {
        public ExceptionDateValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.DateTimes).Must((x, y) => !y.NullOrEmpty());
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
        }
    }

    public class GeoValidator : AbstractValidator<GEO>
    {
        public GeoValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Latitude).InclusiveBetween(0f, 90f);
            RuleFor(x => x.Longitude).InclusiveBetween(-180f, 180f);
        }
    }

    public class RecurrenceDateValidator : AbstractValidator<RDATE>
    {
        public RecurrenceDateValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
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
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).InclusiveBetween(0, 9).When(x => x.Format == PriorityType.Integral);
            RuleFor(x => x.Level).NotEqual(PRIORITYLEVEL.NONE).When(x => x.Format == PriorityType.Level);
            RuleFor(x => x.Schema).NotEqual(PRIORITYSCHEMA.NONE).When(x => x.Format == PriorityType.Schema);
        }
    }

    public class RelatedToValidator : AbstractValidator<RELATEDTO>
    {
        public RelatedToValidator()
        {
            RuleFor(x => x.RelationshipType).NotEqual(RELTYPE.NONE);
        }
    }

    public class ResourcesValidator : AbstractValidator<RESOURCES>
    {
        public ResourcesValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Values).NotNull();
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class TimeZoneNameValidator : AbstractValidator<TZNAME>
    {
        public TimeZoneNameValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Text).NotNull().NotEmpty();
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class ObservanceValidator : AbstractValidator<OBSERVANCE>
    {
        public ObservanceValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Start).SetValidator(new DateTimeValidator());
            RuleFor(x => x.TimeZoneOffsetFrom).SetValidator(new UtcOffsetValidator()).When(x => x.TimeZoneOffsetFrom != default(UTC_OFFSET));
            RuleFor(x => x.TimeZoneOffsetTo).SetValidator(new UtcOffsetValidator()).When(x => x.TimeZoneOffsetTo != default(UTC_OFFSET));
            RuleFor(x => x.RecurrenceRule).SetValidator(new RecurrenceValidator()).When(x => x.RecurrenceRule != null);
            RuleFor(x => x.RecurrenceDates).SetCollectionValidator(new RecurrenceDateValidator()).
                Must((x, y) => y.IsSet()).
                When(x => x.RecurrenceRule != null && !x.RecurrenceDates.NullOrEmpty());
            RuleFor(x => x.Comments).SetCollectionValidator(new TextValidator()).
                Must((x, y) => y.IsSet()).
                When(x => !x.Comments.NullOrEmpty());
            RuleFor(x => x.TimeZoneNames).SetCollectionValidator(new TimeZoneNameValidator()).
                Must((x, y) => x.TimeZoneNames.IsSet()).
                When(x => !x.TimeZoneNames.NullOrEmpty());
        }
    }

    public class TriggerValidator : AbstractValidator<TRIGGER>
    {
        public TriggerValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.DateTime).SetValidator(new DateTimeValidator()).When(x => x.DateTime != default(DATE_TIME));
            RuleFor(x => x.ValueType).NotEqual(VALUE.NONE);
        }
    }
}