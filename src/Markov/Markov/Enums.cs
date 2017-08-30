using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Markov
{
    public class Enums
    {
        public enum CorpusIds : long
        {
            AliceInWonderland = 1,
            ATaleOfTwoCities = 2,
            CatInTheHat = 3,
            HarryPotter = 4,
            Hawking = 5,
            Kafka = 6,
            Lotr = 7,
            Lovecraft = 8,
            OhThePlacesYoullGo = 9,
            ReligiousText = 10,
            TrumpTweets = 11,
            TrumpTweets2017 = 12
        }

        public static Dictionary<CorpusIds, string> CorpusTitles = new Dictionary<CorpusIds, string>
        {
            {CorpusIds.AliceInWonderland, "Lewis Carroll's Alice's Adventures in Wonderland"},
            {CorpusIds.ATaleOfTwoCities, "Charles Dicken's A Tale of Two Cities"},
            {CorpusIds.CatInTheHat, "Dr. Suess's The Cat in the Hat"},
            {CorpusIds.HarryPotter, "J.K. Rowling's Harry Potter series"},
            {CorpusIds.Hawking, "Stephen Hawking's A Brief History of Time"},
            {CorpusIds.Kafka, "Franz Kafka's Collected Works"},
            {CorpusIds.Lotr, "J.R.R. Tolkein's Lord of the Rings series"},
            {CorpusIds.Lovecraft, "H.P. Lovecraft's Collected Works in the Cthulhu Mythos"},
            {CorpusIds.OhThePlacesYoullGo, "Dr. Suess's Oh! The Places You'll Go!"},
            {CorpusIds.ReligiousText, "The Bible (smooth reading edition)"},
            {CorpusIds.TrumpTweets, "All of Tangerine Tweeter's collected tweets"},
            {CorpusIds.TrumpTweets2017, "Tangerine Tweeter's collected tweets from 2017"}
        };
    }
}
