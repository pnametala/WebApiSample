using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApplicationSample.Models
{
    public class MatchEntity
    {
        private string _subtext;

        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Subtext
        {
            get => _subtext;
            set => _subtext = Regex.Escape(value);
        }
        
        /// <summary>
        /// Uses regex to find all occurrences of a given pattern in a string.
        /// </summary>
        /// <returns>MatchCollection</returns>
        public MatchCollection FindOccurrences()
        {
            return string.IsNullOrEmpty(Subtext) ? new Regex(" ").Matches(string.Empty) : new Regex(Subtext, RegexOptions.IgnoreCase).Matches(Text);
        }
    }
}