using System;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.parameters;
using reexjungle.xcal.core.domain.contracts.models.values;

namespace reexjungle.xcal.core.domain.contracts.extensions
{
    public static class TimeExtensions
    {

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="TIME"/> value.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <returns>The <see cref="TIME"/> value that results from the conversion.</returns>
        public static TIME AsTIME(this DateTime datetime) => new TIME(datetime);

        public static TIME AsTIME(this DateTimeOffset datetime) => new TIME(datetime);

    }
}
