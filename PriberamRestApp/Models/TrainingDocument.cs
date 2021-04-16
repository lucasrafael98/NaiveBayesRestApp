using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriberamRestApp.Models
{
    /**
     * Basic training document, following the given JSON definition (text/topic).
    */
    public class TrainingDocument
    {
        public long Id { get; set; }
        public String Text { get; set; }
        public String Topic { get; set; }
    }
}
