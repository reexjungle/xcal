using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;

namespace reexmonkey.xcal.service.plugins.validators.concretes
{
    public class PublishEventsValidator: AbstractValidator<PublishEvents>
    {
        PublishEventsValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.ProductId).NotNull().NotEmpty();
            RuleFor(x => x.Events).SetCollectionValidator(new PublishEventValidator());
        }
    }


    public class PublishEventValidator: AbstractValidator<VEVENT>
    {
        public PublishEventValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Datestamp).NotNull();
            RuleFor(x => x.Start).NotNull();
            RuleFor(x => x.Organizer).NotNull().SetValidator(new OrganizerValidator());
            RuleFor(x => x.Summary).NotNull().SetValidator(new TextValidator());
            RuleFor(x => x.Uid).NotNull().NotEmpty();
            RuleFor(x => x.RecurrenceId).NotNull().When(x => x.RecurrenceRule != null);
            RuleFor(x => x.Sequence).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Attachments.OfType<ATTACH_BINARY>()).SetCollectionValidator(new AttachmentBinaryValidator()).
                When(x => !x.Attachments.OfType<ATTACH_BINARY>().NullOrEmpty());
            RuleFor(x => x.Attachments.OfType<ATTACH_URI>()).SetCollectionValidator(new AttachmentUriValidator()).
                When(x => !x.Attachments.OfType<ATTACH_URI>().NullOrEmpty());
            RuleFor(x => x.Categories).NotNull().When(x => x.Categories != null);
            RuleFor(x => x.Classification).NotEqual(CLASS.UNKNOWN);
            RuleFor(x => x.Comments).SetCollectionValidator(new TextValidator()).
                Must((x,y) => x.Comments.Distinct().Count() ==  x.Comments.Count()).
                When(x => !x.Comments.NullOrEmpty());
            RuleFor(x => x.Contacts).SetCollectionValidator(new ContactValidator()).
                Must((x,y) => x.Contacts.Count() <= 1).
                When(x => !x.Contacts.NullOrEmpty());
            RuleFor(x => x.Description).SetValidator(new TextValidator()).When(x => x.Description != null);
            RuleFor(x => x.End).NotNull().Unless(x => x.Duration != null);
            RuleFor(x => x.Duration).NotNull().Unless(x => x.End != null);
            RuleFor(x => x.ExceptionDates).SetCollectionValidator(new ExceptionDateValidator()).When(x => !x.ExceptionDates.NullOrEmpty());
            RuleFor(x => x.Location).SetValidator(new TextValidator()).When(x => x.Location != null);
            RuleFor(x => x.Priority).SetValidator(new PriorityValidator()).When( x => x.Priority != null);

        }
    }

}
