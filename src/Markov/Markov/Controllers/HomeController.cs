using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Markov.Data;
using Markov.Models;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Markov.Controllers
{
    public class HomeController : Controller
    {
        private Dictionary<Enums.CorpusIds, IMarkovChain> markovCache;
        public HomeController()
        {
            markovCache = new Dictionary<Enums.CorpusIds, IMarkovChain>();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ATaleOfTwoCities()
        {
            var m = GetStoryModel(Enums.CorpusIds.ATaleOfTwoCities);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult AliceInWonderland()
        {
            var m = GetStoryModel(Enums.CorpusIds.AliceInWonderland);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult CatInTheHat()
        {
            var m = GetStoryModel(Enums.CorpusIds.CatInTheHat);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult HarryPotter()
        {
            var m = GetStoryModel(Enums.CorpusIds.HarryPotter);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult Hawking()
        {
            var m = GetStoryModel(Enums.CorpusIds.Hawking);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult Kafka()
        {
            var m = GetStoryModel(Enums.CorpusIds.Kafka);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult LordOfTheRings()
        {
            var m = GetStoryModel(Enums.CorpusIds.Lotr);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult Lovecraft()
        {
            var m = GetStoryModel(Enums.CorpusIds.Lovecraft);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult OhThePlacesYoullGo()
        {
            var m = GetStoryModel(Enums.CorpusIds.OhThePlacesYoullGo);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }
        public ActionResult ReligiousText()
        {
            var m = GetStoryModel(Enums.CorpusIds.ReligiousText);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            ViewBag.StoryText = m.Text;
            return View("MarkovStory");
        }

        public ActionResult TrumpTweets()
        {
            return TrumpTweetsInternal(Enums.CorpusIds.TrumpTweets);
        }

        public ActionResult TrumpTweets2017()
        {
            return TrumpTweetsInternal(Enums.CorpusIds.TrumpTweets2017);
        }

        private ActionResult TrumpTweetsInternal(Enums.CorpusIds id)
        {
            var m = GetStoryModel(id);
            m.AvailableCorpuses = Enums.CorpusTitles;
            ViewBag.StoryTitle = m.Title;
            // special handling to remove tweet ids from tweet text (since each tweet line starts with a tweet id, causing them to end up in the middle of the corpus dictionary
            var rgx = new Regex(@"[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]+");
            for (int i = 0; i < m.Text.Length; i++)
            {
                var wordArr = m.Text[i].Split(new []{ ' '}, StringSplitOptions.RemoveEmptyEntries);
                if (!rgx.IsMatch(wordArr[0]))
                {
                  m.Text[i] = string.Empty;
                }
                else
                {
                  if (wordArr.Length > 1)
                  {
                    for (int j = 1; j < wordArr.Length; j++)
                    {
                      if (rgx.IsMatch(wordArr[j]))
                      {
                        wordArr[j] = string.Empty;
                      }
                    }
                  }
                  m.Text[i] = string.Join(" ", wordArr);
                }
            }

            ViewBag.StoryText = m.Text;
            return View("TweetStory");
        }

        public ActionResult Error()
        {
            return View();
        }
        public MarkovStoryModel GetStoryModel(Enums.CorpusIds id = Enums.CorpusIds.OhThePlacesYoullGo)
        {
            string text = null;
            var title = string.Empty;

            if (!markovCache.ContainsKey(id))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Markov.Data.Corpus." + Enum.GetName(typeof(Enums.CorpusIds), id).ToLower() + ".txt";

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        // Attempt to pick out an appropriate generator based on the size of the selected corpus
                        var sourceSize = stream.Length;
                        if (id == Enums.CorpusIds.TrumpTweets)
                        {
                            markovCache.Add(id, new SecondOrderMarkovChain());
                        }
                        else if (sourceSize > 1000000) //1mb+
                        {
                            markovCache.Add(id, new ThirdOrderMarkovChain());
                        }
                        else if (sourceSize > 20000) //20kb-1mb
                        {
                            markovCache.Add(id, new SecondOrderMarkovChain());
                        }
                        else // <20kb
                        {
                            markovCache.Add(id, new FirstOrderMarkovChain());
                        }
                        markovCache[id].Load(reader.ReadToEnd());
                    }
                }
            }

            var requestedSentences = 6;
            if ((id == Enums.CorpusIds.TrumpTweets) ||
                (markovCache[id].GetType() == typeof(FirstOrderMarkovChain)))
            {
                requestedSentences = 8;
            }

            text = markovCache[id].Generate(requestedSentences);

            // special title 
            if (id == Enums.CorpusIds.HarryPotter)
            {
                title = RandomTitle.Generate();
            }

            var model = new MarkovStoryModel
            {
                Title = title,
                Text = (text ?? string.Empty).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            };

            return model;
        }
    }
}
