using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Better_Audio_Books.Services;
using Better_Audio_Books.Services.Common;
using HtmlAgilityPack;
using NUnit.Framework;


namespace Better_Audio_Books.Tests
{
    public class Tests
    {
        private const string RanobeUrl = "https://ranobelib.me/the-first-hunter-novel";
        
        private readonly RanobeDataScrapper _scrapper = new RanobeDataScrapper(new HtmlWeb());
        private readonly FileService _fileManager = new FileService();
        
        [Test]
        public async Task GetBookAsync_WithUrl_IsNotNull()
        {
            var data = await _scrapper.FetchRanobeInfoAsync(RanobeUrl);
            var text = _scrapper.FetchTextFromRanobe(RanobeUrl, data.Value).Value.Select(rec => rec.Content).ToList();
            var tasks = await Task.WhenAll(text.Select(rec => _fileManager.DownloadFile(rec)));
            File.WriteAllBytes(@".\test.mp3",tasks.Combine());
            Assert.NotNull(data);
        }
    }
}