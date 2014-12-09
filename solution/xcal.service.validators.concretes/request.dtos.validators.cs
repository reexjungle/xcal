using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexjungle.foundation.essentials.concretes;
using reexjungle.crosscut.operations.concretes;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.domain.operations;

namespace reexjungle.xcal.service.validators.concretes
{

    #region calendar request dto validators

    public class AddCalendarValidator : AbstractValidator<AddCalendar>
    {
        public AddCalendarValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Calendar).NotNull();
            RuleFor(x => x.Calendar).SetValidator(new CalendarValidator()).When(x => x.Calendar != null);
        }
    }

    public class AddCalendarsValidator : AbstractValidator<AddCalendars>
    {
        public AddCalendarsValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Calendars).NotNull();
            RuleFor(x => x.Calendars).SetCollectionValidator(new CalendarValidator()).When(x => !x.Calendars.NullOrEmpty());
        }
    }

    public class UpdateCalendarValidator : AbstractValidator<UpdateCalendar>
    {
        public UpdateCalendarValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Calendar).NotNull();
            RuleFor(x => x.Calendar).SetValidator(new CalendarValidator()).When(x => x.Calendar != null);

        }
    }

    public class UpdateCalendarsValidator : AbstractValidator<UpdateCalendars>
    {
        public UpdateCalendarsValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Calendars).NotNull().NotEmpty();
            RuleFor(x => x.Calendars).SetCollectionValidator(new CalendarValidator()).When(x => !x.Calendars.NullOrEmpty());

        }
    }

    public class PatchCalendarValidator : AbstractValidator<PatchCalendar>
    {
        public PatchCalendarValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => !string.IsNullOrEmpty(y) || !string.IsNullOrWhiteSpace(y));
        }
    }

    public class PatchCalendarsValidator : AbstractValidator<PatchCalendars>
    {
        public PatchCalendarsValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarIds).NotNull().NotEmpty();
        }
    }


    public class DeleteCalendarValidator : AbstractValidator<DeleteCalendar>
    {
        public DeleteCalendarValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => !string.IsNullOrEmpty(y) || !string.IsNullOrWhiteSpace(y));
        }
    }

    public class DeleteCalendarsValidator : AbstractValidator<DeleteCalendars>
    {
        public DeleteCalendarsValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarIds).NotNull().NotEmpty();
        }
    }

    public class FindCalendarValidator : AbstractValidator<FindCalendar>
    {
        public FindCalendarValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => !string.IsNullOrEmpty(y) || !string.IsNullOrWhiteSpace(y));
        }
    }

    public class FindCalendarsValidator : AbstractValidator<FindCalendars>
    {
        public FindCalendarsValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarIds).NotNull().NotEmpty();
        }
    }

    public class GetCalendarsValidator : AbstractValidator<GetCalendars>
    {
        public GetCalendarsValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Page).GreaterThan(0).When(x => x.Page != null && x.Page.HasValue);
            RuleFor(x => x.Size).GreaterThan(0).When(x => x.Size != null && x.Size.HasValue);
        }
    } 


    #endregion

    #region event request dto validators
    
    public class AddEventValidator: AbstractValidator<AddEvent>
    {
        public AddEventValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => !string.IsNullOrEmpty(x.CalendarId) || !string.IsNullOrWhiteSpace(x.CalendarId));
            RuleFor(x => x.Event).NotNull();
            RuleFor(x => x.Event).SetValidator(new EventValidator()).When(x => x.Event != null);

        }
    }

    public class AddEventsValidator : AbstractValidator<AddEvents>
    {
        public AddEventsValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.CalendarId).Must((x, y) => !string.IsNullOrEmpty(x.CalendarId) || !string.IsNullOrWhiteSpace(x.CalendarId));
            RuleFor(x => x.Events).NotNull();
            RuleFor(x => x.Events).SetCollectionValidator(new EventValidator()).When(x => !x.Events.NullOrEmpty());

        }
    }

    public class GetEventsValidator : AbstractValidator<GetEvents>
    {
        public GetEventsValidator()
        {
            CascadeMode = ServiceStack.FluentValidation.CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.Page).GreaterThan(0).When(x => x.Page != null && x.Page.HasValue);
            RuleFor(x => x.Size).GreaterThan(0).When(x => x.Size != null && x.Size.HasValue);
        }
    }


    
    #endregion

}
