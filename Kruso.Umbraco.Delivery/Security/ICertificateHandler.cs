using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Security
{
    public interface ICertificateHandler
    {
        X509Certificate2 GetCertificate(string thumbprint = null);
    }
}
