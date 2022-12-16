using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;
using PuppeteerSharp.Transport;

namespace PuppeteerSharp
{
    internal delegate void TargetInterceptor(Target createdTarget, Target parentTarget);

    internal interface ITargetManager
    {
        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        ConcurrentDictionary<string, Target> GetAvailableTargets();

        public Task InitializeAsync();

        public void AddTargetInterceptor(CDPSession session, TargetInterceptor interceptor);

        public void RemoveTargetInterceptor(CDPSession session, TargetInterceptor interceptor);
    }
}