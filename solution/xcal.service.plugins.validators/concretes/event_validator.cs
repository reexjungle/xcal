using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.crosscut.essentials.concretes;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.plugins.validators.concretes;


namespace reexmonkey.xcal.service.plugins.validators.concretes
{
    public class EventValidator: AbstractValidator<VEVENT>
    {
        public EventValidator(): base()
        {
            RuleFor(x => x.Description).SetValidator(new TextValidator()).When(x => x.Description != null);
            RuleFor(x => x.Comments).SetCollectionValidator(new TextValidator()).When(x => !x.Comments.NullOrEmpty());
        }
    }
}
