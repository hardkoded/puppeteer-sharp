using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class FrameTree
    {

        public FrameTree()
        {
            Childs = new List<FrameTree>();
        }

        public FrameTree(dynamic frameTree)
        {
            Frame = new FrameInfo
            {
                Id = frameTree.id,
                ParentId = frameTree.parentId
            };

            LoadChilds(this, frameTree);
        }

        #region Properties
        public FrameInfo Frame { get; set; }
        public List<FrameTree> Childs { get; set; }
        #endregion

        #region Private Functions

        private void LoadChilds(FrameTree frame, dynamic frameTree)
        {
            foreach (dynamic item in frameTree.childs)
            {
                var newFrame = new FrameTree();

                newFrame.Frame = new FrameInfo
                {
                    Id = item.id,
                    ParentId = item.parentID
                };

                LoadChilds(newFrame, item.childs);
                frame.Childs.Add(newFrame);
            }
        }

        #endregion
    }
}