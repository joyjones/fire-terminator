using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FireTerminator.Common.Transitions
{
    public class TransitionRange : RenderingRectangle
    {
        public TransitionRange(TransitionLine line, ElementTransform trans)
        {
            ParentLine = line;
            OwnerTrans = trans;
            CurFocusRangeOptPart = BodyOperationPart.Nothing;
            UseFrameRectProjectingEffect = true;
        }
        public TransitionLine ParentLine
        {
            get;
            private set;
        }
        public ElementTransform OwnerTrans
        {
            get;
            private set;
        }
        public BodyOperationPart CurFocusRangeOptPart
        {
            get;
            private set;
        }

        public void Update(float elapsedTime)
        {
            float x = OwnerTrans.TimeBegin * ParentLine.ParentDrawer.PixelsPerSecond;
            float y = ParentLine.OffsetY + 1;
            float w = OwnerTrans.TimeLength * ParentLine.ParentDrawer.PixelsPerSecond;
            float h = ParentLine.Height - 2;
            Color clr;
            if (ParentLine.ParentDrawer.SelectedRange == this)
                clr = CommonMethods.ConvertColor(ProjectDoc.Instance.Option.TransitionRangeColorSelected);
            else
                clr = CommonMethods.ConvertColor(ProjectDoc.Instance.Option.TransitionRangeColor);
            base.Update(x, y, w, h, clr);
        }
        public override void Draw(GraphicsDevice device)
        {
            base.Draw(device);
        }
        public BodyOperationPart OnMouseMove(System.Drawing.Point pos)
        {
            CurFocusRangeOptPart = BodyOperationPart.Nothing;
            if (pos.X >= 0 && pos.X < Region.Width && pos.Y >= 0 && pos.Y < Region.Height)
            {
                if (pos.X < 4)
                    CurFocusRangeOptPart = BodyOperationPart.BorderL;
                else if (pos.X >= Region.Width - 4)
                    CurFocusRangeOptPart = BodyOperationPart.BorderR;
                else
                    CurFocusRangeOptPart = BodyOperationPart.Body;
            }
            return CurFocusRangeOptPart;
        }
    }
}
