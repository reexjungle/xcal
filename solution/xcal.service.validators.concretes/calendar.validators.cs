using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexjungle.foundation.essentials.concretes;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;

namespace reexjungle.xcal.service.validators.concretes
{
    public class CalendarValidator: AbstractValidator<VCALENDAR>
    {
        public CalendarValidator()
        {
            RuleFor(x => x.Id).Must((x, y) => !string.IsNullOrEmpty(x.Id) || !string.IsNullOrWhiteSpace(x.Id));
            RuleFor(x => x.ProdId).Must((x, y) => !string.IsNullOrEmpty(x.ProdId) || !string.IsNullOrWhiteSpace(x.ProdId));
            RuleFor(x => x.Version).Must((x, y) => !string.IsNullOrEmpty(x.ProdId) || !string.IsNullOrWhiteSpace(x.ProdId));
            RuleFor(x => x.Events).NotNull();
            RuleFor(x => x.Events).SetCollectionValidator(new EventValidator()).When(x => !x.Events.NullOrEmpty());
            RuleFor(x => x.ToDos).NotNull();
            RuleFor(x => x.Journals).NotNull();
            RuleFor(x => x.FreeBusies).NotNull();
            RuleFor(x => x.TimeZones).NotNull();
        }
    }
}
