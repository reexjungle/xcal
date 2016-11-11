using reexjungle.xcal.core.domain.concretes.models.values;
using reexjungle.xcal.core.domain.contracts.models.values;
using System;

namespace reexjungle.xcal.core.domain.concretes.extensions
{
    public static class DurationExtensions
    {
        public static IDURATION AsDURATION(this TimeSpan timespan, Func<TimeSpan, IDURATION> func) => func(timespan);

        public static DURATION AsDURATION(this TimeSpan timespan) => new DURATION(timespan);

        public static IDURATION Add(this IDURATION duration, IDURATION other, Func<int, int, int, int, int, IDURATION> func)
            => func(duration.WEEKS + other.WEEKS, duration.DAYS + other.DAYS, duration.HOURS + other.HOURS, duration.MINUTES + other.MINUTES, duration.SECONDS + other.SECONDS);

        public static DURATION Add(this IDURATION duration, IDURATION other)
            => new DURATION(duration.WEEKS + other.WEEKS, duration.DAYS + other.DAYS, duration.HOURS + other.HOURS, duration.MINUTES + other.MINUTES, duration.SECONDS + other.SECONDS);

        public static IDURATION Subtract(this IDURATION duration, IDURATION other, Func<int, int, int, int, int, IDURATION> func)
            => func(duration.WEEKS - other.WEEKS, duration.DAYS - other.DAYS, duration.HOURS - other.HOURS, duration.MINUTES - other.MINUTES, duration.SECONDS - other.SECONDS);

        public static DURATION Subtract(this IDURATION duration, IDURATION other)
            => new DURATION(duration.WEEKS - other.WEEKS, duration.DAYS - other.DAYS, duration.HOURS - other.HOURS, duration.MINUTES - other.MINUTES, duration.SECONDS - other.SECONDS);

        public static IDURATION MultiplyBy(this IDURATION duration, int scalar, Func<int, int, int, int, int, IDURATION> func)
            => func(duration.WEEKS * scalar, duration.DAYS * scalar, duration.HOURS * scalar, duration.MINUTES * scalar, duration.SECONDS * scalar);

        public static DURATION MultiplyBy(this IDURATION duration, int scalar)
            => new DURATION(duration.WEEKS * scalar, duration.DAYS * scalar, duration.HOURS * scalar, duration.MINUTES * scalar, duration.SECONDS * scalar);

        public static IDURATION DivideBy(this IDURATION duration, int scalar, Func<int, int, int, int, int, IDURATION> func)
            => func(duration.WEEKS / scalar, duration.DAYS / scalar, duration.HOURS / scalar, duration.MINUTES / scalar, duration.SECONDS / scalar);

        public static DURATION DivideBy(this IDURATION duration, int scalar)
            => new DURATION(duration.WEEKS / scalar, duration.DAYS / scalar, duration.HOURS / scalar, duration.MINUTES / scalar, duration.SECONDS / scalar);
    }
}
