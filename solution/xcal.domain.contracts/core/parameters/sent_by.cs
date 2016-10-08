using xcal.domain.contracts.core.values;

namespace xcal.domain.contracts.core.parameters
{
    public interface ISENT_BY
    {
        /// <summary>
        /// Gets the address of the calendar user actingf on the behalf of the other user.
        /// </summary>
        ICAL_ADDRESS Address { get; set; }
    }
}
