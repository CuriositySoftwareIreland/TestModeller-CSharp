using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.DataAllocation.Entities
{
    public class DataAllocationResult
    {
        public List<DataAllocationRow> DataRows { get; set; }

        public DataAllocationResult()
        {
            DataRows = new List<DataAllocationRow>();
        }

        public List<DataAllocationRow> getDataRows()
        {
            return DataRows;
        }

        public void setDataRows(List<DataAllocationRow> m_DataRows)
        {
            this.DataRows = m_DataRows;
        }

        public List<Object> GetValuesByColumn(String colName)
        {
            List<Object> objects = new List<Object>();

            foreach (DataAllocationRow row in DataRows)
            {
                objects.Add(row[colName]);
            }

            return objects;
        }

        public Object GetValueByColumn(String colName)
        {
            return DataRows.ElementAt(0)[colName];
        }

        public Object GetValueByColumnIndex(int index)
        {
            return (new List<Object>(DataRows.ElementAt(0).Values)).ElementAt(index);
        }
    }
}
