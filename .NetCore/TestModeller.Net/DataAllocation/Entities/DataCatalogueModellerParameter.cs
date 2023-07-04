using System;

namespace TestModeller_CSharp.DataAllocation.Entities
{
    public class DataCatalogueModellerParameter
    {
        public enum ModuleParameterDirection
        {
            In,
            Out,
            InOut
        }

        public long? id { get; set; }

        public long? testCriteriaId { get; set; }

        public String name { get; set; }

        public Int32? index { get; set; }

        public ModuleParameterDirection type { get; set; }
    }
}