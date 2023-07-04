
using System;
using System.Collections.Generic;

namespace TestModeller_CSharp.DataAllocation.Entities
{
    public class AllocatedTest
    {
        public List<AllocatedTestKey> keys { get; set; }

        public List<AllocatedTestParameter> parameters { get; set; }

        public long? id { get; set; }

        public long? poolId { get; set; }

        public Boolean active { get; set; }

        public Boolean prepEnvironment { get; set; }

        public String suiteName { get; set; }

        public String name { get; set; }

        public Boolean uniqueFind { get; set; }

        public Int32? howMany { get; set; }

        public String sourceDatabase { get; set; }

        public String sourceSchema { get; set; }

        public String tableName { get; set; }

        public long? testCriteriaIdCatalogueId { get; set; }

        public long? testCriteriaId { get; set; }
    }
}