using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.ServiceHost;
using ServiceStack.Common.Web;
using reexmonkey.xcal.domain.models;


namespace reexmonkey.xcal.service.plugins.formats.concretes
{
    /// <summary>
    /// 
    /// </summary>
    public class iCalendarFormat: IPlugin
    {
        private const string mime_type = "text/calendar";
        private const string ext = "ics";

        public void Register(IAppHost appHost)
        {
            appHost.ContentTypeFilters.Register(mime_type, iCalendarFormat.SerializeToStream, iCalendarFormat.DeserializeFromStream);
            appHost.ResponseFilters.Add((req, res, dto) =>
                {
                    if (req.ResponseContentType.Equals(mime_type, StringComparison.OrdinalIgnoreCase))
                        res.AddHeader(HttpHeaders.ContentDisposition,
                        string.Format("attachment;filename={0}.{1}", req.OperationName, ext));
         
                });
        }

        /// <summary>
        /// Serializes to stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dto">The dto.</param>
        /// <param name="stream">The stream.</param>
        public static void SerializeToStream(IRequestContext context, object dto, Stream stream)
        {
            using (var sw = new StreamWriter(stream))
            {
                if(dto != null)
                {
                    //calendars
                    if(dto is VCALENDAR) sw.WriteLine(dto as VCALENDAR);
                    if(dto is List<VCALENDAR>)
                    {
                        var cals = dto as List<VCALENDAR>;
                        cals.ForEach(x => sw.WriteLine(x));
                    }

                    //events
                    if(dto is  VEVENT) sw.WriteLine(dto as VEVENT);
                    if (dto is List<VEVENT>)
                    {
                        var events = dto as List<VEVENT>;
                        events.ForEach(x => sw.WriteLine(x));
                    }

                    //todos
                    if (dto is VTODO) sw.WriteLine(dto as VTODO);
                    if (dto is List<VTODO>)
                    {
                        var todos = dto as List<VTODO>;
                        todos.ForEach(x => sw.WriteLine(x));
                    }

                    //journals
                    if (dto is VJOURNAL) sw.WriteLine(dto as VJOURNAL);
                    if (dto is List<VJOURNAL>)
                    {
                        var journals = dto as List<VJOURNAL>;
                        journals.ForEach(x => sw.WriteLine(x));
                    }

                    //timezones
                    if (dto is VTIMEZONE) sw.WriteLine(dto as VTIMEZONE);
                    if (dto is List<VTIMEZONE>)
                    {
                        var journals = dto as List<VTIMEZONE>;
                        journals.ForEach(x => sw.WriteLine(x));
                    }

                    //IANA Components
                    if (dto is IANA_COMPONENT) sw.WriteLine(dto as IANA_COMPONENT);
                    if (dto is List<IANA_COMPONENT>)
                    {
                        var ianac = dto as List<IANA_COMPONENT>;
                        ianac.ForEach(x => sw.WriteLine(x));
                    }

                    //X Components
                    if (dto is XCOMPONENT) sw.WriteLine(dto as XCOMPONENT);
                    if (dto is List<XCOMPONENT>)
                    {
                        var xc = dto as List<XCOMPONENT>;
                        xc.ForEach(x => sw.WriteLine(x));
                    }

                }
            } 
        }

        public static object DeserializeFromStream(Type type, Stream stream)
        {
            throw new NotImplementedException();
        }

    }
}
