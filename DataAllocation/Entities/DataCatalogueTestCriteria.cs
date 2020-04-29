
using System;
using System.Collections.Generic;

namespace TestModeller_CSharp.DataAllocation.Entities
{
    public class DataCatalogueTestCriteria {
        public long? id { get; set; }

        public long? catalogueId { get; set; }

        public String catalogueName { get; set; }

        public Boolean active { get; set; }
        public String testType { get; set; }
        public String testDescription { get; set; }
        public String sqlCriteria { get; set; }
        public String groupBy { get; set; }
        public String orderBy { get; set; }
        public String tableName { get; set; }
        public String keyNamesSqlOverride { get; set; }
        public String uniqueCheckSqlOverride { get; set; }
        public Int32? defaultHowMany { get; set; }
        public Boolean defaultUnique { get; set; }
        public String expectedResultsSql { get; set; }
        public Boolean exposeAsModuleObject { get; set; }

        public String dataIdentifier { get; set; }
        public String sourceName { get; set; }

        public long? connectionId { get; set; }
        public long? processId { get; set; }
        public long? makeCriteriaId { get; set; }

        public long? moduleObjectId { get; set; }

        public String connectionName { get; set; }

        public String processName { get; set; }

        public String makeCriteriaTestType { get; set; }

        public String makeCriteriaExecutionType { get; set; }

        public List<DataCatalogueKey> keys { get; set; }

        public List<DataCatalogueModellerParameter> modellerParameters { get; set; }

        public Dictionary<String, DataCatalogueModellerParameter> getModellerParameterHash()
        {
            Dictionary<String, DataCatalogueModellerParameter> paramHash = new Dictionary<String, DataCatalogueModellerParameter>();

            foreach (DataCatalogueModellerParameter param in modellerParameters) {
                paramHash.Add(param.name, param);
            }

            return paramHash;
        }
    }
}
