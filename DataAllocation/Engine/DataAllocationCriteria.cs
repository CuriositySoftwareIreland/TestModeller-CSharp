using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModeller_CSharp.DataAllocation.Engine
{
    public class DataAllocationCriteria
    {
        public String parameterName { get; set; }

        public String parameterValue { get; set; }

        public DataAllocationCriteria(String name, String value)
        {
            parameterName = name;

            parameterValue = value;
        }

    }
}
