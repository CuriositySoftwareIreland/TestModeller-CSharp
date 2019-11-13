using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.DataAllocation.Entities
{
    public class AllocationType
    {
        public String poolName { get; set; }

        public String allocationTestName { get; set; }

        public String suiteName { get; set; }

        public AllocationType(String poolName, String suiteName, String allocationTestName)
        {
            this.poolName = poolName;

            this.suiteName = suiteName;

            this.allocationTestName = allocationTestName;
        }
    }
}
