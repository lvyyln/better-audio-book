using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Better_Audio_Books.Services.Common;

namespace Better_Audio_Books.Services
{
    public interface IRanobeScrapper
    {
        Task<byte[]> DownloadFile(string url);
    }

    public class RanobeScrapper
    {
        private const string AudioUrl =
            "https://tts.voicetech.yandex.net/generate?key=22fe10e2-aa2f-4a58-a934-54f2c1c4d908&text={0}&format=mp3&lang=ru-RU&speed=1&emotion=neutral&speaker=ermilov&robot=1";

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