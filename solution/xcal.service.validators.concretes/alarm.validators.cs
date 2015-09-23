using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;

namespace reexjungle.xcal.service.validators.concretes
{
    public class AlarmValidator : AbstractValidator<IALARM>
    {
        public AlarmValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Trigger).NotNull().SetValidator(new TriggerValidator());
            RuleFor(x => x.Duration).NotNull().SetValidator(new DurationValidator()).Unless(x => x.Repeat < 0);
            RuleFor(x => x.Repeat).GreaterThanOrEqualTo(0).Unless(x => x.Duration == default(DURATION));
        }
    }

    public class AudioAlarmValidator : AbstractValidator<AUDIO_ALARM>
    {
        public AudioAlarmValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.AttachmentBinary).NotNull().SetValidator(new AttachmentBinaryValidator()).When(x => x.AttachmentBinary != null);
            RuleFor(x => x.AttachmentUri).NotNull().SetValidator(new AttachmentUriValidator()).When(x => x.AttachmentUri != null);
        }
    }

    public class DisplayAlarmValidator : AbstractValidator<DISPLAY_ALARM>
    {
        public DisplayAlarmValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Description).NotNull().SetValidator(new TextValidator());
        }
    }

    public class EmailAlarmValidator : AbstractValidator<EMAIL_ALARM>
    {
        public EmailAlarmValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Description).NotNull().SetValidator(new TextValidator());
            RuleFor(x => x.Summary).NotNull().SetValidator(new TextValidator());
            RuleFor(x => x.Attendees).NotNull().NotEmpty().SetCollectionValidator(new AttendeeValidator());
            RuleFor(x => x.AttachmentBinaries).SetCollectionValidator(new AttachmentBinaryValidator()).When(x => !x.AttachmentBinaries.NullOrEmpty());
            RuleFor(x => x.AttachmentUris).SetCollectionValidator(new AttachmentUriValidator()).When(x => !x.AttachmentUris.NullOrEmpty());
        }
    }
}