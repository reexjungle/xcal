using Funq;
using reexjungle.xcal.test.server.integration.contracts;
using ServiceStack.Service;
using ServiceStack.ServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexjungle.xcal.test.server.integration.concretes
{
    public abstract class ServiceTestsBase<T> : IServiceTests<T>
        where T : IServiceClient
    {
        protected string baseUri = string.Empty;

        public string BaseUri
        {
            get { return baseUri; }
            set { baseUri = value; }
        }

        public ServiceTestsBase()
        {
        }

        public ServiceTestsBase(string baseUri)
        {
            this.baseUri = baseUri;
        }

        public abstract T CreateServiceClient();
    }
}