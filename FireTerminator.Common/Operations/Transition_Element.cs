using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.Elements;
using FireTerminator.Common.Transitions;

namespace FireTerminator.Common.Operations
{
    public abstract class Transition_Element : Operation
    {
        public Transition_Element(ElementInfo e, TransitionKind kind)
        {
            Element = e;
            Kind = kind;
        }
        public override void Undo() { }
        public override void Do() { }
        public override void Commit() { }
        public override void Merge(Operation opt) { }
        public ElementInfo Element
        {
            get;
            private set;
        }
        public TransitionKind Kind
        {
            get;
            private set;
        }
        public TransitionLine ParentLine
        {
            get
            {
                if (ProjectDoc.Instance.TransGraphics == null)
                    return null;
                return ProjectDoc.Instance.TransGraphics.GetLine(Kind);
            }
        }
        public virtual ElementTransform ResultTransform
        {
            get;
            protected set;
        }
    }

    public class Transition_Element_Add : Transition_Element
    {
        public Transition_Element_Add(ElementInfo e, TransitionKind kind)
            : base(e, kind)
        {
        }
        public Transition_Element_Add(ElementInfo e, ElementTransform trans)
            : base(e, trans.Kind)
        {
            ResultTransform = trans;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            Element.RemoveTransition(ResultTransform);
        }
        public override void Do()
        {
            Element.AddTransition(ResultTransform);
        }
    }

    public class Transition_Element_Create : Transition_Element_Add
    {
        public Transition_Element_Create(ElementInfo e, TransitionKind kind, float timeBegin, float timeLen)
            : base(e, kind)
        {
            ResultTransform = System.Activator.CreateInstance(ElementTransform.TransTypes[Kind], Element, timeBegin, timeLen) as ElementTransform;
        }
    }

    public class Transition_Element_Duplicate : Transition_Element_Add
    {
        public Transition_Element_Duplicate(ElementInfo e, ElementTransform trans, float time)
            : base(e, trans.Kind)
        {
            ResultTransform = System.Activator.CreateInstance(ElementTransform.TransTypes[Kind], Element, time, trans.TimeLength) as ElementTransform;
            ResultTransform.CopyFrom(trans);
        }
    }

    public class Transition_Element_Split : Transition_Element
    {
        public Transition_Element_Split(ElementInfo e, TransitionKind kind, ElementTransform trans, float time)
            : base(e, kind)
        {
            TargetTransform = trans;
            Time = time;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            if (m_SplittedTransform != null)
            {
                Element.RemoveTransition(m_SplittedTransform);
                TargetTransform.Merge(m_SplittedTransform);
            }
        }
        public override void Do()
        {
            ElementTransform trans;
            TargetTransform.Split(Time, out trans);
            if (m_SplittedTransform != null)
                trans.CopyFrom(m_SplittedTransform);
            m_SplittedTransform = trans;
        }
        public float Time
        {
            get;
            protected set;
        }
        public ElementTransform TargetTransform
        {
            get;
            private set;
        }
        private ElementTransform m_SplittedTransform = null;
        public override ElementTransform ResultTransform
        {
            get { return m_SplittedTransform; }
        }
    }

    public class Transition_Element_Delete : Transition_Element
    {
        public Transition_Element_Delete(ElementInfo e, ElementTransform trans)
            : base(e, trans.Kind)
        {
            ResultTransform = trans;
        }
        public override bool IsTimeLimit
        {
            get { return false; }
        }
        public override void Undo()
        {
            Element.AddTransition(ResultTransform);
        }
        public override void Do()
        {
            Element.RemoveTransition(ResultTransform);
        }
    }
}
