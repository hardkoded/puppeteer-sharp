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
        private readonly List<string> _amendments = new();
        private bool _updated = false;

        public void Append(string statement) => Update(() => _amendments.Add(statement));

        public void Pop(string statement) => Update(() => _amendments.Remove(statement));

        public string Get()
        {
            var amendments = string.Join(string.Empty, _amendments.Select(statement => $"({statement})(module.exports.default);"));

            return $@"(() => {{
                const module = {{}};
                {GetInjectedSource()}
                {amendments}
                return module.exports.default;
            }})()";
        }

        public async Task InjectAsync(Func<string, Task> inject, bool force = false)
        {
            if (_updated || force)
            {
                await inject(Get()).ConfigureAwait(false);
            }

            _updated = false;
        }

        private static string GetInjectedSource()
        {
            if (string.IsNullOrEmpty(_injectedSource))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PuppeteerSharp.Injected.injected.js";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream);
                var fileContent = reader.ReadToEnd();
                _injectedSource = fileContent;
            }

            return _injectedSource;
        }

        private void Update(Action callback)
        {
            callback();
            _updated = true;
        }
    }
}
