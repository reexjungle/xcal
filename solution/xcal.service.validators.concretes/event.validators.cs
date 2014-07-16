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



}
