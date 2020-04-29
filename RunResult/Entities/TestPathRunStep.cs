using System;

namespace CuriositySoftware.RunResult.Entities
{
    public class TestPathRunStep {
        public long? id { get; set; }

        public String stepName { get; set; }

        public String stepDescription { get; set; }

        public TestPathRunStatus testStatus { get; set; }

        public String message { get; set; }

        public byte[] image { get; set; }

        public String nodeGuid { get; set; }
    }
}