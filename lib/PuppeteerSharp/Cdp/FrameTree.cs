using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp
{
    internal class FrameTree
    {
        private readonly AsyncDictionaryHelper<string, CdpFrame> _frames = new("Frame {0} not found");
        private readonly ConcurrentDictionary<string, string> _parentIds = new();
        private readonly ConcurrentDictionary<string, List<string>> _childIds = new();
        private readonly ConcurrentDictionary<string, List<TaskCompletionSource<CdpFrame>>> _waitRequests = new();

        public CdpFrame MainFrame { get; set; }

        public CdpFrame[] Frames => _frames.Values.ToArray();

        internal Task<CdpFrame> GetFrameAsync(string frameId) => _frames.GetItemAsync(frameId);

        internal Task<CdpFrame> TryGetFrameAsync(string frameId) => _frames.TryGetItemAsync(frameId);

        internal CdpFrame GetById(string id)
        {
            _frames.TryGetValue(id, out var result);
            return result;
        }

        internal Task<CdpFrame> WaitForFrameAsync(string frameId)
        {
            var frame = GetById(frameId);
            if (frame != null)
            {
                return Task.FromResult(frame);
            }

            var deferred = new TaskCompletionSource<CdpFrame>(TaskCreationOptions.RunContinuationsAsynchronously);
            var callbacks = _waitRequests.GetOrAdd(frameId, static _ => new());
            callbacks.Add(deferred);

            return deferred.Task;
        }

        internal void AddFrame(CdpFrame frame)
        {
            _frames.AddItem(frame.Id, frame);
            if (frame.ParentId != null)
            {
                _parentIds.TryAdd(frame.Id, frame.ParentId);

                var childIds = _childIds.GetOrAdd(frame.ParentId, static _ => new());
                childIds.Add(frame.Id);
            }
            else
            {
                MainFrame = frame;
            }

            _waitRequests.TryGetValue(frame.Id, out var requests);

            if (requests != null)
            {
                foreach (var request in requests)
                {
                    request.TrySetResult(frame);
                }
            }
        }

        internal void RemoveFrame(Frame frame)
        {
            _frames.TryRemove(frame.Id, out _);
            _parentIds.TryRemove(frame.Id, out var _);

            if (frame.ParentId != null)
            {
                _childIds.TryGetValue(frame.ParentId, out var childs);
                childs?.Remove(frame.Id);
            }
            else
            {
                MainFrame = null;
            }
        }

        internal CdpFrame[] GetChildFrames(string frameId)
        {
            _childIds.TryGetValue(frameId, out var childIds);
            if (childIds == null)
            {
                return Array.Empty<CdpFrame>();
            }

            return childIds
                .Select(id => GetById(id))
                .Where(frame => frame != null)
                .ToArray();
        }

        internal Frame GetParentFrame(string frameId)
        {
            _parentIds.TryGetValue(frameId, out var parentId);
            return !string.IsNullOrEmpty(parentId) ? GetById(parentId) : null;
        }
    }
}
