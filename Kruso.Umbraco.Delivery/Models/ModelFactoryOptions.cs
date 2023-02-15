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
        public string[] IncludeFields { get; set; } = new string[0];
        public string[] ExcludeFields { get; set; } = new string[0];
        public bool ApplyPublicAccessRights { get; set; }
        public IQueryCollection QueryString { get; set; }
        public bool ModifyFields => (IncludeFields?.Any() ?? false) || (ExcludeFields?.Any() ?? false);
    }
}
