using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;
using System;

namespace reexjungle.xcal.service.validators.concretes
{
    /// <summary>
    /// Validates instances of the <see cref="VCALENDAR"/> type.
    /// </summary>
    public class CalendarValidator : AbstractValidator<VCALENDAR>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CalendarValidator()
        {
            RuleFor(x => x.Id).Must((x, y) => x.Id != Guid.Empty);
            RuleFor(x => x.ProdId).Must((x, y) => !string.IsNullOrWhiteSpace(x.ProdId));
            RuleFor(x => x.Version).Must((x, y) => !string.IsNullOrWhiteSpace(x.Version));
            RuleFor(x => x.Events).SetCollectionValidator(new EventValidator()).When(x => !x.Events.NullOrEmpty());
            //RuleFor(x => x.ToDos).NotNull();
            //RuleFor(x => x.Journals).NotNull();
            //RuleFor(x => x.FreeBusies).NotNull();
            //RuleFor(x => x.TimeZones).NotNull();
        }
    }
}