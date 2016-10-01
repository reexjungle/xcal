using System;
using System.Collections.Generic;
using System.IO;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.infrastructure.serialization;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;

namespace reexjungle.xcal.service.plugins.formats.concretes
{
    /// <summary>
    /// </summary>
    public class iCalendarFormat : IPlugin
    {
        private const string mime_type = "text/calendar";
        private const string ext = "ics";

        public void Register(IAppHost appHost)
        {
            appHost.ContentTypeFilters.Register(mime_type, SerializeToStream, DeserializeFromStream);
            appHost.ResponseFilters.Add((req, res, dto) =>
                {
                    if (req.ResponseContentType.Equals(mime_type, StringComparison.OrdinalIgnoreCase))
                        res.AddHeader(HttpHeaders.ContentDisposition,
                            $"attachment;filename={req.OperationName}.{ext}");
                });
        }

        private static void Serialize<T>(T instance, CalendarWriter writer)
        {
            var serializer = new CalendarSerializer<T>();
            serializer.Serialize(instance, writer.InsertLineBreaks());
        }

        private static void Serialize<T>(IEnumerable<T> instances, CalendarWriter writer)
        {
            var serializer = new CalendarSerializer<T>();
            foreach (var instance in instances)
            {
                serializer.Serialize(instance, writer.InsertLineBreaks());
            }
        }

        /// <summary>
        /// Serializes the data transfer object to stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="o">The dto.</param>
        /// <param name="stream">The stream.</param>
        public static void SerializeToStream(IRequestContext context, object o, Stream stream)
        {
            using (var writer = new CalendarStreamWriter(stream))
            {
                if (o == null) return;

                //Serialize VCALENDAR
                var calendar = o as VCALENDAR;
                if (calendar != null) Serialize(calendar, writer);
                else
                {
                    var calendars = o as IEnumerable<VCALENDAR>;
                    if (calendars != null) Serialize(calendars, writer);
                }

                //Serialize VEVENT
                var @event = o as VEVENT;
                if (@event != null) Serialize(@event, writer);
                else
                {
                    var events = o as IEnumerable<VEVENT>;
                    if (events != null) Serialize(events, writer);
                }

                //Serialize VTODO
                var todo = o as VTODO;
                if (todo != null) Serialize(todo, writer);
                else
                {
                    var todos = o as IEnumerable<VTODO>;
                    if (todos != null) Serialize(todos, writer);
                }

                //Serialize VJOURNAL
                var journal = o as VJOURNAL;
                if (journal != null) Serialize(journal, writer);
                else
                {
                    var journals = o as IEnumerable<VJOURNAL>;
                    if (journals != null) Serialize(journals, writer);
                }

                //Serialize VFREEBUSY
                var freebusy = o as VFREEBUSY;
                if (freebusy != null) Serialize(freebusy, writer);
                else
                {
                    var freebusies = o as IEnumerable<VFREEBUSY>;
                    if (freebusies != null) Serialize(freebusies, writer);
                }

                //Serialize VTIMEZONE
                var timezone = o as VTIMEZONE;
                if (timezone != null) Serialize(timezone, writer);
                else
                {
                    var timezones = o as IEnumerable<VTIMEZONE>;
                    if (timezones != null) Serialize(timezones, writer);
                }
            }
        }

        public static object DeserializeFromStream(Type type, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}