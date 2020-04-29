using System;
using System.Collections.Generic;

namespace CuriositySoftware.PageObjects.Entities
{
    public class PageObjectEntity
    {
        public long? id { get; set; }

        public String name { get; set; }

        public String docName { get; set; }

        public String docDescription { get; set; }

        public PageObjectTypeEnum objectType { get; set; }

        public String dataType { get; set; }

        public String iframeXPath { get; set; }

        public String dataAttributes { get; set; }

        public String customObjectType { get; set; }

        public List<PageObjectParameterEntity> parameters { get; set; }

        public List<PageObjectHistoryEntity> pageObjectHistory { get; set; }

        public long? parent { get; set; }

        public List<PageObjectTypeEntity> pageObjectTypes { get; set; }

        public PageObjectEntity()
        {
            parameters = new List<PageObjectParameterEntity>();
        }
    }
}