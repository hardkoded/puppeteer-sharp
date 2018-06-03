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

        internal FrameTree(dynamic frameTree)
        {
            Frame = new FramePayload
            {
                Id = frameTree.frame.id,
                ParentId = frameTree.frame.parentId,
                Name = frameTree.frame.name,
                Url = frameTree.frame.url
            };

            Childs = new List<FrameTree>();
            LoadChilds(this, frameTree);
        }

        #region Properties
        internal FramePayload Frame { get; set; }
        internal List<FrameTree> Childs { get; set; }
        #endregion

        #region Private Functions

        private void LoadChilds(FrameTree frame, dynamic frameTree)
        {
            if ((frameTree as JObject)["childFrames"] != null)
            {
                foreach (dynamic item in frameTree.childFrames)
                {
                    var newFrame = new FrameTree
                    {
                        Frame = new FramePayload
                        {
                            Id = item.frame.id,
                            ParentId = item.frame.parentId,
                            Url = item.frame.url
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