using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Security
{
    public class DeliveryClaimTypes
    {
        public const string PreviewId = "preview-id";
        public const string PreviewCulture = "preview-culture";
    }

    public enum UserType
    {
        Visitor,
        BackOffice
    }
}
