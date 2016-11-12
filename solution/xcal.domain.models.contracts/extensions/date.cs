using System;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.values;

namespace reexjungle.xcal.core.domain.contracts.extensions
{
    public static class DateExtensions
    {

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="DATE"/> value.
        /// </summary>
        /// <param name="datetime">The <see cref="DateTime"/> value to be converted.</param>
        /// <returns>The <see cref="DATE"/> value that results from the conversion.</returns>
        public static DATE AsDATE(this DateTime datetime) => new DATE(datetime);



    }
}
