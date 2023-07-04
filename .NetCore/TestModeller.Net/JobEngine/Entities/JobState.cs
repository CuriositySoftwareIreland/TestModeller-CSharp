using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuriositySoftware.JobEngine.Entities
{
    public enum JobState
    {
        Created,
        InQueue,
        InProgress,
        Complete,
        Error,
        Cancelled
    }
}
