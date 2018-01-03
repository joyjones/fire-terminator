using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.Services;
using FireTerminator.Common.Transitions;
using System.Drawing;
using FireTerminator.Common;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Structures;

namespace FireTerminator.Server
{
    public class UserOperation
    {
        public UserOperation(long userId, ViewportInfo vi)
        {
            UserID = userId;
            Viewport = vi;
            ViewportTime = vi.CurTimeTick;
        }
        public long UserID
        {
            get;
            private set;
        }
        public float ViewportTime
        {
            get;
            private set;
        }
        public ViewportInfo Viewport
        {
            get;
            private set;
        }
        public virtual void Do()
        {}
    }
    public class UserOperation_Create : UserOperation
    {
        public UserOperation_Create(long userId, ViewportInfo vi, ElementCreateInfo info)
            : base(userId, vi)
        {
            Info = info;
        }
        public ElementCreateInfo Info
        {
            get;
            private set;
        }
        public override void Do()
        {
            var rg = ProjectDoc.Instance.ResourceGroups[Info.ResKind];
            var res = rg.GetResourceInfo(Info.ResPathFile);
            if (res == null)
            {
                // TODO: Log Null resources
            }
            else
            {
                var e = ElementInfo.CreateElement(res, Viewport, new System.Drawing.PointF(0, 0));
                e.GUID = Info.GUID;
                e.CreatorId = UserID;
                e.BaseTrans.Copy(Info.TransInfo);
                e.ManualScaleOnSrcBackImage = Info.ManualScaleOnSrcBackImage;
                Viewport.AddElement(e);
            }
        }
    }
    public class UserOperation_Delete : UserOperation
    {
        public UserOperation_Delete(long userId, ViewportInfo vi, Guid guid)
            : base(userId, vi)
        {
            Guid = guid;
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && !e.IsLocked && e.CreatorId == UserID)
            {
                Viewport.RemoveElement(e);
            }
        }
    }
    public class UserOperation_Trans : UserOperation
    {
        public UserOperation_Trans(long userId, ViewportInfo vi, Guid guid, ElementTransitionInfo info, SizeF scale)
            : base(userId, vi)
        {
            Guid = guid;
            Info = info;
            Scale = scale;
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public ElementTransitionInfo Info
        {
            get;
            private set;
        }
        public SizeF Scale
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && e.CreatorId == UserID)
            {
                e.BaseTrans.Copy(Info);
                e.ManualScaleOnSrcBackImage = Scale;
            }
        }
    }

    public class UserOperation_MoveDepth : UserOperation
    {
        public UserOperation_MoveDepth(long userId, ViewportInfo vi, Guid guid, int depth)
            : base(userId, vi)
        {
            Guid = guid;
            Depth = depth;
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public int Depth
        {
            get;
            private set;
        }
        public SizeF Scale
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && e.CreatorId == UserID)
            {
                e.DepthLevel = Depth;
            }
        }
    }

    public class UserOperation_SetFlip : UserOperation
    {
        public UserOperation_SetFlip(long userId, ViewportInfo vi, Guid guid, FlippingState state)
            : base(userId, vi)
        {
            Guid = guid;
            State = state;
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public FlippingState State
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && e.CreatorId == UserID)
            {
                e.CurFlippingState = State;
            }
        }
    }

    public class UserOperation_SetProperty : UserOperation
    {
        public UserOperation_SetProperty(long userId, ViewportInfo vi, Guid guid, string propertyName, object value)
            : base(userId, vi)
        {
            Guid = guid;
            PropertyName = propertyName;
            Value = value;
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public string PropertyName
        {
            get;
            private set;
        }
        public object Value
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && e.CreatorId == UserID)
            {
                var pi = e.GetType().GetProperty(PropertyName);
                if (pi != null)
                {
                    pi.SetValue(e, Value, null);
                }
            }
        }
    }

    public class UserOperation_Drive : UserOperation
    {
        public UserOperation_Drive(long userId, ViewportInfo vi, Guid guid, int x, int y)
            : base(userId, vi)
        {
            Guid = guid;
            Location = new Point(x, y);
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public Point Location
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && e.CreatorId == UserID)
            {
                e.TargetLocation = Location;
            }
        }
    }

    public class UserOperation_HotkeyAnimation : UserOperation
    {
        public UserOperation_HotkeyAnimation(long userId, ViewportInfo vi, Guid guid, float animTime)
            : base(userId, vi)
        {
            Guid = guid;
            AnimTime = animTime;
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public float AnimTime
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && e.CreatorId == UserID)
            {
                e.HotKeyAnimBeginTime = AnimTime;
            }
        }
    }

    public class UserOperation_SetMaskInfo : UserOperation
    {
        public UserOperation_SetMaskInfo(long userId, ViewportInfo vi, Guid guid, Microsoft.Xna.Framework.Vector2[] vecs)
            : base(userId, vi)
        {
            Guid = guid;
            Vecs = vecs;
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public Microsoft.Xna.Framework.Vector2[] Vecs
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && e.CreatorId == UserID)
            {
                ((ElementInfo_Mask)e).SetPosition(Vecs);
            }
        }
    }

    public class UserOperation_SetWaterbagInfo : UserOperation
    {
        public UserOperation_SetWaterbagInfo(long userId, ViewportInfo vi, Guid guid, float width, Microsoft.Xna.Framework.Vector2[] vecs)
            : base(userId, vi)
        {
            Guid = guid;
            Width = width;
            Vecs = vecs;
        }
        public Guid Guid
        {
            get;
            private set;
        }
        public float Width;
        public Microsoft.Xna.Framework.Vector2[] Vecs
        {
            get;
            private set;
        }
        public override void Do()
        {
            var e = Viewport.GetElementInfo(Guid);
            if (e != null && e.CreatorId == UserID)
            {
                ((ElementInfo_Waterbag)e).SetProperties(Width, Vecs);
            }
        }
    }

    public class ViewportUserOperations
    {
        public ViewportUserOperations()
        {
        }
        public void ResetOperations(ViewportInfo vi)
        {
            if (Operations.ContainsKey(vi))
                Operations[vi].Clear();
        }
        public void PushOperation(UserOperation opt)
        {
            Dictionary<float, UserOperation> dict = null;
            if (!Operations.TryGetValue(opt.Viewport, out dict))
            {
                dict = new Dictionary<float, UserOperation>();
                Operations[opt.Viewport] = dict;
            }
            dict[opt.ViewportTime] = opt;

            opt.Do();
        }
        private float m_fLastReplayTick = -1;
        public void Replay(ProjectReferrencer pr, bool bReplay)
        {
            var vi = pr.SpecificViewportInfo;
            vi.IsPlaying = bReplay;
            vi.RemoveAllUnlockElements();
            vi.CurTimeTick = 0;
            m_fLastReplayTick = -1;
        }
        public void UpdateReplay(ViewportInfo vi)
        {
            if (m_fLastReplayTick >= vi.CurTimeTick)
                return;
            Dictionary<float, UserOperation> opts = null;
            if (Operations.TryGetValue(vi, out opts))
            {
                foreach (float t in opts.Keys)
                {
                    if (t > m_fLastReplayTick && t <= vi.CurTimeTick)
                        opts[t].Do();
                }
            }
            m_fLastReplayTick = vi.CurTimeTick;
        }
        public Dictionary<ViewportInfo, Dictionary<float, UserOperation>> Operations = new Dictionary<ViewportInfo, Dictionary<float, UserOperation>>();
    }
}
