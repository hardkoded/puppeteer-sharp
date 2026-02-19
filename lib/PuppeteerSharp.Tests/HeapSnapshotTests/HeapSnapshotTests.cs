using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.HeapSnapshotTests
{
    public class HeapSnapshotTests : PuppeteerPageBaseTest
    {
        public HeapSnapshotTests() : base()
        {
        }

        [Test, PuppeteerTest("heapSnapshot.spec", "Heap Snapshot", "should capture heap snapshot")]
        public async Task ShouldCaptureHeapSnapshot()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            try
            {
                var filePath = Path.Combine(tempDir, "heap.heapsnapshot");

                await Page.CaptureHeapSnapshotAsync(new HeapSnapshotOptions { Path = filePath });

                Assert.That(File.Exists(filePath), Is.True);
                var content = File.ReadAllText(filePath);
                var snapshot = JsonSerializer.Deserialize<JsonElement>(content);
                Assert.That(snapshot.TryGetProperty("snapshot", out _), Is.True);
                Assert.That(snapshot.TryGetProperty("nodes", out _), Is.True);
                Assert.That(snapshot.TryGetProperty("edges", out _), Is.True);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
