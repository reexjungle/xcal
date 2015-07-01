using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;
using System;

namespace reexjungle.xcal.service.validators.concretes
{
    public class CalendarValidator : AbstractValidator<VCALENDAR>
    {
        public CalendarValidator()
        {
            RuleFor(x => x.Id).Must((x, y) => x.Id != Guid.Empty);
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