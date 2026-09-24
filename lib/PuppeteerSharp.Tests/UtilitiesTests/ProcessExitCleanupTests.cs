using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.UtilitiesTests
{
    public class ProcessExitCleanupTests
    {
        [Test]
        public void RemovesATemporaryProfileSynchronouslyOnProcessExit()
        {
            var userDataDir = Directory.CreateTempSubdirectory("puppeteer-process-exit-cleanup-").FullName;
            File.WriteAllText(Path.Combine(userDataDir, "profile"), "profile");
            var processEmitter = new ProcessEmitter();

            ProcessExitCleanup.Register(userDataDir, _ => { }, processEmitter);
            processEmitter.Emit();

            Assert.That(Directory.Exists(userDataDir), Is.False);
        }

        [Test]
        public void CanUnregisterCleanupAfterTheBrowserProcessExits()
        {
            var userDataDir = Directory.CreateTempSubdirectory("puppeteer-process-exit-cleanup-").FullName;
            var processEmitter = new ProcessEmitter();
            var unregister = ProcessExitCleanup.Register(userDataDir, _ => { }, processEmitter);

            unregister();
            processEmitter.Emit();

            Assert.That(Directory.Exists(userDataDir), Is.True);
            Directory.Delete(userDataDir, recursive: true);
        }

        [Test]
        public void UsesOneProcessExitListenerForMultipleTemporaryProfiles()
        {
            var firstDir = Directory.CreateTempSubdirectory("puppeteer-process-exit-cleanup-").FullName;
            var secondDir = Directory.CreateTempSubdirectory("puppeteer-process-exit-cleanup-").FullName;
            var processEmitter = new ProcessEmitter();

            ProcessExitCleanup.Register(firstDir, _ => { }, processEmitter);
            ProcessExitCleanup.Register(secondDir, _ => { }, processEmitter);
            Assert.That(processEmitter.ListenerCount, Is.EqualTo(1));

            processEmitter.Emit();

            Assert.That(Directory.Exists(firstDir), Is.False);
            Assert.That(Directory.Exists(secondDir), Is.False);
        }

        private sealed class ProcessEmitter : IProcessExitEmitter
        {
            private readonly HashSet<Action> _listeners = [];

            public int ListenerCount => _listeners.Count;

            public void Once(Action listener) => _listeners.Add(listener);

            public void Off(Action listener) => _listeners.Remove(listener);

            public void Emit()
            {
                foreach (var listener in _listeners)
                {
                    listener();
                }

                _listeners.Clear();
            }
        }
    }
}
