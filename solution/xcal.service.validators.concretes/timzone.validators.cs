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
    public class TimeZoneValidator : AbstractValidator<VTIMEZONE>
    {
        public TimeZoneValidator()
            : base()
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


}
