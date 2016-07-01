using reexjungle.xcal.service.operations.concretes.live;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;
using System;

namespace reexjungle.xcal.service.validators.concretes
{
    #region calendar request dto validators

    public class AddCalendarValidator : AbstractValidator<AddCalendar>
    {
        public AddCalendarValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Calendar).NotNull();
            //RuleFor(x => x.Calendar).SetValidator(new CalendarValidator()).When(x => x.Calendar != null);
        }
    }

    public class AddCalendarsValidator : AbstractValidator<AddCalendars>
    {
        public AddCalendarsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Calendars).NotNull();
            RuleFor(x => x.Calendars).SetCollectionValidator(new CalendarValidator()).When(x => !x.Calendars.NullOrEmpty());
        }
    }

    public class UpdateCalendarValidator : AbstractValidator<UpdateCalendar>
    {
        public UpdateCalendarValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Calendar).NotNull();
            RuleFor(x => x.Calendar).SetValidator(new CalendarValidator()).When(x => x.Calendar != null);
        }
    }

    public class UpdateCalendarsValidator : AbstractValidator<UpdateCalendars>
    {
        public UpdateCalendarsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Calendars).NotNull().NotEmpty();
            RuleFor(x => x.Calendars).SetCollectionValidator(new CalendarValidator()).When(x => !x.Calendars.NullOrEmpty());
        }
    }

    public class PatchCalendarValidator : AbstractValidator<PatchCalendar>
    {
        public PatchCalendarValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => y != Guid.Empty);
        }
    }

    public class PatchCalendarsValidator : AbstractValidator<PatchCalendars>
    {
        public PatchCalendarsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarIds).NotNull().NotEmpty();
        }
    }

    public class DeleteCalendarValidator : AbstractValidator<DeleteCalendar>
    {
        public DeleteCalendarValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => y != Guid.Empty);
        }
    }

    public class FindCalendarValidator : AbstractValidator<FindCalendar>
    {
        public FindCalendarValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => y != Guid.Empty);
        }
    }

    public class FindCalendarsValidator : AbstractValidator<FindCalendars>
    {
        public FindCalendarsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarIds).NotNull().NotEmpty();
        }
    }

    public class GetCalendarsValidator : AbstractValidator<GetCalendars>
    {
        public GetCalendarsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Page).GreaterThan(0).When(x => x.Page != null);
            RuleFor(x => x.Size).GreaterThan(0).When(x => x.Size != null);
        }
    }

    #endregion calendar request dto validators

    #region event request dto validators

    public class AddEventValidator : AbstractValidator<AddEvent>
    {
        public AddEventValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => x.CalendarId != Guid.Empty);
            RuleFor(x => x.Event).NotNull();
            RuleFor(x => x.Event).SetValidator(new EventValidator()).When(x => x.Event != null);
        }
    }

    public class AddEventsValidator : AbstractValidator<AddEvents>
    {
        public AddEventsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => x.CalendarId != Guid.Empty);
            RuleFor(x => x.Events).NotNull();
            RuleFor(x => x.Events).SetCollectionValidator(new EventValidator()).When(x => !x.Events.NullOrEmpty());
        }
    }

    public class GetEventsValidator : AbstractValidator<GetEvents>
    {
        public GetEventsValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Page).GreaterThan(0).When(x => x.Page != null && x.Page.HasValue);
            RuleFor(x => x.Size).GreaterThan(0).When(x => x.Size != null && x.Size.HasValue);
        }
    }

    #endregion event request dto validators
}