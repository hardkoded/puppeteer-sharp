using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal interface ITargetManager
    {
        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        Dictionary<string, Target> GetAvailableTargets();

        Dictionary<string, Target> GetAllTargets();

        public Task InitializeAsync();
    }
}