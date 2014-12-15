using reexjungle.xcal.test.server.integration.contracts;
using ServiceStack.Service;
using ServiceStack.ServiceClient.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexjungle.xcal.test.server.integration.concretes
{
    public class JsonWebServiceTestFactory : IServiceTestFactory<JsonServiceClient>
    {
        private JsonServiceClient client = null;

        public string BaseUri { get; set; }

        public JsonWebServiceTestFactory(string baseUri)
        {
            this.BaseUri = baseUri;
        }

        public JsonServiceClient GetClient()
        {
            return this.client ?? (this.client = new JsonServiceClient(this.BaseUri));
        }
    }

    public class XmlWebServiceTestFactory : IServiceTestFactory<XmlServiceClient>
    {
        private XmlServiceClient client = null;

        public string BaseUri { get; set; }

        public XmlWebServiceTestFactory(string baseUri)
        {
            this.BaseUri = baseUri;
        }

        public XmlServiceClient GetClient()
        {
            return this.client ?? (this.client = new XmlServiceClient(this.BaseUri));
        }
    }

    public class JsvWebServicesTestFactory : IServiceTestFactory<JsvServiceClient>
    {
        private JsvServiceClient client = null;

        public JsvWebServicesTestFactory(string baseUri)
        {
            this.BaseUri = baseUri;
        }

        public string BaseUri { get; set; }

        public JsvServiceClient GetClient()
        {
            return this.client ?? (this.client = new JsvServiceClient(this.BaseUri));
        }
    }
}