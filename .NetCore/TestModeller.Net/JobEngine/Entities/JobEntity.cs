using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.JobEngine.Entities
{
    public class JobEntity
    {
        public long? id { get; set; }

        public JobState jobState { get; set; }

        public JobType jobType { get; set; }

        public DateTime createdDate { get; set; }

        public DateTime lastUpdate { get; set; }

        public Boolean cancel { get; set; }

        public Boolean expectResult { get; set; }

        public String progressMessage { get; set; }

        public String createdUser { get; set; }

        public String tenant { get; set; }

        public String APIKey { get; set; }

        public Boolean localJob { get; set; }
    }
}
