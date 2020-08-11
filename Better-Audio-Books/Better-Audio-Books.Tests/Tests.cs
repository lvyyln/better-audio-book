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
        
        [Test]
        public async Task GetBookAsync_WithUrl_IsNotNull()
        {
            var data = await _scrapper.FetchRanobeInfoAsync(RanobeUrl);
            var text = _scrapper.FetchTextFromRanobe(RanobeUrl, data.Value).Value.Select(rec => rec.Content).ToList();
            var tasks = await Task.WhenAll(text.Select(rec => _scrapper.DownloadFile(rec)));
            File.WriteAllBytes(@".\test.mp3", Combine(tasks));
            Assert.NotNull(data);
        }
        
        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays) {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}