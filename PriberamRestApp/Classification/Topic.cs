using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriberamRestApp.Classification
{
    /*
     * Simple class used to send a response to the client when testing the classifier.
     */
    public class Topic
    {
        public String topic { get; set; }

        public Topic(String topicName) => topic = topicName;
    }
}
