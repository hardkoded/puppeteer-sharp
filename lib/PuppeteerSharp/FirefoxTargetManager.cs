#pragma warning disable CS0067 // Temporal, do not merge with this
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class FirefoxTargetManager : ITargetManager
    {
        private readonly Connection _connection;
        private readonly Func<TargetCreatedResponse, CDPSession, Target> _createTargetFunc;
        private readonly Func<TargetInfo, bool> _targetFilterFunc;

        public FirefoxTargetManager(
            Connection connection,
            Func<TargetCreatedResponse, CDPSession, Target> createTargetFUNC,
            Func<TargetInfo, bool> targetFilterFunc)
        {
            _connection = connection;
            _createTargetFunc = createTargetFUNC;
            _targetFilterFunc = targetFilterFunc;
        }

        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        public Task InitializeAsync() => throw new NotImplementedException();

        public void AddTargetInterceptor(CDPSession session, Action<TargetChangedArgs> interceptor) => throw new NotImplementedException();

        public void RemoveTargetInterceptor(CDPSession session, Action<TargetChangedArgs> interceptor) => throw new NotImplementedException();

        ConcurrentDictionary<string, Target> ITargetManager.GetAvailableTargets() => throw new NotImplementedException();

        ConcurrentDictionary<string, Target> ITargetManager.GetAllTargets() => throw new NotImplementedException();
    }
}
#pragma warning restore CS0067