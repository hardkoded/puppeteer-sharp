#pragma warning disable CS0067 // Temporal, do not merge with this
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class ChromeTargetManager : ITargetManager
    {
        private readonly Connection _connection;
        private readonly Func<TargetCreatedResponse, CDPSession, Target> _createTargetFunc;
        private readonly Func<TargetInfo, bool> _targetFilterCallback;
        private readonly ILogger<ChromeTargetManager> _logger;
        private readonly Dictionary<string, Target> _attachedTargetsByTargetId = new();

        public ChromeTargetManager(
            Connection connection,
            Func<TargetCreatedResponse, CDPSession, Target> createTargetFunc,
            Func<TargetInfo, bool> targetFilterCallback)
        {
            _connection = connection;
            _createTargetFunc = createTargetFunc;
            _targetFilterCallback = targetFilterCallback;
            _connection.MessageReceived += Connect_MessageReceived;
            _logger = _connection.LoggerFactory.CreateLogger<ChromeTargetManager>();
        }

        public event EventHandler<TargetChangedArgs> TargetAvailable;

        public event EventHandler<TargetChangedArgs> TargetGone;

        public event EventHandler<TargetChangedArgs> TargetChanged;

        public event EventHandler<TargetChangedArgs> TargetDiscovered;

        internal IDictionary<string, Target> TargetsMap { get; }

        public Dictionary<string, Target> GetAvailableTargets() => _attachedTargetsByTargetId;

        public MessageTask InitializeAsync() => throw new NotImplementedException();

        private async void Connect_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Target.targetCreated":
                        await CreateTargetAsync(e.MessageData.ToObject<TargetCreatedResponse>(true)).ConfigureAwait(false);
                        return;

                    case "Target.targetDestroyed":
                        await DestroyTargetAsync(e.MessageData.ToObject<TargetDestroyedResponse>(true)).ConfigureAwait(false);
                        return;

                    case "Target.targetInfoChanged":
                        ChangeTargetInfo(e.MessageData.ToObject<TargetCreatedResponse>(true));
                        return;
                }
            }
            catch (Exception ex)
            {
                var message = $"Browser failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _connection.Close(message);
            }
        }

        private void ChangeTargetInfo(TargetCreatedResponse targetCreatedResponse) => throw new NotImplementedException();

        private Task DestroyTargetAsync(TargetDestroyedResponse targetDestroyedResponse) => throw new NotImplementedException();

        private Task CreateTargetAsync(TargetCreatedResponse targetCreatedResponse) => throw new NotImplementedException();

        public Dictionary<string, Target> GetAllTargets() => throw new NotImplementedException();

        Task ITargetManager.InitializeAsync() => throw new NotImplementedException();
    }
}
#pragma warning restore CS0067