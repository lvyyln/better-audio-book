using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Better_Audio_Books.Services.Common;
using HtmlAgilityPack;
using Microsoft.VisualBasic;

namespace Better_Audio_Books.Services
{
    public interface IRanobeDataScrapper
    {
        Task<Result<RanobeInfo>> FetchRanobeInfoAsync(string url);

        Result<IEnumerable<RanobeText>> FetchTextFromRanobe(string url, RanobeInfo info);
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
            var ranobeData = htmlDocument.DocumentNode
                .SelectNodes("//div[@class=\"chapter-item__name\"]/a[@class=\"link-default\"]");

            var ranobePageCount = ranobeData.Count;
            var ranobeLinks = ranobeData.Select(rec => rec.Attributes["href"].Value);

            return Result.Success(new RanobeInfo(ranobeName, ranobePageCount, ranobeLinks.Reverse()));
        }

        public Result<IEnumerable<RanobeText>> FetchTextFromRanobe(string url, RanobeInfo info)
        {
            var htmlDocuments = info
                .RanobeLinks
                .Select(GetPageAsync)
                .WaitAll()
                .ToList();

            return Result.Success(htmlDocuments.Select(rec => rec.Value));
        }

        
        private async Task<Result<RanobeText>> GetPageAsync(string link, int idx)
        {
            var doc = await _htmlWeb.LoadFromWebAsync(link);
            if (doc == null) return Result.Failure<RanobeText>("Link invalid or website isn't working");

            var htmlNodes =
                doc.DocumentNode?.SelectSingleNode("//div[@class=\"reader-container container container_center\"]")
                    ?.ChildNodes;
            if (htmlNodes == null)
            {
                doc = await _htmlWeb.LoadFromWebAsync(link);
                htmlNodes = doc.DocumentNode
                    ?.SelectSingleNode("//div[@class=\"reader-container container container_center\"]")
                    ?.ChildNodes;
                if (htmlNodes == null) return Result.Failure<RanobeText>("error");
            }
            
            return Result.Success(new RanobeText
            {
                ChapterName = htmlNodes.FirstOrDefault()?.InnerHtml,
                ChapterNumber = idx,
                Content = String.Join("\n", htmlNodes.Select(rec => rec.InnerHtml))
            });
        }

    }
}