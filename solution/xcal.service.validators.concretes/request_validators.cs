using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;

namespace reexmonkey.xcal.service.validators.concretes
{
    public class PublishEventsValidator: AbstractValidator<PublishEvents>
    {
        public PublishEventsValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.ProductId).NotNull().NotEmpty();
            RuleFor(x => x.Events).NotNull().NotEmpty().SetCollectionValidator(new PublishEventValidator());
            RuleFor(x => x.TimeZones).SetCollectionValidator(new TimeZoneValidator()).
                Must((x, y) => y.OfType<VTIMEZONE>().AreUnique(new EqualByStringId<VTIMEZONE>())).
                When(x => !x.TimeZones.NullOrEmpty());
        }
    }

    public class PublishEventValidator: AbstractValidator<IEVENT>
    {
        public PublishEventValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Datestamp).NotNull();
            RuleFor(x => x.Start).NotNull();
            RuleFor(x => x.Organizer).NotNull().SetValidator(new OrganizerValidator());
            RuleFor(x => x.Summary).NotNull().SetValidator(new TextValidator());
            RuleFor(x => x.Uid).NotNull().NotEmpty();
            RuleFor(x => x.RecurrenceId).SetValidator(new RecurrenceIdValidator()).When(x => x.RecurrenceId != null && x.RecurrenceRule != null);
            RuleFor(x => x.RecurrenceRule).SetValidator(new RecurrenceValidator()).When(x => x.RecurrenceRule != null);
            RuleFor(x => x.Sequence).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Status).Must((x, y) => y == STATUS.TENTATIVE || y == STATUS.CONFIRMED || y == STATUS.CANCELLED);
            RuleFor(x => x.Url).SetValidator(new UriValidator()).When(x => x.Url != null);
            
            RuleFor(x => x.Attachments.OfType<ATTACH_BINARY>()).SetCollectionValidator(new AttachmentBinaryValidator()).
                Must((x, y) => x.Attachments.OfType<ATTACH_BINARY>().AreUnique(new EqualByStringId<ATTACH_BINARY>())).
                When(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty());

            RuleFor(x => x.Attachments.OfType<ATTACH_URI>()).SetCollectionValidator(new AttachmentUriValidator()).
                Must((x,y) => x.Attachments.OfType<ATTACH_URI>().AreUnique(new EqualByStringId<ATTACH_URI>())).
                When(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty());

            RuleFor(x => x.Categories).NotNull().When(x => x.Categories != null);
            RuleFor(x => x.Classification).NotEqual(CLASS.UNKNOWN);
            
            RuleFor(x => x.Comments.OfType<COMMENT>()).SetCollectionValidator(new TextValidator()).
                Must((x,y) => x.Comments.OfType<COMMENT>().AreUnique(new EqualByStringId<COMMENT>())).
                When(x => !x.Comments.OfType<COMMENT>().NullOrEmpty());
            
            RuleFor(x => x.Contacts.OfType<CONTACT>()).SetCollectionValidator(new ContactValidator()).
                Must((x, y) => x.Contacts.OfType<CONTACT>().AreUnique(new EqualByStringId<CONTACT>()) && 
                    x.Contacts.OfType<CONTACT>().Count() <= 1).
                When(x => !x.Contacts.OfType<CONTACT>().NullOrEmpty());
            
            RuleFor(x => x.Description).SetValidator(new TextValidator()).When(x => x.Description != null);
            RuleFor(x => x.End).NotNull().Unless(x => x.Duration != null);
            RuleFor(x => x.Duration).NotNull().Unless(x => x.End != null);
            
            RuleFor(x => x.ExceptionDates).SetCollectionValidator(new ExceptionDateValidator()).
                Must((x,y) => x.ExceptionDates.OfType<EXDATE>().AreUnique(new EqualByStringId<EXDATE>())).
                When(x => !x.ExceptionDates.NullOrEmpty());
            
            RuleFor(x => x.Location).SetValidator(new TextValidator()).When(x => x.Location != null);
            RuleFor(x => x.Priority).SetValidator(new PriorityValidator()).When( x => x.Priority != null);
            RuleFor(x => x.RelatedTos).SetCollectionValidator(new RelatedToValidator()).
                Must((x,y) => x.RelatedTos.OfType<RELATEDTO>().AreUnique(new EqualByStringId<RELATEDTO>())).
                When(x => !x.RelatedTos.NullOrEmpty());

            RuleFor(x => x.Resources).SetCollectionValidator(new ResourcesValidator()).
                Must((x, y) => x.Resources.OfType<RESOURCES>().AreUnique(new EqualByStringId<RESOURCES>())).
                When(x => !x.Resources.NullOrEmpty());

        }
    }

    public class RescheduleEventsValidator: AbstractValidator<RescheduleEvents>
    {
        public RescheduleEventsValidator()
            : base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Events).NotNull().NotEmpty();
            RuleFor(x => x.Events).Must((x, y) => y.Select(z => z.Uid)
                .Distinct()
                .Skip(1).Any())
                .When(x => !x.Events.NullOrEmpty() && x.Events.Count() > 1);

        }
    }

    public class RescheduleEventValidator: AbstractValidator<IEVENT>
    {
        public RescheduleEventValidator(): base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Datestamp).NotNull();
            RuleFor(x => x.Start).NotNull();
            RuleFor(x => x.Organizer).NotNull().NotEmpty().SetValidator(new OrganizerValidator());
            RuleFor(x => x.Summary).SetValidator(new TextValidator()).When(x => x.Summary != null);
            RuleFor(x => x.Uid).NotNull().Must((x, y) => !x.Uid.Equals(string.Empty, StringComparison.OrdinalIgnoreCase));
            RuleFor(x => x.RecurrenceId).SetValidator(new RecurrenceIdValidator()).When(x => x.RecurrenceId != null && x.RecurrenceRule != null);
            RuleFor(x => x.RecurrenceRule).SetValidator(new RecurrenceValidator()).When(x => x.RecurrenceRule != null);
            RuleFor(x => x.Sequence).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Status).Must((x, y) => y == STATUS.TENTATIVE || y == STATUS.CONFIRMED || y == STATUS.CANCELLED);
            RuleFor(x => x.Url).SetValidator(new UriValidator()).When(x => x.Url != null);

            RuleFor(x => x.Attachments.OfType<ATTACH_BINARY>()).SetCollectionValidator(new AttachmentBinaryValidator()).
                Must((x, y) => x.Attachments.OfType<ATTACH_BINARY>().AreUnique(new EqualByStringId<ATTACH_BINARY>())).
                When(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty());

            RuleFor(x => x.Attachments.OfType<ATTACH_URI>()).SetCollectionValidator(new AttachmentUriValidator()).
                Must((x, y) => x.Attachments.OfType<ATTACH_URI>().AreUnique(new EqualByStringId<ATTACH_URI>())).
                When(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty());

            RuleFor(x => x.Categories).NotNull().When(x => x.Categories != null);
            RuleFor(x => x.Classification).NotEqual(CLASS.UNKNOWN);

            RuleFor(x => x.Comments.OfType<COMMENT>()).SetCollectionValidator(new TextValidator()).
                Must((x, y) => x.Comments.OfType<COMMENT>().AreUnique(new EqualByStringId<COMMENT>())).
                When(x => !x.Comments.OfType<COMMENT>().NullOrEmpty());

            RuleFor(x => x.Contacts.OfType<CONTACT>()).SetCollectionValidator(new ContactValidator()).
                Must((x, y) => x.Contacts.OfType<CONTACT>().AreUnique(new EqualByStringId<CONTACT>()) &&
                    x.Contacts.OfType<CONTACT>().Count() <= 1).
                When(x => !x.Contacts.OfType<CONTACT>().NullOrEmpty());

            RuleFor(x => x.Description).SetValidator(new TextValidator()).When(x => x.Description != null);
            RuleFor(x => x.End).NotNull().Unless(x => x.Duration != null);
            RuleFor(x => x.Duration).NotNull().Unless(x => x.End != null);

            RuleFor(x => x.ExceptionDates).SetCollectionValidator(new ExceptionDateValidator()).
                Must((x, y) => x.ExceptionDates.OfType<EXDATE>().AreUnique(new EqualByStringId<EXDATE>())).
                When(x => !x.ExceptionDates.NullOrEmpty());

            RuleFor(x => x.Location).SetValidator(new TextValidator()).When(x => x.Location != null);
            RuleFor(x => x.Priority).SetValidator(new PriorityValidator()).When(x => x.Priority != null);
            RuleFor(x => x.RelatedTos).SetCollectionValidator(new RelatedToValidator()).
                Must((x, y) => x.RelatedTos.OfType<RELATEDTO>().AreUnique(new EqualByStringId<RELATEDTO>())).
                When(x => !x.RelatedTos.NullOrEmpty());

            RuleFor(x => x.Resources).SetCollectionValidator(new ResourcesValidator()).
                Must((x, y) => x.Resources.OfType<RESOURCES>().AreUnique(new EqualByStringId<RESOURCES>())).
                When(x => !x.Resources.NullOrEmpty());
        }
    }


}
