namespace Better_Audio_Books
{
    public class RanobeInfo
    {
        public RanobeInfo(string ranobeName, string ranobePagesCount, string ranobeLanguage = "russian")
        {
            RanobeName = ranobeName;
            RanobePagesCount = ranobePagesCount;
            RanobeLanguage = ranobeLanguage;
        }
        public string RanobeName { get; private set; }
        public string RanobeLanguage { get; private set; }
        public string RanobePagesCount { get; private set; }
    }
}