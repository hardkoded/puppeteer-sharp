using System;
using PuppeteerSharp.Tests.Attributes;

namespace CefSharp.DevTools.Dom.Tests
{
    public class SkipIfRunOnAppVeyorFact : PuppeteerFact
    {
        public SkipIfRunOnAppVeyorFact()
        {
            if (Environment.GetEnvironmentVariable("APPVEYOR") == "True")
            {
                Skip = "Running on Appveyor - Test Skipped";
            }
        }
    }
}
