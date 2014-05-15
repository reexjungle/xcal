using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.validators.concretes
{
    public class TimeZoneValidator: AbstractValidator<VTIMEZONE>
    {
        public TimeZoneValidator(): base()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
            RuleFor(x => x.Url).SetValidator(new UriValidator()).When(x => x.Url != null);
            RuleFor(x => x.StandardTimes).SetCollectionValidator(new ObservanceValidator()).
                Must((x, y) => y.AreUnique()).
                When(x => !x.StandardTimes.NullOrEmpty());
            RuleFor(x => x.DaylightTimes).SetCollectionValidator(new ObservanceValidator()).
                Must((x, y) => y.AreUnique()).
                When(x => !x.DaylightTimes.NullOrEmpty());
        }
    }

    public class EventValidator: AbstractValidator<VEVENT>
    {
        public EventValidator(): base()
        {
            RuleFor(x => x.Url).SetValidator(new UriValidator()).When(x => x.Url != null);
            RuleFor(x => x.Location).SetValidator(new TextValidator()).When(x => x.Location != null);
            RuleFor(x => x.Description).SetValidator(new TextValidator()).When(x => x.Description != null);
            RuleFor(x => x.Summary).SetValidator(new TextValidator()).When(x => x.Summary != null);
            RuleFor(x => x.RecurrenceRule).SetValidator(new RecurrenceValidator()).When(x => x.RecurrenceRule != null);
            RuleFor(x => x.Comments).SetCollectionValidator(new TextValidator()).When(x => !x.Comments.NullOrEmpty());
            RuleFor(x => x.Organizer).SetValidator(new OrganizerValidator());
        }
    }

    public class AlarmValidator: AbstractValidator<IALARM>
    {
        public AlarmValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Trigger).NotNull().SetValidator(new TriggerValidator());
            RuleFor(x => x.Duration).NotNull().SetValidator(new DurationValidator()).Unless(x => x.Repeat < 0);
            RuleFor(x => x.Repeat).GreaterThanOrEqualTo(0).Unless(x => x.Duration == null);
        }
    }

    public class AudioAlarmValidator:AbstractValidator<AUDIO_ALARM>
    {
        public AudioAlarmValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.AttachmentBinary).NotNull().SetValidator(new AttachmentBinaryValidator()).When(x => x.AttachmentBinary != null);
            RuleFor(x => x.AttachmentUri).NotNull().SetValidator(new AttachmentUriValidator()).When(x => x.AttachmentUri != null);
        }
    }    

    public class DisplayAlarmValidator:AbstractValidator<DISPLAY_ALARM>
    {
        public DisplayAlarmValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Description).NotNull().SetValidator(new TextValidator());
        }
    }

    public class EmailAlarmValidator:AbstractValidator<EMAIL_ALARM>
    {
        public EmailAlarmValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Description).NotNull().SetValidator(new TextValidator());
            RuleFor(x => x.Summary).NotNull().SetValidator(new TextValidator());
            RuleFor(x => x.Attendees).NotNull().NotEmpty().SetCollectionValidator(new AttendeeValidator());
            RuleFor(x => x.AttachmentBinaries).SetCollectionValidator(new AttachmentBinaryValidator()).When(x => !x.AttachmentBinaries.NullOrEmpty());
            RuleFor(x => x.AttachmentUris).SetCollectionValidator(new AttachmentUriValidator()).When(x => !x.AttachmentUris.NullOrEmpty());
        }
    }

}
