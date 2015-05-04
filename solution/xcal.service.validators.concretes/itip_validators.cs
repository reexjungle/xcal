using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;
using System.Linq;

namespace reexjungle.xcal.service.validators.concretes
{
    public class PublishEventsValidator : AbstractValidator<PublishEvents>
    {
        public PublishEventsValidator()
            : base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Method).Equal(METHOD.PUBLISH);
            RuleFor(x => x.Events).NotNull().NotEmpty().SetCollectionValidator(new PublishEventValidator()).When(x => !x.Events.NullOrEmpty());
            RuleFor(x => x.TimeZones).SetCollectionValidator(new TimeZoneValidator()).When(x => !x.TimeZones.NullOrEmpty());
            RuleFor(x => x.IANAComponents).SetCollectionValidator(new IANAComponentValidator()).When(x => !x.IANAComponents.NullOrEmpty());
            RuleFor(x => x.XComponents).SetCollectionValidator(new XComponentValidator()).When(x => !x.IANAComponents.NullOrEmpty());
        }

        private class PublishEventValidator : AbstractValidator<VEVENT>
        {
            public PublishEventValidator()
            {
                CascadeMode = CascadeMode.StopOnFirstFailure;
                RuleFor(x => x.Datestamp).NotEqual(default(DATE_TIME)).SetValidator(new DateTimeValidator());
                RuleFor(x => x.Start).NotEqual(default(DATE_TIME)).SetValidator(new DateTimeValidator());
                RuleFor(x => x.Organizer).NotNull().SetValidator(new OrganizerValidator());
                RuleFor(x => x.Summary).SetValidator(new TextValidator()).When(x => x.Summary != null);
                RuleFor(x => x.RecurrenceId).SetValidator(new RecurrenceIdValidator()).When(x => x.RecurrenceId != null);
                RuleFor(x => x.Sequence).GreaterThanOrEqualTo(1).When(x => x.Sequence > 0);
                RuleFor(x => x.AttachmentBinaries).SetCollectionValidator(new AttachmentBinaryValidator()).When(x => x.AttachmentBinaries.NullOrEmpty());
                RuleFor(x => x.AttachmentUris).SetCollectionValidator(new AttachmentUriValidator()).When(x => x.AttachmentUris.NullOrEmpty());
                RuleFor(x => x.Categories).SetValidator(new CategoriesValidator()).When(x => x.Categories != null);
                RuleFor(x => x.Comments).SetCollectionValidator(new TextValidator()).When(x => !x.Comments.NullOrEmpty());
                RuleFor(x => x.Contacts).Must((x, y) => y.Count() <= 1).SetCollectionValidator(new TextValidator()).When(x => !x.Contacts.NullOrEmpty());
                RuleFor(x => x.Created).SetValidator(new DateTimeValidator()).When(x => x.Created != default(DATE_TIME));
                RuleFor(x => x.Description).SetValidator(new TextValidator()).When(x => x.Description != null);
                RuleFor(x => x.End).SetValidator(new DateTimeValidator()).Must((x, y) => x.Duration == default(DURATION)).When(x => x.End != default(DATE_TIME));
                RuleFor(x => x.Duration).SetValidator(new DurationValidator()).Must((x, y) => x.End == default(DATE_TIME)).When(x => x.Duration != default(DURATION));
                RuleFor(x => x.ExceptionDates).SetCollectionValidator(new ExceptionDateValidator()).When(x => !x.ExceptionDates.NullOrEmpty());
                RuleFor(x => x.Position).SetValidator(new GeoValidator()).When(x => x.Position != default(GEO));
                RuleFor(x => x.LastModified).SetValidator(new DateTimeValidator()).When(x => x.LastModified != default(DATE_TIME));
                RuleFor(x => x.Location).SetValidator(new TextValidator()).When(x => x.Location != null);

                RuleFor(x => x.RecurrenceDates).SetCollectionValidator(new RecurrenceDateValidator()).When(x => !x.RecurrenceDates.NullOrEmpty());

                RuleFor(x => x.RelatedTos).SetCollectionValidator(new RelatedToValidator()).When(x => !x.RelatedTos.NullOrEmpty());
                RuleFor(x => x.Resources).SetCollectionValidator(new ResourcesValidator()).When(x => !x.Resources.NullOrEmpty());
                RuleFor(x => x.RecurrenceRule).SetValidator(new RecurrenceValidator()).When(x => x.RecurrenceRule != null);
                RuleFor(x => x.Status).Must((x, y) => y == STATUS.TENTATIVE || y == STATUS.CONFIRMED || y == STATUS.CANCELLED);
                RuleFor(x => x.IANAProperties.Values).SetCollectionValidator(new IANAPropertyValidator()).When(x => !x.IANAProperties.NullOrEmpty());
                RuleFor(x => x.XProperties.Values).SetCollectionValidator(new XPropertyValidator()).When(x => !x.XProperties.NullOrEmpty());
                RuleFor(x => x.Attendees).Must((x, y) => x.Attendees.NullOrEmpty());
                RuleFor(x => x.RequestStatuses).Must((x, y) => x.RequestStatuses.NullOrEmpty());
                RuleFor(x => x.AudioAlarms).SetCollectionValidator(new AudioAlarmValidator()).When(x => !x.AudioAlarms.NullOrEmpty());
                RuleFor(x => x.DisplayAlarms).SetCollectionValidator(new DisplayAlarmValidator()).When(x => !x.DisplayAlarms.NullOrEmpty());
                RuleFor(x => x.EmailAlarms).SetCollectionValidator(new EmailAlarmValidator()).When(x => !x.EmailAlarms.NullOrEmpty());
            }
        }
    }
}