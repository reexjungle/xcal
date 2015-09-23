using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace reexjungle.xcal.crosscut.concretes.security
{
    public class TrustAllCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }

        public bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }

    public class TrustX509CertificatePolicy : ICertificatePolicy
    {
        private string hash = string.Empty;

        public string Hash
        {
            get { return hash; }
        }

        public TrustX509CertificatePolicy(string certhash)
        {
            hash = certhash;
        }

        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return certificate.GetCertHashString() == hash.Replace(Environment.NewLine, string.Empty);
        }

        public bool CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return certificate.GetCertHashString() == hash.Replace(Environment.NewLine, string.Empty);
        }
    }

    public class TrustX509Certificate2Policy : ICertificatePolicy
    {
        private string thumbprint = string.Empty;

        public string ThumbPrint
        {
            get { return thumbprint; }
        }

        public TrustX509Certificate2Policy(string thumbprint)
        {
            this.thumbprint = thumbprint;
        }

        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return (certificate as X509Certificate2).Thumbprint == thumbprint.Replace(Environment.NewLine, string.Empty);
        }

        public bool CertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return (certificate as X509Certificate2).Thumbprint == thumbprint.Replace(Environment.NewLine, string.Empty);
        }
    }
}