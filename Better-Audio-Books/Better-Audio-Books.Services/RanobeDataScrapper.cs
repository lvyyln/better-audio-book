using System.Linq;
using System.Threading.Tasks;
using Better_Audio_Books.Services.Common;
using HtmlAgilityPack;

namespace Better_Audio_Books.Services
{
    public interface IRanobeDataScrapper
    {
        Task<Result<RanobeInfo>> FetchRanobeInfoAsync(string url);
        
        Task<Result<RanobeText>> FetchTextFromRanobe(string url);
    }

    public class RanobeDataScrapper : IRanobeDataScrapper
    {
        private readonly HtmlWeb _htmlWeb;

        public RanobeDataScrapper(HtmlWeb htmlWeb) =>
            _htmlWeb = htmlWeb;

        public async Task<Result<RanobeInfo>> FetchRanobeInfoAsync(string url)
        {
            var htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            if (htmlDocument == null) return Result.Failure<RanobeInfo>("Document is empty");

            var ranobeName = htmlDocument.DocumentNode.SelectSingleNode("//div[@class=\"manga-title\"]/h1")?.InnerHtml;
            var ranobePageCount = htmlDocument.DocumentNode
                .SelectNodes("//div[@class=\"info-list manga-info\"]//div[@class=\"info-list__row\"]")
                .FirstOrDefault(rec => rec.SelectSingleNode("strong").InnerHtml.StartsWith("Загружено глав "))
                ?.SelectSingleNode("span").InnerHtml;
            
            return Result.Success(new RanobeInfo(ranobeName, ranobePageCount));
        }

        public Task<Result<RanobeText>> FetchTextFromRanobe(string url)
        {
            throw new System.NotImplementedException();
        }
    }
}