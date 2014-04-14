using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.crosscut.goodies.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;

namespace reexmonkey.xcal.service.validators.concretes
{
    public class PublishEventValidator: AbstractValidator<PublishEvent>
    {
        public PublishEventValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.ProductId).NotNull().NotEmpty();
            RuleFor(x => x.Events).Must((x, y) => !x.Events.NullOrEmpty()).WithMessage("Events MUST NOT NEITHER be null NOR empty");
            RuleFor(x => x.Events).SetCollectionValidator(new PublishedEventValidator());
            RuleFor(x => x.TimeZones).Must((x, y) => !x.TimeZones.NullOrEmpty()).When(x => !x.Events.NullOrEmpty() && x.Events.FirstOrDefault().Datestamp.TimeFormat == TimeFormat.LocalAndTimeZone);
            RuleFor(x => x.TimeZones).SetCollectionValidator(new TimeZoneValidator()).
                Must((x, y) => y.OfType<VTIMEZONE>().AreUnique(new EqualByStringId<VTIMEZONE>())).
                When(x => !x.TimeZones.NullOrEmpty());
        }
    }

    public class PublishedEventValidator: AbstractValidator<IEVENT>
    {
        public PublishedEventValidator()
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

            RuleFor(x => x.Attachments).SetValidator(
                new PolymorphicCollectionValidator<IATTACH>()
                .Add<ATTACH_BINARY>(new AttachmentBinaryValidator())
                .Add<ATTACH_URI>(new AttachmentUriValidator()))
                .Must((x, y) => x.Attachments.AreUnique())
                .When(x => !x.Attachments.NullOrEmpty());

            RuleFor(x => x.Categories).NotNull().When(x => x.Categories != null);
            RuleFor(x => x.Classification).NotEqual(CLASS.UNKNOWN);

            RuleFor(x => x.Comments).SetCollectionValidator(new TextValidator()).
                Must((x, y) => x.Comments.AreUnique()).
                When(x => !x.Comments.NullOrEmpty());

            RuleFor(x => x.Contacts).SetCollectionValidator(new ContactValidator()).
                Must((x, y) => x.Contacts.AreUnique() && x.Contacts.Count() <= 1).
                When(x => !x.Contacts.NullOrEmpty());

            RuleFor(x => x.Description).SetValidator(new TextValidator()).When(x => x.Description != null);
            RuleFor(x => x.End).NotNull().Unless(x => x.Duration != null);
            RuleFor(x => x.Duration).NotNull().Unless(x => x.End != null);

            RuleFor(x => x.ExceptionDates).SetCollectionValidator(new ExceptionDateValidator()).
                Must((x, y) => x.ExceptionDates.AreUnique()).
                When(x => !x.ExceptionDates.NullOrEmpty());

            RuleFor(x => x.Location).SetValidator(new TextValidator()).When(x => x.Location != null);
            RuleFor(x => x.Priority).SetValidator(new PriorityValidator()).When(x => x.Priority != null);
            RuleFor(x => x.RelatedTos).SetCollectionValidator(new RelatedToValidator()).Must((x, y) => x.RelatedTos.AreUnique()).
                When(x => !x.RelatedTos.NullOrEmpty());

            RuleFor(x => x.Resources).SetCollectionValidator(new ResourcesValidator()).
                Must((x, y) => x.Resources.AreUnique()).
                When(x => !x.Resources.NullOrEmpty());

            RuleFor(x => x.Attendees).Must((x, y) => x.Attendees.NullOrEmpty());
            RuleFor(x => x.RequestStatuses).Must((x, y) => x.RequestStatuses.NullOrEmpty());

        }
    }

    public class RescheduleEventValidator: AbstractValidator<RescheduleEvent>
    {
        public RescheduleEventValidator()
            : base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Events).NotNull().NotEmpty();
            RuleFor(x => x.Events).Must((x, y) => y.Select(z => z.Uid)
                .Distinct()
                .Skip(1).Any())
                .When(x => !x.Events.NullOrEmpty() && x.Events.Count() > 1);
            RuleFor(x => x.Events).NotNull().NotEmpty().SetCollectionValidator(new RequestedEventValidator());
            RuleFor(x => x.TimeZones).SetCollectionValidator(new TimeZoneValidator()).
                Must((x, y) => y.OfType<VTIMEZONE>().AreUnique(new EqualByStringId<VTIMEZONE>())).
                When(x => !x.TimeZones.NullOrEmpty());
        }
    }

    public class RequestedEventValidator: AbstractValidator<IEVENT>
    {
        public RequestedEventValidator(): base()
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
