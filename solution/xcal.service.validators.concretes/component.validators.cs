using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;

namespace reexjungle.xcal.service.validators.concretes
{
    /// <summary>
    /// Validates instances of the <see cref="VEVENT"/> type.
    /// </summary>
    public class EventValidator : AbstractValidator<VEVENT>
    {
        private static readonly DateTimeValidator DateTimeValidator = new DateTimeValidator();
        private static readonly TextValidator TextValidator = new TextValidator();

        /// <summary>
        /// Default constructor
        /// </summary>
        public EventValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Datestamp).SetValidator(DateTimeValidator).When(x => x.Url != null);
            RuleFor(x => x.Uid).Must((x, y) => !string.IsNullOrWhiteSpace(y));
            RuleFor(x => x.Start).SetValidator(DateTimeValidator);
            RuleFor(x => x.Created).SetValidator(DateTimeValidator);
            //RuleFor(x => x.Organizer).NotNull().SetValidator(new OrganizerValidator());
            //RuleFor(x => x.Description).SetValidator(TextValidator).When(x => x.Description != null);
            //RuleFor(x => x.Location).SetValidator(TextValidator).When(x => x.Location != null);
            //RuleFor(x => x.Summary).SetValidator(TextValidator).When(x => x.Summary != null);
            //RuleFor(x => x.RecurrenceId).SetValidator(new RecurrenceIdValidator()).When(x => x.RecurrenceId != null);
            //RuleFor(x => x.Comments).SetCollectionValidator(TextValidator).When(x => !x.Comments.NullOrEmpty());
        }
    }
}