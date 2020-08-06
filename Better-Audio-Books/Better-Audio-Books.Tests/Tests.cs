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

        private const string AudioUrl =
            "https://tts.voicetech.yandex.net/generate?key=22fe10e2-aa2f-4a58-a934-54f2c1c4d908&text={0}&format=mp3&lang=ru-RU&speed=1&emotion=neutral&speaker=ermilov&robot=1";

        private readonly RanobeDataScrapper _scrapper = new RanobeDataScrapper(new HtmlWeb());
        
        [Test]
        public async Task GetBookAsync_WithUrl_IsNotNull()
        {
            var data = await _scrapper.FetchRanobeInfoAsync(RanobeUrl);
            var text = _scrapper.FetchTextFromRanobe(RanobeUrl, data.Value).Value.Select(rec => rec.Content).ToList();
            var tasks = text.Select(rec => _scrapper.DownloadFile(String.Format(AudioUrl, rec)));
            var bytes = string.Join("", tasks.WaitAll());
            Assert.NotNull(data);
            Assert.NotNull(bytes);
        }
    }
}