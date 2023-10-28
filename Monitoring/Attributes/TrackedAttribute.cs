using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Attributes
{
    public class TrackedAttribute : Attribute
    {
        public string Category;
        public string Name;
    }
}
