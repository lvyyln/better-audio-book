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
        Task<byte[]> DownloadFile(string url);
    }

    public class RanobeDataScrapper : IRanobeDataScrapper
    {
        private readonly HtmlWeb _htmlWeb;

        private const string AudioUrl =
            "https://tts.voicetech.yandex.net/generate?key=22fe10e2-aa2f-4a58-a934-54f2c1c4d908&text={0}&format=mp3&lang=ru-RU&speed=1&emotion=neutral&speaker=ermilov&robot=1";

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

        public async Task<byte[]> DownloadFile(string url)
        {
            var client = new HttpClient();
            var data = await Task.WhenAll(url.Replace("\n", "").Split(".")
                .Select((rec) =>
                {
                    if (string.IsNullOrWhiteSpace(rec)) 
                        rec = " ";
                    var uri = String.Format(AudioUrl, rec);
                    return client.GetByteArrayAsync(uri);
                }));
            return data.Aggregate((f, s) => Combine(f, s));
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

            var chapterName = htmlNodes.FirstOrDefault()?.InnerHtml;
            var chapterNumber = idx;
            var chapterContent = String.Join("\n", htmlNodes.Select(rec => rec.InnerHtml));

            return Result.Success(new RanobeText
            {
                ChapterName = chapterName,
                ChapterNumber = chapterNumber,
                Content = chapterContent
            });
        }

        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }

            return rv;
        }
    }
}