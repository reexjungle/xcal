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

    public class ContactValidator: AbstractValidator<ICONTACT>
    {
        public ContactValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Value).NotNull().NotEmpty();
            RuleFor(x => x.AlternativeText).SetValidator(new AltrepValidator()).When(x => x.AlternativeText != null);
            RuleFor(x => x.Language).SetValidator(new TextValidator()).When(x => x.Language != null);
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
            RuleFor(x => x.TimeZoneId).SetValidator(new TzIdValidator()).When(x => x.TimeZoneId != null);
            RuleFor(x => x.Format).Must((x, y) => x.Format == ValueFormat.DATE_TIME || x.Format == ValueFormat.DATE);
        }
    }

    public class RecurrenceDateValidator : AbstractValidator<IRDATE>
    {
        public RecurrenceDateValidator()
        {
            RuleFor(x => x.DateTimes).NotEmpty();
            RuleFor(x => x.Periods).SetCollectionValidator(new PeriodValidator());
            RuleFor(x => x.TimeZoneId).SetValidator(new TzIdValidator()).When(x => x.TimeZoneId != null);
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
            RuleFor(x => x.Language).SetValidator(new TextValidator()).When(x => x.Language != null);
        }
    }
   

}
