using FizzWare.NBuilder;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xmisc.foundation.concretes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexjungle.xcal.tests.concretes.factories
{
    public class SharedFactory : ISharedFactory
    {
        private readonly RandomGenerator rndGenerator;
        private readonly List<string> suffixes;
        private readonly List<string> prefixes;

        public SharedFactory()
        {
            rndGenerator = new RandomGenerator();

            prefixes = new List<string>
            {
                "http",
                "https",
                "ftp",
                "ftps",
                "sftp",
            };

            suffixes = new List<string>
            {
                "com",
                "edu",
                "org",
                "ca",
                "de",
                "es",
                "fr",
                "it",
                "pl",
                "ir",
                "ro",
                "co.uk",
                "net"
            };
        }

        public string CreateEmail(string username = null)
        {
            var prefix = string.IsNullOrWhiteSpace(username)
                ? rndGenerator.Phrase(10)
                : username;
            return string.Format("{0}@example.{1}",
                prefix.Replace(" ", ".").ToLower(),
                Pick<string>.RandomItemFrom(suffixes));
        }

        public IEnumerable<string> CreateEmails(int quantity, IEnumerable<string> usernames = null)
        {
            Func<int, IEnumerable<string>> func = (max) =>
            {
                var generated = new List<string>();
                for (var i = 0; i < max; i++)
                {
                    generated.Add(CreateEmail());
                }
                return generated;
            };

            if (!usernames.NullOrEmpty())
            {
                var count = usernames.Count();
                var created = usernames.Select(CreateEmail).ToList();
                return quantity < count
                    ? created.Select(CreateEmail).Take(quantity)
                    : created.Select(CreateEmail).Merge(func(quantity - count));
            }

            return func(quantity);
        }

        public string CreateUrl()
        {
            return string.Format("{0}://{1}.{2}",
                Pick<string>.RandomItemFrom(prefixes),
                rndGenerator.Phrase(10).Replace(" ", "."),
                Pick<string>.RandomItemFrom(suffixes));
        }

        public IEnumerable<string> CreateUrls(int quantity)
        {
            var urls = new List<string>();

            for (var i = 0; i < quantity; i++)
            {
                urls.Add(CreateUrl());
            }

            return urls;
        }

        public string CreateUrl(string resource)
        {
            return !string.IsNullOrWhiteSpace(resource)
                ? string.Format("{0}/{1}", CreateUrl(), resource)
                : CreateUrl();
        }
    }
}