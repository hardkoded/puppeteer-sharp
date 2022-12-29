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
        event EventHandler<TargetChangedArgs> TargetAvailable;

        event EventHandler<TargetChangedArgs> TargetGone;

        event EventHandler<TargetChangedArgs> TargetChanged;

        event EventHandler<TargetChangedArgs> TargetDiscovered;

        ConcurrentDictionary<string, Target> GetAvailableTargets();

        Task InitializeAsync();

        void AddTargetInterceptor(CDPSession session, TargetInterceptor interceptor);

        void RemoveTargetInterceptor(CDPSession session, TargetInterceptor interceptor);
    }
}