using reexjungle.xcal.test.server.integration.contracts;
using ServiceStack.ServiceClient.Web;

namespace reexjungle.xcal.test.server.integration.concretes
{
    public class JsonWebServiceTestFactory : IServiceTestFactory<JsonServiceClient>
    {
        private JsonServiceClient client = null;

        public string BaseUri { get; set; }

        public JsonWebServiceTestFactory(string baseUri)
        {
            BaseUri = baseUri;
        }

        public JsonServiceClient GetClient()
        {
            return client ?? (client = new JsonServiceClient(BaseUri));
        }
    }

    public class XmlWebServiceTestFactory : IServiceTestFactory<XmlServiceClient>
    {
        private XmlServiceClient client = null;

        public string BaseUri { get; set; }

        public XmlWebServiceTestFactory(string baseUri)
        {
            BaseUri = baseUri;
        }

        public XmlServiceClient GetClient()
        {
            return client ?? (client = new XmlServiceClient(BaseUri));
        }
    }

    public class JsvWebServicesTestFactory : IServiceTestFactory<JsvServiceClient>
    {
        private JsvServiceClient client = null;

        public JsvWebServicesTestFactory(string baseUri)
        {
            BaseUri = baseUri;
        }

        public string BaseUri { get; set; }

        public JsvServiceClient GetClient()
        {
            return client ?? (client = new JsvServiceClient(BaseUri));
        }
    }
}