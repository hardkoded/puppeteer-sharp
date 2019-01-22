using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class FrameTree
    {
        internal FrameTree() => Childs = new List<FrameTree>();

        internal FrameTree(PageGetFrameTreeItem frameTree)
        {
            var frame = frameTree.Frame;

            Frame = new FramePayload
            {
                Id = frame.Id,
                ParentId = frame.ParentId,
                Name = frame.Name,
                Url = frame.Url
            };

            Childs = new List<FrameTree>();
            LoadChilds(this, frameTree);
        }

        #region Properties
        internal FramePayload Frame { get; set; }
        internal List<FrameTree> Childs { get; set; }
        #endregion

        #region Private Functions

        private void LoadChilds(FrameTree frame, PageGetFrameTreeItem frameTree)
        {
            var childFrames = frameTree.ChildFrames;

            if (childFrames != null)
            {
                foreach (var item in childFrames)
                {
                    var childFrame = item.Frame;

                    var newFrame = new FrameTree
                    {
                        Frame = new FramePayload
                        {
                            Id = childFrame.Id,
                            ParentId = childFrame.ParentId,
                            Url = childFrame.Url
                        }
                    };

                    LoadChilds(newFrame, item);
                    frame.Childs.Add(newFrame);
                }
            }
        }

        #endregion
    }
}