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
    public class JsonWebServicesTests : ServiceTestsBase<JsonServiceClient>
    {
        public JsonWebServicesTests()
            : base()
        {
            this.baseUri = Properties.Settings.Default.localhost_uri;
        }

        public JsonWebServicesTests(string baseUri)
            : base(baseUri)
        {
        }

        public override JsonServiceClient CreateServiceClient()
        {
            return new JsonServiceClient(this.BaseUri);
        }
    }

    public class XmlWebServicesTests : ServiceTestsBase<XmlServiceClient>
    {
        public XmlWebServicesTests()
            : base()
        {
        }

        public XmlWebServicesTests(string baseUri)
            : base(baseUri)
        {
        }

        public override XmlServiceClient CreateServiceClient()
        {
            return new XmlServiceClient(this.BaseUri);
        }
    }

    public class JsvWebServicesTests : ServiceTestsBase<JsvServiceClient>
    {
        public JsvWebServicesTests()
            : base()
        {
        }

        public JsvWebServicesTests(string baseUri)
            : base(baseUri)
        {
        }

        public override JsvServiceClient CreateServiceClient()
        {
            return new JsvServiceClient(this.BaseUri);
        }
    }
}