using System.Runtime.Serialization;

namespace reexmonkey.xcal.domain.models
{
    [DataContract]
    public class REL_CALENDARS_EVENTS
    {
        /// <summary>
        /// Gets or sets the unique identifier of the calender-event relation
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the product identifier of the related calendar entity
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the related event entity
        /// </summary>
        [DataMember]
        public string Uid { get; set; }
    }
}
