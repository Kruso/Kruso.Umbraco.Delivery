using Kruso.Umbraco.Delivery.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Kruso.Umbraco.Delivery.Security
{
    public class CertificateHandler : ICertificateHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CertificateHandler> _log;

        public CertificateHandler(IServiceProvider serviceProvider, ILogger<CertificateHandler> log) 
        {
            _serviceProvider = serviceProvider;
            _log = log;
        }

        public X509Certificate2 GetCertificate()
        {
            return GetCertificateByThumbprint()
                ?? GetEmbeddedCertificate()
                ?? GetFileCertificate();
        }

        private X509Certificate2 GetEmbeddedCertificate()
        {
            try
            {
                var config = GetDeliConfig().Get();
                if (!string.IsNullOrEmpty(config.CertificateResourceName) && !string.IsNullOrEmpty(config.CertificatePassword))
                {
                    using (var certStream = Assembly.GetEntryAssembly().GetManifestResourceStream(config.CertificateResourceName))
                    {
                        var rawBytes = new byte[certStream.Length];
                        for (var index = 0; index < certStream.Length; index++)
                        {
                            rawBytes[index] = (byte)certStream.ReadByte();
                        }

                        return new X509Certificate2(rawBytes, config.CertificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                    }
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to load embedded certificate");
            }

            return null;
        }

        private X509Certificate2 GetFileCertificate()
        {
            try
            {
                var config = GetDeliConfig().Get();
                if (!string.IsNullOrEmpty(config.CertificateFileName) && !string.IsNullOrEmpty(config.CertificatePassword))
                {
                    return new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory + config.CertificateFileName, config.CertificatePassword);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to load file certificate");
            }

            return null;
        }

        private X509Certificate2 GetCertificateByThumbprint()
        {
            try
            {
                var config = GetDeliConfig().Get();
                if (!string.IsNullOrEmpty(config.CertificateThumbprint))
                {
                    var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                    try
                    {
                        // Try to open the store.
                        store.Open(OpenFlags.ReadOnly);
                        X509Certificate2Collection certCollection = store.Certificates;
                        // Find currently valid certificates.
                        //X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                        // Find the certificate that matches the thumbprint.
                        X509Certificate2Collection signingCertificates = certCollection.Find(
                            X509FindType.FindByThumbprint, config.CertificateThumbprint, false);

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
            catch (Exception ex)
            {
                _log.LogError(ex, "Failed to load certificate by thumbprint");
            }

            return null;
        }

        private IDeliConfig GetDeliConfig()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetService<IDeliConfig>();
            }
        }
    }
}
