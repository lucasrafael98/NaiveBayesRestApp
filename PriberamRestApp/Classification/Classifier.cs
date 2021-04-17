using System;
using PriberamRestApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

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
        private const float NotTrainedProbability = 1e-2f;
        // When the application exits, the classifier state (Frequencies) is saved in this file.
        private const String SaveFileName = "frequencies.json";

        // A list of frequency dictionaries for each topic.
        // The dictionary key is each word.
        private List<Dictionary<String, Frequency>> Frequencies = new();
        // Having a list of locks for each separate dictionary allows us to train different topics concurrently.
        private List<object> FrequencyLocks = new List<object>();
        private List<int> DocumentsTrained = new();

        private Classifier()
        {
            // Creating a dictionary for this topic. 
            // This is done as such in order to allow for any number of new topics to be added.
            foreach(Topic topic in Enum.GetValues(typeof(Topic))){
                Frequencies.Add(new Dictionary<string, Frequency>());
                FrequencyLocks.Add(new object());
                DocumentsTrained.Add(0);
            }
        }

        /*
         * Returns a frequency dictionary for a single document.
         */
        private Dictionary<String, int> CountWordOccurrences(String text)
        {
            Dictionary<String, int> wordOccurrences = new();

            String[] words = text.Split(
                new char[] { '.', '?', '!', ' ', ';', ':', ',', '-', '"', '\n' },
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
            lock (FrequencyLocks[topic])
            {
                DocumentsTrained[topic] += 1;
                foreach (var item in wordOccurrences)
                {
                    if (Frequencies[topic].ContainsKey(item.Key))
                    {
                        Frequencies[topic][item.Key].FrequencyTotal += item.Value;
                        Frequencies[topic][item.Key].FrequencyPerDocument += 1;
                        Frequencies[topic][item.Key].Occurrence
                            = Frequencies[topic][item.Key].FrequencyPerDocument
                            / DocumentsTrained[topic];
                    }
                    else
                    {
                        Frequencies[topic][item.Key] = new Frequency(item.Value, 1, 1.0);
                    }
                }
            }

        }

        /*
         * Classifies the given document, returning a topic.
         */
        public Task<Topic> ClassifyAsync(TestDocument document)
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
                        currentProbability *= Frequencies[topic][word].Occurrence;
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
            return Task.FromResult(classifiedTopic);
        }

        public void SaveClassifierState()
        {
            String frequencyString = JsonSerializer.Serialize(Frequencies);
            File.WriteAllText(SaveFileName, frequencyString);
        }
    }

}
