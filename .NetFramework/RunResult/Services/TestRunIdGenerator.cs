using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.RunResult.Services
{
    public class TestRunIdGenerator
    {
        public static String GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
