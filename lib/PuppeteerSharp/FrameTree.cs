using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
            var frame = frameTree[Constants.FRAME];

            Frame = new FramePayload
            {
                Id = frame[Constants.ID].Value<string>(),
                ParentId = frame[Constants.PARENT_ID].Value<string>(),
                Name = frame[Constants.NAME].Value<string>(),
                Url = frame[Constants.URL].Value<string>()
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
            var childFrames = frameTree[Constants.CHILD_FRAMES];

            if (childFrames != null)
            {
                foreach (var item in childFrames)
                {
                    var childFrame = item[Constants.FRAME];

                    var newFrame = new FrameTree
                    {
                        Frame = new FramePayload
                        {
                            Id = childFrame[Constants.ID].Value<string>(),
                            ParentId = childFrame[Constants.PARENT_ID].Value<string>(),
                            Url = childFrame[Constants.URL].Value<string>()
                        }
                    };

                    if ((item as JObject)["childFrames"] != null)
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