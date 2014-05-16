using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceHost;
using reexmonkey.xcal.service.plugins.formats.concretes;

namespace xcal.service.clients.concretes
{
    public class iCalendarServiceClient : ServiceClientBase
    {
        public override string ContentType
        {
            get { return "text/calendar"; }
        }

        public override string Format
        {
            get { return "calendar"; }
        }


        public override StreamDeserializerDelegate StreamDeserializer
        {
            get { return iCalendarFormat.DeserializeFromStream; }
        }

        
        public iCalendarServiceClient(string baseUri): base()
        {
            this.SetBaseUri(baseUri);
        }

        public iCalendarServiceClient(string syncReplyBaseUri, string asyncOneWayBaseUri) : base(syncReplyBaseUri, asyncOneWayBaseUri) { }

        public override T DeserializeFromStream<T>(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override void SerializeToStream(IRequestContext requestContext, object request, Stream stream)
        {
            throw new NotImplementedException();
        }


    }
}
