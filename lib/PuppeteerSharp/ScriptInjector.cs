using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class ScriptInjector
    {
        private static string _injectedSource;
        private readonly object _lock = new();
        private readonly List<string> _amendments = new();
        private bool _updated = false;

        internal static ScriptInjector Default { get; } = new();

        public void Append(string statement)
        {
            lock (_lock)
            {
                _amendments.Add(statement);
                _updated = true;
            }
        }

        public void Pop(string statement)
        {
            lock (_lock)
            {
                _amendments.Remove(statement);
                _updated = true;
            }
        }

        public string Get()
        {
            string amendments;
            lock (_lock)
            {
                amendments = string.Concat(_amendments.Select(statement => $"({statement})(module.exports.default);"));
            }

            return $@"(() => {{
                const module = {{}};
                {GetInjectedSource()}
                {amendments}
                return module.exports.default;
            }})()";
        }

        public async Task InjectAsync(Func<string, Task> inject, bool force = false)
        {
            bool shouldInject;
            lock (_lock)
            {
                shouldInject = _updated || force;
            }

            if (shouldInject)
            {
                await inject(Get()).ConfigureAwait(false);
            }

            lock (_lock)
            {
                _updated = false;
            }
        }

        private static string GetInjectedSource()
        {
            if (string.IsNullOrEmpty(_injectedSource))
            {
                var assembly = Assembly.GetExecutingAssembly();
                const string resourceName = "PuppeteerSharp.Injected.injected.js";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream);
                var fileContent = reader.ReadToEnd();
                _injectedSource = fileContent;
            }

            return _injectedSource;
        }
    }
}
