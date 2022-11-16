using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Kruso.Umbraco.Delivery.Security
{
    public class CertificateHandler : ICertificateHandler
    {
        public X509Certificate2 GetCertificate(string thumbprint = null)
        {
            return string.IsNullOrEmpty(thumbprint)
                ? GetEmbeddedCertificate()
                : GetCertificateByThumbprint(thumbprint);
        }

        private X509Certificate2 GetEmbeddedCertificate()
        {
            try
            {
                using (var certStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"Kruso.Umbraco.Delivery.CN=RSKSampleIdentityServer.pfx"))
                {
                    var rawBytes = new byte[certStream.Length];
                    for (var index = 0; index < certStream.Length; index++)
                    {
                        rawBytes[index] = (byte)certStream.ReadByte();
                    }

                    return new X509Certificate2(rawBytes, "Password123!", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                }
            }
            catch (Exception)
            {
                return new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + "CN=RSKSampleIdentityServer.pfx", "Password123!");
            }
        }

        private X509Certificate2 GetCertificateByThumbprint(string certThumbPrint)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                // Try to open the store.
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = store.Certificates;
                // Find currently valid certificates.
                //X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                // Find the certificate that matches the thumbprint.
                X509Certificate2Collection signingCertificates = certCollection.Find(
                    X509FindType.FindByThumbprint, certThumbPrint, false);

                if (signingCertificates.Count == 0)
                    return null;
                return signingCertificates[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
