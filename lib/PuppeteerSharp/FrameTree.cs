using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal class FrameTree
    {
        private readonly ConcurrentDictionary<string, Frame> _frames;
        private readonly ConcurrentDictionary<string, string> _parentIds = new();
        private readonly ConcurrentDictionary<string, List<string>> _childIds = new();
        private readonly ConcurrentDictionary<string, List<TaskCompletionSource<Frame>>> _waitRequests = new();
        private readonly AsyncDictionaryHelper<string, Frame> _asyncFrames;

        public FrameTree()
        {
            _frames = new();
            _asyncFrames = new AsyncDictionaryHelper<string, Frame>(_frames, "Frame {0} not found");
        }

        public Frame MainFrame { get; set; }

        public Frame[] Frames => _frames.Values.ToArray();

        internal Task<Frame> GetFrameAsync(string frameId) => _asyncFrames.GetItemAsync(frameId);

        internal Task<Frame> TryGetFrameAsync(string frameId) => _asyncFrames.TryGetItemAsync(frameId);

        internal Frame GetById(string id)
        {
            _frames.TryGetValue(id, out var result);
            return result;
        }

        internal Task<Frame> WaitForFrameAsync(string frameId)
        {
            var frame = GetById(frameId);
            if (frame != null)
            {
                return Task.FromResult(frame);
            }

            var deferred = new TaskCompletionSource<Frame>(TaskCreationOptions.RunContinuationsAsynchronously);
            _waitRequests.TryAdd(frameId, new List<TaskCompletionSource<Frame>>());
            _waitRequests.TryGetValue(frameId, out var callbacks);
            callbacks.Add(deferred);

            return deferred.Task;
        }

        internal void AddFrame(Frame frame)
        {
            _asyncFrames.AddItem(frame.Id, frame);
            if (frame.ParentId != null)
            {
                _parentIds.TryAdd(frame.Id, frame.ParentId);

                _childIds.TryAdd(frame.ParentId, new List<string>());
                _childIds.TryGetValue(frame.ParentId, out var childIds);
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
            _frames.TryRemove(frame.Id, out var _);
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

        internal Frame[] GetChildFrames(string frameId)
        {
            _childIds.TryGetValue(frameId, out var childIds);
            if (childIds == null)
            {
                return Array.Empty<Frame>();
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
