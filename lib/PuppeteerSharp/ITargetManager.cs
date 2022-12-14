using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal interface ITargetManager
    {
        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        ConcurrentDictionary<string, Target> GetAvailableTargets();

        ConcurrentDictionary<string, Target> GetAllTargets();

        public Task InitializeAsync();

        public void AddTargetInterceptor(CDPSession session, Action<TargetChangedArgs> interceptor);

        public void RemoveTargetInterceptor(CDPSession session, Action<TargetChangedArgs> interceptor);
    }
}