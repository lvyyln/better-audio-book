using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Better_Audio_Books.Services;
using HtmlAgilityPack;
using NUnit.Framework;


namespace Better_Audio_Books.Tests
{
    public class Tests
    {
        private const string RanobeUrl = "https://ranobelib.me/wujie-shushi-novel";

        private readonly RanobeDataScrapper _scrapper = new RanobeDataScrapper(new HtmlWeb());
        
        [Test]
        public async Task GetBookAsync_WithUrl_IsNotNull()
        {
            var data = await _scrapper.FetchRanobeInfoAsync(RanobeUrl);
            Assert.NotNull(data);
        }
    }
}