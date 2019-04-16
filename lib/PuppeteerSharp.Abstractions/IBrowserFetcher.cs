using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface IBrowserFetcher
    {
        string DownloadsFolder { get; }
        string DownloadHost { get; }
        Platform Platform { get; }
        Task<bool> CanDownloadAsync(int revision);
        IEnumerable<int> LocalRevisions();
        void Remove(int revision);
        RevisionInfo RevisionInfo(int revision);
        Task<RevisionInfo> DownloadAsync(int revision);
        string GetExecutablePath(int revision);
        string GetExecutablePath(Platform platform, int revision, string folderPath);
    }
}
