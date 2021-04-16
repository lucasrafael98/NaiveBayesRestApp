using System;
using PriberamRestApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriberamRestApp.Classification
{
    public class Classifier
    {
        private static Classifier instance = new Classifier();

        public static Classifier Instance
        {
            get
            {
                return instance;
            }
        }

        public enum Topic
        {
            None,
            business,
            entertainment,
            politics,
            sports,
            tech
        }

        // This value is used for cases when a word being tested isn't in training data.
        private static float NotTrainedProbability = 1e-2f;

        // A list of frequency dictionaries for each topic.
        // The dictionary key is each word, and the list contained stores
        // three integers: frequency total, frequency per document, and occurrence.
        private List<Dictionary<String, List<float>>> Frequencies = new();
        private List<int> DocumentsTrained = new();

        private Classifier()
        {
            // Creating a dictionary for this topic. 
            // This is done as such in order to allow for any number of new topics to be added.
            foreach(Topic topic in Enum.GetValues(typeof(Topic))){
                Frequencies.Add(new Dictionary<string, List<float>>());
                DocumentsTrained.Add(0);
            }
        }

        private Dictionary<String, int> CountWordOccurrences(String text)
        {
            Dictionary<String, int> wordOccurrences = new();

            String[] words = text.Split(
                new char[] { '.', '?', '!', ' ', ';', ':', ',', '-', '"' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (String word in words)
            {
                if (wordOccurrences.ContainsKey(word))
                {
                    wordOccurrences[word] += 1;
                } else
                {
                    wordOccurrences[word] = 1;
                }
            }

            return wordOccurrences;
        }

        /*
         * Trains the classifier using the given document.
         * 
         * We update the frequencies of each word found in the given text.
         */
        public void Train(TrainingDocument document)
        {
            int topic = (int) Enum.Parse(typeof(Topic), document.Topic);

            Dictionary<String, int> wordOccurrences = CountWordOccurrences(document.Text);

            DocumentsTrained[topic] += 1;
            foreach (var item in wordOccurrences)
            {
                if (Frequencies[topic].ContainsKey(item.Key)) {
                    Frequencies[topic][item.Key][0] += item.Value;
                    Frequencies[topic][item.Key][1] += 1;
                    Frequencies[topic][item.Key][2] = Frequencies[topic][item.Key][1] / DocumentsTrained[topic];
                } else
                {
                    Frequencies[topic][item.Key] = new List<float>();
                    Frequencies[topic][item.Key].Add(item.Value);
                    Frequencies[topic][item.Key].Add(1);
                    Frequencies[topic][item.Key].Add(1.0f);
                }
            }
        }

        /*
         * Classifies the given document, returning a topic.
         */
        public Topic Classify(TestDocument document)
        {
            Topic classifiedTopic = Topic.None;
            double maxProbability = 0.0;
            double currentProbability = 1.0;

            String[] words = document.Text.Split(
                new char[] { '.', '?', '!', ' ', ';', ':', ',', '-', '"' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (int topic in Enum.GetValues(typeof(Topic)))
            {
                // skipping the first topic, as it's just there as a default value.
                if (topic == 0)
                {
                    continue;
                }
                foreach(String word in words)
                {
                    if (Frequencies[topic].ContainsKey(word))
                    {
                        currentProbability *= Frequencies[topic][word][2];
                    } else
                    {
                        currentProbability *= NotTrainedProbability;
                    }
                }
                if(currentProbability > maxProbability)
                {
                    maxProbability = currentProbability;
                    classifiedTopic = (Topic)topic;
                }
            }
            return classifiedTopic;
        }

    }
}
