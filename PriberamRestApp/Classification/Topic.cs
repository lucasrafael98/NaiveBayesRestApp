using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriberamRestApp.Classification
{
    public class Topic
    {
        public String topic { get; set; }

        public Topic(String topicName) => topic = topicName;
    }
}
