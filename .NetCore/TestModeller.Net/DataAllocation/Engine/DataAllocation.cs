using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.DataAllocation.Engine
{
    public class DataAllocation : Attribute
    {
        public String poolName { get; set; }

        public String suiteName { get; set; }

        public String[] groups { get; set; }

        public DataAllocation(String poolName, String suiteName, String[] groups)
        {
            this.poolName = poolName;

            this.suiteName = suiteName;

            this.groups = groups;
        }
    }
}
