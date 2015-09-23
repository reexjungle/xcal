using reexjungle.xcal.domain.contracts;
using reexjungle.xmisc.foundation.contracts;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace reexjungle.xcal.domain.models
{
    [DataContract]
    public class IANA_COMPONENT : IMISC_COMPONENT, IContainsKey<Guid>, IEquatable<IANA_COMPONENT>
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string TokenName { get; set; }

        [DataMember]
        [Ignore]
        public List<IANA_PROPERTY> ContentLines { get; set; }

        public IANA_COMPONENT()
        {
            ContentLines = new List<IANA_PROPERTY>();
        }

        public bool Equals(IANA_COMPONENT other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IANA_COMPONENT)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(IANA_COMPONENT left, IANA_COMPONENT right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IANA_COMPONENT left, IANA_COMPONENT right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("BEGIN:{0}", TokenName).AppendLine();
            foreach (var line in ContentLines) sb.Append(line);
            sb.AppendFormat("END:{0}", TokenName);
            return sb.ToString();
        }
    }

    [DataContract]
    public class X_COMPONENT : IMISC_COMPONENT, IContainsKey<Guid>, IEquatable<X_COMPONENT>
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string TokenName { get; set; }

        [DataMember]
        [Ignore]
        private List<X_PROPERTY> ContentLines { get; set; }

        public X_COMPONENT()
        {
            ContentLines = new List<X_PROPERTY>();
        }

        public bool Equals(X_COMPONENT other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((X_COMPONENT)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(X_COMPONENT left, X_COMPONENT right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(X_COMPONENT left, X_COMPONENT right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("BEGIN:{0}", TokenName).AppendLine();
            foreach (var line in ContentLines) sb.Append(line);
            sb.AppendFormat("END:{0}", TokenName);
            return sb.ToString();
        }
    }
}