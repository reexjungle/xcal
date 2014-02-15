using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexmonkey.xcal.domain.contracts
{

    /// <summary>
    /// Specifies a general contract for alarm objects associated to an iCalendar object.
    /// </summary>
    public interface IALARM: ICOMPONENT 
    { 
        ACTION Action { get; set; }
        ITRIGGER Trigger { get; set; }
        IDURATION Duration { get; set; }
        IREPEAT Repeat { get; set; }

    }

    public interface IAUDIO_ALARM
    {
        IATTACH Attachment { get; set; }
    }

    public interface IDALARM
    {
        ITEXT Description { get; set; }
    }

    public interface IEMAIL_ALARM
    {
        ITEXT Description { get; set; }
        ITEXT Summary { get; set; }
        IEnumerable<IATTENDEE> Attendees { get; set; }
        IEnumerable<IATTACH> Attachments { get; set; }
    }

}
