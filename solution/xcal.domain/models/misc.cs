using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServiceStack.DataAnnotations;
using reexmonkey.xcal.domain.contracts;



namespace reexmonkey.xcal.domain.models
{
    [DataContract]
    public class IANA_COMPONENT: IMISC_COMPONENT
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [Ignore]
        List<IANA_PROPERTY> ContentLines { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("BEGIN:{0}", this.Name).AppendLine();
            foreach (var line in this.ContentLines) sb.Append(line);
            sb.AppendFormat("END:{0}", this.Name).AppendLine();
            return sb.ToString();
        }

    }

    [DataContract]
    public class X_COMPONENT : IMISC_COMPONENT
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [Ignore]
        List<X_PROPERTY> ContentLines { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("BEGIN:{0}", this.Name).AppendLine();
            foreach (var line in this.ContentLines) sb.Append(line);
            sb.AppendFormat("END:{0}", this.Name).AppendLine();
            return sb.ToString();
        }

    }
}
