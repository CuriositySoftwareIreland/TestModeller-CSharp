using System;

namespace CuriositySoftware.PageObjects.Entities
{
    public class PageObjectTypeEntity
    {
        public long id { get; set; }

        public PageObjectTypeEnum objectType { get; set; }

        public String customObjectType { get; set; }

        public long? pageObject { get; set; }
    }
}