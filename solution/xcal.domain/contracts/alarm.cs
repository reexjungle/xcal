using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.domain.contracts
{

    /// <summary>
    /// Specifies a general contract for alarm objects associated to an iCalendar object.
    /// </summary>
    public interface IALARM: ICOMPONENT 
    { 
        ACTION Action { get; set; }
        TRIGGER Trigger { get; set; }
        DURATION Duration { get; set; }
        int Repeat { get; set; }

    }

    public interface IAUDIO_ALARM: IALARM
    {
        IATTACH Attachment { get; set; }
    }

    public interface IDISPLAY_ALARM: IALARM
    {
        DESCRIPTION Description { get; set; }
    }

    public interface IEMAIL_ALARM: IALARM
    {
        DESCRIPTION Description { get; set; }
        SUMMARY Summary { get; set; }
        List<ATTENDEE> Attendees { get; set; }
        List<IATTACH> Attachments { get; set; }
    }

}
