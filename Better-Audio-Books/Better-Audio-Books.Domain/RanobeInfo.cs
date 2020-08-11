using System.Collections.Generic;

namespace Better_Audio_Books
{
    public class RanobeInfo
    {
        public RanobeInfo(string ranobeName, int ranobePagesCount, IEnumerable<string> ranobeLinks, string ranobeLanguage = "russian")
        {
            RanobeName = ranobeName;
            RanobePagesCount = ranobePagesCount;
            RanobeLanguage = ranobeLanguage;
            RanobeLinks = ranobeLinks;
        }
        public string RanobeName { get; private set; }
        public string RanobeLanguage { get; private set; }
        public int RanobePagesCount { get; private set; }
        public IEnumerable<string> RanobeLinks { get; private set; }
    }
}