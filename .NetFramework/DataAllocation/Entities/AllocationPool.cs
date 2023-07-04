
using System;

namespace TestModeller_CSharp.DataAllocation.Entities
{
    public class AllocationPool
    {
        public long? id { get; set; }

        public String name { get; set; }

        public DateTime processedDate { get; set; }

        public String catalogueName { get; set; }

        public long? catalogueId { get; set; }
    }
}