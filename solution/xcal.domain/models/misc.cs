using reexjungle.xcal.domain.contracts;
using ServiceStack.DataAnnotations;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class IANA_COMPONENT : IMISC_COMPONENT
    {
        [DataMember]
        public string TokenName { get; set; }

        [DataMember]
        [Ignore]
        public List<IANA_PROPERTY> ContentLines { get; set; }

        public IANA_COMPONENT()
        {
            this.ContentLines = new List<IANA_PROPERTY>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("BEGIN:{0}", this.TokenName).AppendLine();
            foreach (var line in this.ContentLines) sb.Append(line);
            sb.AppendFormat("END:{0}", this.TokenName);
            return sb.ToString();
        }
    }

    [DataContract]
    public class X_COMPONENT : IMISC_COMPONENT
    {
        [DataMember]
        public string TokenName { get; set; }

        [DataMember]
        [Ignore]
        private List<X_PROPERTY> ContentLines { get; set; }

        public X_COMPONENT()
        {
            this.ContentLines = new List<X_PROPERTY>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("BEGIN:{0}", this.TokenName).AppendLine();
            foreach (var line in this.ContentLines) sb.Append(line);
            sb.AppendFormat("END:{0}", this.TokenName);
            return sb.ToString();
        }
    }
}