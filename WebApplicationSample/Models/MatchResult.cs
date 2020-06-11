using System.Collections.Generic;

namespace WebApplicationSample.Models
{
    public class MatchResult
    {   
        public MatchEntity Match { get; set; }
        public List<MatchOccurrence> Occurrences { get; set; }

        public MatchResult(MatchEntity match)
        {
            Match = match;
            Occurrences = new List<MatchOccurrence>();
        }
    }
}