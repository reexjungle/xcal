using System.Collections.Generic;

namespace reexjungle.xcal.tests.contracts.factories
{
    public interface ISharedFactory
    {
        string CreateEmail(string username = null);

        IEnumerable<string> CreateEmails(int quantity, IEnumerable<string> usernames = null);

        string CreateUrl();

        IEnumerable<string> CreateUrls(int quantity);

        string CreateUrl(string resource);
    }
}