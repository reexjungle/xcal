using reexjungle.xcal.core.domain.contracts.models.values;

namespace reexjungle.xcal.core.domain.contracts.models.parameters
{
    public interface ISENT_BY
    {
        /// <summary>
        /// Gets the address of the calendar user actingf on the behalf of the other user.
        /// </summary>
        CAL_ADDRESS Address { get; set; }
    }
}
