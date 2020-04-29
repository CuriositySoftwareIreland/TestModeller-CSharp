
using System;

namespace TestModeller_CSharp.DataAllocation.Entities
{
    public class AllocatedTestParameter
    {
        public long? id { get; set; }

        public long? testId { get; set; }

        public long? criteriaParameterId { get; set; }

        public String value { get; set; }

        public String criteriaParameterName { get; set; }
    }
}