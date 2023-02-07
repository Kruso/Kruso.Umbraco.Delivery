using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kruso.Umbraco.Delivery.Models
{
    public class ModelFactoryOptions
    {
        public int MaxDepth { get; set; }
        public bool LoadPreview { get; set; }
        public string[] IncludeFields { get; set; }
        public string[] ExcludeFields { get; set; }
        public bool ApplyPublicAccessRights { get; set; }
        public IQueryCollection QueryString { get; set; }
    }
}
