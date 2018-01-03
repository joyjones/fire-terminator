using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireTerminator.Common.Operations
{
    public abstract class Operation
    {
        public abstract void Undo();
        public abstract void Do();
        public virtual void TryDo() { Do(); }
        public abstract void Merge(Operation opt);
        public abstract void Commit();
        public virtual bool IsTimeLimit
        {
            get { return true; }
        }
        public bool LinkNext
        {
            get;
            set;
        }
    }

    public class OperationHistory
    {
        private OperationHistory()
        {
            IsDirty = false;
            IsServerMonitoring = false;
            LastNewOperationTime = 0;
            IsInTransaction = false;
        }
        public static OperationHistory Instance
        {
            get { return m_Instance; }
        }
        public int CurOperationIndex
        {
            get
            {
                if (CurOperation == null)
                    return -1;
                return m_Oprations.IndexOf(CurOperation);
            }
        }
        public int CurOperationsCount
        {
            get { return m_Oprations.Count; }
        }
        public bool CanUndo
        {
            get { return CurOperationIndex >= 0; }
        }
        public bool CanRedo
        {
            get { return CurOperationIndex < CurOperationsCount - 1; }
        }
        public Operation CurOperation
        {
            get { return m_CurOperation; }
            set
            {
                if (m_CurOperation != value)
                {
                    m_CurOperation = value;
                    if (CurOperationChanged != null)
                        CurOperationChanged(m_CurOperation);
                }
            }
        }
        public bool IsDirty
        {
            get;
            set;
        }
        public bool IsServerMonitoring
        {
            get;
            set;
        }
        public bool IsInternalOperating
        {
            get { return m_Operating; }
        }
        public long LastNewOperationTime
        {
            get;
            private set;
        }
        public bool IsInTransaction
        {
            get;
            private set;
        }

        public void Clear()
        {
            m_NewOperation = null;
            m_CurOperation = null;
            m_Oprations.Clear();
            IsDirty = false;
        }
        public void Update()
        {
            if (NewOperationPushed != null && m_SuspendedOperation != null)
            {
                var time = new TimeSpan(DateTime.Now.Ticks - LastNewOperationTime);
                if (time.Milliseconds >= ProjectDoc.Instance.Option.OperationSyncTimeInterval)
                {
                    NewOperationPushed(m_SuspendedOperation);
                    m_SuspendedOperation = null;
                }
            }
        }
        private bool m_Operating = false;
        public void PushOperation(Operation opt)
        {
            if (m_Operating)
                return;
            m_Operating = true;
            try
            {
                if (m_NewOperation == null || m_NewOperation.GetType() != opt.GetType())
                    m_NewOperation = opt;
                else
                    m_NewOperation.Merge(opt);
                m_NewOperation.TryDo();
                if (IsInTransaction)
                    m_NewOperation.LinkNext = true;

                if (NewOperationPushed != null)
                {
                    if (!m_NewOperation.IsTimeLimit)
                    {
                        if (m_SuspendedOperation != null)
                            NewOperationPushed(m_SuspendedOperation);
                        m_SuspendedOperation = null;
                        NewOperationPushed(m_NewOperation);
                    }
                    else
                    {
                        long tick = DateTime.Now.Ticks;
                        var time = new TimeSpan(tick - LastNewOperationTime);
                        if (time.Milliseconds < ProjectDoc.Instance.Option.OperationSyncTimeInterval)
                            m_SuspendedOperation = m_NewOperation;
                        else
                        {
                            if (m_SuspendedOperation != null && m_SuspendedOperation.GetType() != m_NewOperation.GetType())
                                NewOperationPushed(m_SuspendedOperation);
                            m_SuspendedOperation = null;
                            NewOperationPushed(m_NewOperation);
                            LastNewOperationTime = tick;
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                m_Operating = false;
            }
        }
        public void BeginTransaction()
        {
            IsInTransaction = true;
        }
        public void EndTransaction()
        {
            if (IsInTransaction)
            {
                IsInTransaction = false;
                if (m_Oprations.Count > 0)
                {
                    m_Oprations[m_Oprations.Count - 1].LinkNext = false;
                }
            }
        }
        public bool CommitOperation(Operation opt)
        {
            if (m_Operating || opt == null)
                return false;
            m_NewOperation = null;
            PushOperation(opt);
            return CommitOperation();
        }
        public bool CommitOperation()
        {
            if (m_Operating) return false;
            m_Operating = true;
            bool result = false;
            if (m_NewOperation != null)
            {
                int index = CurOperationIndex + 1;
                int count = m_Oprations.Count;
                for (int i = index; i < count; ++i)
                    m_Oprations.RemoveAt(index);
                m_Oprations.Add(m_NewOperation);
                count = m_Oprations.Count;
                for (int i = 0; i < count - ProjectDoc.Instance.Option.MaxOperationHistoryStepsCount; ++i)
                    m_Oprations.RemoveAt(0);
                m_NewOperation.Commit();
                CurOperation = m_NewOperation;
                if (NewOperationCommited != null)
                    NewOperationCommited(m_NewOperation);
                m_NewOperation = null;
                IsDirty = true;
                result = true;
            }
            m_Operating = false;
            return result;
        }
        public void Undo()
        {
            if (CurOperation == null)
                IsInTransaction = false;
            else
            {
                var prevOpt = CurOperationIndex > 0 ? m_Oprations[CurOperationIndex - 1] : null;
                if (prevOpt != null && prevOpt.LinkNext)
                    IsInTransaction = true;
                else
                    IsInTransaction = false;
                CurOperation.Undo();
                if (AfterUndoOperation != null)
                    AfterUndoOperation(CurOperation);
                int index = CurOperationIndex;
                if (index <= 0)
                    CurOperation = null;
                else
                {
                    CurOperation = m_Oprations[index - 1];
                    if (IsInTransaction)
                        Undo();
                }
            }
        }
        public void Redo()
        {
            int index = CurOperationIndex + 1;
            if (index >= m_Oprations.Count)
            {
                IsInTransaction = false;
                return;
            }
            CurOperation = m_Oprations[index];
            
            if (CurOperation.LinkNext && !IsInTransaction)
                IsInTransaction = true;
            
            CurOperation.Do();
            if (AfterRedoOperation != null)
                AfterRedoOperation(CurOperation);

            if (CurOperation.LinkNext && index < m_Oprations.Count - 1)
                Redo();
            else
                IsInTransaction = false;
        }

        public delegate void Delegate_OnOperationChanged(Operation opt);
        public event Delegate_OnOperationChanged CurOperationChanged;
        public event Delegate_OnOperationChanged NewOperationCommited;
        public event Delegate_OnOperationChanged NewOperationPushed;
        public event Delegate_OnOperationChanged AfterUndoOperation;
        public event Delegate_OnOperationChanged AfterRedoOperation;
        private static OperationHistory m_Instance = new OperationHistory();
        private Operation m_NewOperation = null;
        private Operation m_CurOperation = null;
        private Operation m_SuspendedOperation = null;
        private List<Operation> m_Oprations = new List<Operation>();
    }
}
