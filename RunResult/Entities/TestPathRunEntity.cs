using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.RunResult.Entities
{
    public class TestPathRunEntity
    {
        public TestPathRunStatus testStatus { get; set; }

        public String message { get; set; }

        public String vipRunId { get; set; }

        public String lastRunGuid { get; set; }

        public DateTime runTimeStamp { get; set; }

        public String runColId { get; set; }

        public long jobId { get; set; }

        public String testPathGuid { get; set; }

        public int runTime { get; set; }

        public List<TestPathRunStep> testPathRunSteps { get; set; }

        public TestPathRunEntity()
        {
            testPathRunSteps = new List<TestPathRunStep>();
        }

    }
}
