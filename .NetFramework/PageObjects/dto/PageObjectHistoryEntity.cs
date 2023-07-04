using System;

namespace CuriositySoftware.PageObjects.Entities
{
    public class PageObjectHistoryEntity
    {
        public long? id { get; set; }

        public DateTime latestRun { get; set; }

        public PageObjectParameterStateEnum pageObjectStatus { get; set; }

        public String testGuid { get; set; }

        public String runId { get; set; }

        public String testName { get; set; }

        public long pageObject { get; set; }
    }
}