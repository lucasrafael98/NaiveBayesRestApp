using System;
using PriberamRestApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using GroBuf;
using GroBuf.DataMembersExtracters;

namespace PriberamRestApp.Classification
{
    public class Classifier
    {
        private static Classifier instance = new Classifier();
        
        // There should only be one classifier whose methods are
        // accessed from different parts of the API, so we use
        // a Singleton design pattern.
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
            sport,
            tech
        }

        // This value is used for cases when a word being tested isn't in training data.
        // (It corresponds to log(1e-2). Please check the description for ClassifyAsync().)
        private const float NotTrainedProbability = -2f;
        // When the application exits, the classifier state (Frequencies) is saved in this file.
        private const String SaveFileName = "frequencies.bin";

        // A list of frequency dictionaries for each topic.
        // The dictionary key is each word.
        private List<Dictionary<String, Frequency>> Frequencies = new();

        // Having a list of locks for each separate dictionary allows us to train different topics concurrently.
        private List<object> FrequencyLocks = new List<object>();
        private List<int> DocumentsTrained = new();

        // Serializer used to keep the classifier state even if the program shuts down arbitrarily
        // (from https://www.nuget.org/packages/GroBuf/1.7.3)
        private Serializer serializer = new Serializer(new PropertiesExtractor(), options: GroBufOptions.WriteEmptyObjects);

        private Classifier()
        {
            // Creating a dictionary for this topic. 
            // This is done as such in order to allow for any number of new topics to be added.
            foreach(Topic topic in Enum.GetValues(typeof(Topic))){
                Frequencies.Add(new Dictionary<string, Frequency>());
                FrequencyLocks.Add(new object());
                DocumentsTrained.Add(0);
            }
            // Checking if there's an existing save state for the classifier. If so, it's deserialized.
            if (File.Exists(SaveFileName))
            {
                Frequencies = serializer.Deserialize<List<Dictionary<String, Frequency>>>(File.ReadAllBytes(SaveFileName));
            }
        }

        /*
         * Returns a frequency dictionary for a single document.
         * 
         * It starts by removing punctuation and stop words, and 
         * only then does it count word occurrences.
         */
        private Dictionary<String, int> CountWordOccurrences(String text)
        {
            Dictionary<String, int> wordOccurrences = new();

            String[] words = text.Split(
                new char[] { '.', '?', '!', ' ', ';', ':', ',', '-', '\"', '\n', '\\' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (String word in words)
            {
                // Naturally, "Search" and "search" shouldn't be considered different words.
                String wordLowerCase = word.ToLower();
                // Stop word removal
                if(StopWords.StopWordDictionary.ContainsKey(wordLowerCase)){
                    continue;
                }
                if (wordOccurrences.ContainsKey(wordLowerCase))
                {
                    wordOccurrences[wordLowerCase] += 1;
                } else
                {
                    wordOccurrences[wordLowerCase] = 1;
                }
            }

            return wordOccurrences;
        }

        /*
         * Trains the classifier using the given document.
         * 
         * We update the frequencies of each word found in the given text.
         * At the end we serialize the current classifier state so that 
         * in case of a shutdown, the classifier retains its state.
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
                            = (float) Frequencies[topic][item.Key].FrequencyPerDocument
                            / DocumentsTrained[topic];
                    }
                    else
                    {
                        Frequencies[topic][item.Key] = new Frequency(item.Value, 1, 1.0);
                    }
                }
                SaveClassifierState();
            }

        }

        /*
         * Classifies the given document, returning a topic.
         * 
         * Instead of multiplying the probabilities for each word
         * (which is very likely to lead to floating-point underflow, 
         * and does so in the example dataset) we apply a logarithm to 
         * each multiplication, turning a product operation into a 
         * sum and avoiding underflow.
         */
        public Task<Topic> ClassifyAsync(TestDocument document)
        {
            Topic classifiedTopic = Topic.None;
            // Since we're using sums of logarithms, the "probability"
            // is likely negative, and certainly not between 0 and 1.
            double maxProbability = double.MinValue;
            double currentProbability;

            String[] words = document.Text.Split(
                new char[] { '.', '?', '!', ' ', ';', ':', ',', '-', '\"', '\n', '\\' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (int topic in Enum.GetValues(typeof(Topic)))
            {
                // skipping the first topic, as it's just there as a default value.
                if (topic == 0)
                {
                    continue;
                }
                currentProbability = 1.0;
                foreach(String word in words)
                {
                    if (Frequencies[topic].ContainsKey(word))
                    {
                        currentProbability += Math.Log(Frequencies[topic][word].Occurrence);
                    } else
                    {
                        currentProbability += NotTrainedProbability;
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

        /*
         * Serializes the current classifier state and writes the byte buffer to a file.
         */
        public void SaveClassifierState()
        {
            byte[] data = serializer.Serialize(Frequencies);
            File.WriteAllBytes(SaveFileName, data);
        }
    }

}
