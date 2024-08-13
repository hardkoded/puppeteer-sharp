using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.SingleFileDeployment
{
    public class PublishSingleFileTests
    {
        public void ShouldWork()
        {
            var tempPath = Path.GetTempPath();
            var actualFilePath = Path.Combine(tempPath, $"google.jpg");
            var actualWindowsBinary = DotnetPublishSingleFile("PuppeteerSharp.Tests.SingleFileDeployment");

            DeleteIfExists(actualFilePath);

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = actualWindowsBinary,
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

            Assert.That(isExited, Is.True);

            Assert.That(File.Exists(actualFilePath), Is.True, $"StdOut: {outputResult}\nStdErr: {errorResult}\n");
        }

        private static string GetStreamOutput(StreamReader stream)
        {
            var outputReadTask = Task.Run(() => stream.ReadToEnd());

            return outputReadTask.Result;
        }

        private static string DotnetPublishSingleFile(string projectName)
        {
            var absolutePath = Path.GetFullPath(Path.Combine("../../../../", projectName));
            var expectedBinaryPath = Path.Combine(absolutePath, $"publish/{projectName}.exe");

            DeleteIfExists(expectedBinaryPath);

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"publish {absolutePath} --configuration Release -p:PublishSingleFile=true --self-contained false --use-current-runtime -o ./publish",
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
            Assert.That(process.WaitForExit(20000), Is.True);

            Assert.That(errorResult, Is.Empty);

            Assert.That(File.Exists(expectedBinaryPath), Is.True, outputResult);

            return expectedBinaryPath;
        }

        private static void DeleteIfExists(string actualFilePath)
        {
            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }
        }
    }
}
