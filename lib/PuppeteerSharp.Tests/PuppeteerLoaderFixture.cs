using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests
{
    public class PuppeteerLoaderFixture : IDisposable
    {
        Process _webServerProcess = null;

        public PuppeteerLoaderFixture()
        {
            SetupAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            int processId = 0;

            Console.WriteLine("Attempting to kill process");
            try
            {
                processId = _webServerProcess.Id;
            }
            catch (InvalidOperationException)
            {
                //We might get an InvalidOperationException if the process is in an invalid state
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to kill webserver", ex);
            }

            if (processId != 0 && Process.GetProcessById(processId) != null)
            {
                try
                {
                    Console.WriteLine("Killing process");
                    _webServerProcess.Kill();
                    Console.WriteLine("Process killed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to kill process: {ex.Message}");
                }
            }
        }

        private async Task SetupAsync()
        {
            Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision);
            var downloaderTask = Downloader.CreateDefault().DownloadRevisionAsync(TestConstants.ChromiumRevision);
            var serverTask = StartWebServerAsync();

            await Task.WhenAll(downloaderTask, serverTask);

        }

        private async Task StartWebServerAsync()
        {
            var taskWrapper = new TaskCompletionSource<bool>();
            const int timeout = 2000;

            var build = Directory.GetCurrentDirectory().Contains("Debug") ? "Debug" : "Release";
            var webServerPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..",
                                             "PuppeteerSharp.TestServer");

            _webServerProcess = new Process();
            _webServerProcess.StartInfo.FileName = "dotnet";
            _webServerProcess.StartInfo.WorkingDirectory = webServerPath;
            _webServerProcess.StartInfo.Arguments = $"./bin/{build}/netcoreapp2.0/PuppeteerSharp.TestServer.dll";

            _webServerProcess.StartInfo.RedirectStandardOutput = true;
            _webServerProcess.StartInfo.RedirectStandardError = true;

            _webServerProcess.OutputDataReceived += (sender, e) =>
            {
                Console.WriteLine(e.Data);
                if (e.Data != null &&
                    taskWrapper.Task.Status != TaskStatus.RanToCompletion &&
                    //Though this is not bulletproof for the purpose of local testing
                    //We assume that if the address is already in use is because we have another
                    //process hosting the site
                    (e.Data.Contains("Now listening on") || e.Data.Contains("ADDRINUSE")))
                {
                    taskWrapper.SetResult(true);
                }
            };

            _webServerProcess.Exited += (sender, e) =>
            {
                taskWrapper.SetException(new Exception("Unable to start web server"));
            };

            Timer timer = null;
            //We have to declare timer before initializing it because if we don't do this 
            //we can't dispose it in the action created in the constructor
            timer = new Timer((state) =>
            {
                if (taskWrapper.Task.Status != TaskStatus.RanToCompletion)
                {
                    taskWrapper.SetException(
                        new Exception($"Timed out after {timeout} ms while trying to connect to WebServer! "));
                }
                timer.Dispose();
            }, null, timeout, 0);

            _webServerProcess.Start();
            _webServerProcess.BeginOutputReadLine();

            await taskWrapper.Task;
        }

    }
}
