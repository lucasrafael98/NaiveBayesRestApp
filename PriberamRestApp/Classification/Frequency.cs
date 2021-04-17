using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriberamRestApp.Classification
{
    public class Frequency
    {
        // How many times a word appears overall.
        public int FrequencyTotal { get; set; }
        // How many documents contain a word.
        public int FrequencyPerDocument { get; set; }
        // Percentage of a word's occurrence in all documents.
        public double Occurrence { get; set; }

        public Frequency(int frequencyTotal, int frequencyPerDocument, double occurrence)
        {
            FrequencyTotal = frequencyTotal;
            FrequencyPerDocument = frequencyPerDocument;
            Occurrence = occurrence;
        }
    }
}
