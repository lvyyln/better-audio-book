using System.Collections.Generic;
using System.Threading.Tasks;

namespace Better_Audio_Books.Services.Common
{
    public static class TaskHelpers
    {
        public static IEnumerable<T> WaitAll<T>(this IEnumerable<Task<T>> source,
            bool continueOnCapturedContext = false)
        {
            return Task.WhenAll(source).ConfigureAwait(continueOnCapturedContext).GetAwaiter().GetResult();
        }
    }
}