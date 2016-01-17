using reexjungle.xcal.domain.models;
using reexjungle.xmisc.foundation.concretes;
using ServiceStack.FluentValidation;

namespace reexjungle.xcal.service.validators.concretes
{
    public class TimeZoneValidator : AbstractValidator<VTIMEZONE>
    {
        public TimeZoneValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(x => x.TimeZoneId).SetValidator(new TimeZoneIdValidator()).When(x => x.TimeZoneId != null);
            RuleFor(x => x.StandardTimes).SetCollectionValidator(new ObservanceValidator()).
                Must((x, y) => y.IsSet()).
                When(x => !x.StandardTimes.NullOrEmpty());
            RuleFor(x => x.DaylightTimes).SetCollectionValidator(new ObservanceValidator()).
                Must((x, y) => y.IsSet()).
                When(x => !x.DaylightTimes.NullOrEmpty());
        }
    }
}