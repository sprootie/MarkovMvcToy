using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Markov;

namespace Markov.Models
{
    public class MarkovStoryModel
    {
        /// <summary>
        /// Title of the text page
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Blocks of text on the help page
        /// </summary>
        public string[] Text { get; set; }

        public Dictionary<Enums.CorpusIds, string> AvailableCorpuses { get; set; }

        public string SelectedCorpus { get; set; }
    }
}
