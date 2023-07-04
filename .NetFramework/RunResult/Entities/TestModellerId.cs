using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.RunResult.Entities
{
    public class TestModellerId : Attribute
    {
        public String guid { get; set; }

        public TestModellerId(String guid)
        {
            this.guid = guid;
        }
    }
}
