using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.SingleFileDeployment
{
    public class PublishedSelfContainedBinary
    {
        [FactRunableOnWindows]
        public void PublishedSelfContainedBinaryShouldWork()
        {
            var tempPath = Path.GetTempPath();
            var actualFilePath = Path.Combine(tempPath, $"google.jpg");

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            var acutualWindowsBinary = DotnetPublishFolderProfileWindows("PuppeteerSharp.SingleFileDeployment");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = acutualWindowsBinary,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = tempPath
                }
            };

            process.Start();
            var outputResult = GetStreamOutput(process.StandardOutput);
            var errorResult = GetStreamOutput(process.StandardError);
            var isExited = process.WaitForExit(80000);

            if (!isExited)
            {
                process.Kill();
            }

            Assert.True(isExited);

            Assert.True(File.Exists(actualFilePath), $"StdOut: {outputResult}\nStdErr: {errorResult}\n");
        }

        private static string GetStreamOutput(StreamReader stream)
        {
            var outputReadTask = Task.Run(() => stream.ReadToEnd());

            return outputReadTask.Result;
        }

        private static string DotnetPublishFolderProfileWindows(string projectName)
        {
            var absolutePath = Path.GetFullPath("../../../../" + projectName);
            var expectedBinaryPath = Path.Combine(absolutePath, $"bin/Release/net6.0/publish/{projectName}.exe");

            if (File.Exists(expectedBinaryPath))
            {
                File.Delete(expectedBinaryPath);
            }

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"publish {absolutePath} --configuration Release -f net6.0 -p:PublishProfile=FolderProfile",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = absolutePath
                }
            };
            process.Start();

            var outputResult = GetStreamOutput(process.StandardOutput);
            var errorResult = GetStreamOutput(process.StandardError);
            Assert.True(process.WaitForExit(20000));

            Assert.Empty(errorResult);

            Assert.True(File.Exists(expectedBinaryPath), outputResult);

            return expectedBinaryPath;
        }
    }
}
