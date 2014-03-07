using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.validators.concretes
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
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
        }
    }

    public class ContactValidator: AbstractValidator<ICONTACT>
    {
        public ContactValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().NotEmpty();
            RuleFor(x => x.AlternativeText).SetValidator(new AltrepValidator()).When(x => x.AlternativeText != null);
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class OrganizerValidator: AbstractValidator<IORGANIZER>
    {
        public OrganizerValidator(): base()
        {
            RuleFor(x => x.Address).NotNull().When(x => x != null);
        }
    }

    public class AttendeeValidator : AbstractValidator<IATTENDEE>
    {
        public AttendeeValidator()
            : base()
        {
            RuleFor(x => x.Address).NotNull().When(x => x != null);
        }
    }

    public class AttachmentBinaryValidator: AbstractValidator<ATTACH_BINARY>
    {
        public AttachmentBinaryValidator()
        {

        }
    }

    public class AttachmentUriValidator : AbstractValidator<ATTACH_URI>
    {
        public AttachmentUriValidator()
        {

        }
    }

    public class ExceptionDateValidator: AbstractValidator<IEXDATE>
    {
        public ExceptionDateValidator()
        {
            RuleFor(x => x.DateTimes).NotEmpty();
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
            RuleFor(x => x.Format).Must((x, y) => x.Format == ValueFormat.DATE_TIME || x.Format == ValueFormat.DATE);
        }
    }

    public class RecurrenceDateValidator : AbstractValidator<IRDATE>
    {
        public RecurrenceDateValidator()
        {
            RuleFor(x => x.DateTimes).NotEmpty();
            RuleFor(x => x.Periods).SetCollectionValidator(new PeriodValidator());
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
            RuleFor(x => x.Format).Must((x, y) => x.Format == ValueFormat.DATE_TIME || x.Format == ValueFormat.DATE);
        }
    }

    public class PriorityValidator : AbstractValidator<IPRIORITY>
    {
        public PriorityValidator()
        {
            RuleFor(x => x.Value).InclusiveBetween(0,9).When(x => x.Format == PriorityFormat.Integral);
            RuleFor(x => x.Level).NotEqual(PRIORITYLEVEL.UNKNOWN).When(x => x.Format == PriorityFormat.Level);
            RuleFor(x => x.Schema).NotEqual(PRIORITYSCHEMA.UNKNOWN).When(x => x.Format == PriorityFormat.Schema);

        }
    }

    public class RelatedToValidator: AbstractValidator<IRELATEDTO>
    {
        public RelatedToValidator()
        {
            RuleFor(x => x.Reference).NotNull();
            RuleFor(x => x.RelationshipType).NotEqual(RELTYPE.UNKNOWN);
        }
    }

    public class ResourcesValidator: AbstractValidator<IRESOURCES>
    {
        public ResourcesValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Values).NotNull().NotEmpty();
            RuleFor(x => x.AlternativeText).SetValidator(new AltrepValidator()).When(x => x.AlternativeText != null);
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class TimeZoneNameValidator: AbstractValidator<ITZNAME>
    {
        public TimeZoneNameValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Text).NotNull().NotEmpty();
            RuleFor(x => x.Language).SetValidator(new LanguageValidator()).When(x => x.Language != null);
        }
    }

    public class ObservanceValidator: AbstractValidator<IObservance>
    {
        public ObservanceValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.StartDate).NotNull();
            RuleFor(x => x.TimeZoneOffsetFrom).SetValidator(new UtcOffsetValidator()).When(x => x.TimeZoneOffsetFrom != null);
            RuleFor(x => x.TimeZoneOffsetTo).SetValidator(new UtcOffsetValidator()).When(x => x.TimeZoneOffsetTo != null);
            RuleFor(x => x.RecurrenceRule).SetValidator(new RecurrenceValidator()).When(x => x.RecurrenceRule != null);
            RuleFor(x => x.RecurrenceDates).SetCollectionValidator(new RecurrenceDateValidator()).
                Must((x, y) => y.OfType<RDATE>().AreUnique(new EqualByStringId<RDATE>())).
                When(x => x.RecurrenceRule != null && !x.RecurrenceDates.NullOrEmpty());
            RuleFor(x => x.Comments).SetCollectionValidator(new TextValidator()).
                Must((x, y) => y.OfType<COMMENT>().AreUnique(new EqualByStringId<COMMENT>())).
                When(x => !x.Comments.NullOrEmpty());
            RuleFor(x => x.Names).SetCollectionValidator(new TimeZoneNameValidator()).
                Must((x, y) => x.Names.OfType<TZNAME>().AreUnique(new EqualByStringId<TZNAME>())).
                When(x => !x.Names.NullOrEmpty());
            
        }
    }

}
