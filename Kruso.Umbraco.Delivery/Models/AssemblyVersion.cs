using System;

namespace Kruso.Umbraco.Delivery.Models
{
    public class AssemblyVersion
    {
        public string Assembly { get; set; }
        public string Version { get; set; }
        public DateTime BuildTime { get; set; }
    }
}
