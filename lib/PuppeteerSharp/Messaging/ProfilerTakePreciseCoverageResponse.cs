namespace PuppeteerSharp.Messaging
{
    internal class ProfilerTakePreciseCoverageResponse
    {
        public ProfilerTakePreciseCoverageResponseItem[] Result { get; set; }

        internal class ProfilerTakePreciseCoverageResponseItem
        {
            public string ScriptId { get; set; }
            public ProfilerTakePreciseCoverageResponseFunction[] Functions { get; set; }
        }

        internal class ProfilerTakePreciseCoverageResponseFunction
        {
            public CoverageResponseRange[] Ranges { get; set; }
        }
    }
}
