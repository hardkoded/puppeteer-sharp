using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class FrameTree
    {
        internal FrameTree()
        {
            Childs = new List<FrameTree>();
        }

        internal FrameTree(JToken frameTree)
        {
            var frame = frameTree[MessageKeys.Frame];

            Frame = new FramePayload
            {
                Id = frame[MessageKeys.Id].AsString(),
                ParentId = frame[MessageKeys.ParentId].AsString(),
                Name = frame[MessageKeys.Name].AsString(),
                Url = frame[MessageKeys.Url].AsString()
            };

            Childs = new List<FrameTree>();
            LoadChilds(this, frameTree);
        }

        #region Properties
        internal FramePayload Frame { get; set; }
        internal List<FrameTree> Childs { get; set; }
        #endregion

        #region Private Functions

        private void LoadChilds(FrameTree frame, JToken frameTree)
        {
            var childFrames = frameTree[MessageKeys.ChildFrames];

            if (childFrames != null)
            {
                foreach (var item in childFrames)
                {
                    var childFrame = item[MessageKeys.Frame];

                    var newFrame = new FrameTree
                    {
                        Frame = new FramePayload
                        {
                            Id = childFrame[MessageKeys.Id].AsString(),
                            ParentId = childFrame[MessageKeys.ParentId].AsString(),
                            Url = childFrame[MessageKeys.Url].AsString()
                        }
                    };

                    if ((item as JObject)[MessageKeys.ChildFrames] != null)
                    {
                        LoadChilds(newFrame, item);
                    }

                    frame.Childs.Add(newFrame);
                }
            }
        }

        #endregion
    }
}