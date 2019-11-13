using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.TestModeller
{
    public class TestModellerModuleId : Attribute
    {
        public String guid { get; set; }

        public TestModellerModuleId(String guid)
        {
            this.guid = guid;
        }
    }
}
