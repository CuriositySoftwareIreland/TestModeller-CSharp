using System;

namespace CuriositySoftware.PageObjects.Entities
{
    public class PageObjectParameterEntity
    {
        public long? id { get; set; }

        public String name { get; set; }

        public VipAutomationSelectorEnum paramType { get; set; }

        public Boolean preferred { get; set; }

        public String paramValue { get; set; }

        public double confidence { get; set; }

        public String objectName { get; set; }

        public String moduleName { get; set; }

        public long? pageObject { get; set; }

        public PageObjectParameterStateEnum parameterState { get; set; }
    }
}