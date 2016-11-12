using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public struct WEEKDAYNUM 
    {
        /// <summary>
        /// Gets the nth occurence of the day within the MONTHLY or YEARLY recurrence rule
        /// </summary>
        public int NthOccurrence { get; }

        /// <summary>
        /// Gets the weekday
        /// </summary>
        public WEEKDAY Weekday { get; }
    }
}
