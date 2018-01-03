using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DevExpress.XtraTreeList.Nodes;

namespace FireTerminator.Common.Structures
{
    [DataContract]
    public class LoginUserInfo : ProjectReferrencer
    {
        public LoginUserInfo()
        {
        }
        public LoginUserInfo(RegisteredUserInfo info, int transPort, LoginStatus status)
        {
            Info = info;
            TransServicePort = transPort;
            Status = status;
        }
        public long AccountID
        {
            get { return Info == null ? 0 : Info.AccountID; }
        }
        public static readonly long SystemUserID = Int32.MaxValue;
        private RegisteredUserInfo m_Info = null;
        [DataMember]
        public RegisteredUserInfo Info
        {
            get { return m_Info; }
            set
            {
                if (value != null)
                {
                    m_Info = value;
                    Name = Info.Name;
                }
            }
        }
        [DataMember]
        public int TransServicePort = 0;
        [DataMember]
        public LoginStatus Status = LoginStatus.离线;
        [DataMember]
        public long TargetMonitorUserOrGroupID = 0;
        [DataMember]
        public string JudgementConfigFileMD5 = "";

        private UserTaskGroup m_ParentGroup = null;
        public UserTaskGroup ParentGroup
        {
            get { return m_ParentGroup; }
            set
            {
                if (m_ParentGroup != value && Info != null)
                {
                    m_ParentGroup = value;
                    Info.TaskGroupID = m_ParentGroup.GroupID;
                }
            }
        }
        public object LocalObj
        {
            get;
            set;
        }
        public override long[] UserIDs
        {
            get { return new long[] { AccountID }; }
        }
        public override bool IsStarted
        {
            get
            {
                if (ParentGroup != null && ParentGroup.IsCooperation)
                    return ParentGroup.IsStarted;
                return base.IsStarted;
            }
            set
            {
                if (ParentGroup != null && ParentGroup.IsCooperation)
                    ParentGroup.IsStarted = value;
                else
                    base.IsStarted = value;
            }
        }
        public override ProjectInfo ProjectInstance
        {
            get
            {
                if (ParentGroup != null && ParentGroup.IsCooperation)
                    return ParentGroup.ProjectInstance;
                return base.ProjectInstance;
            }
            internal set
            {
                if (ParentGroup == null || !ParentGroup.IsCooperation)
                    base.ProjectInstance = value;
                else
                    ParentGroup.ProjectInstance = value;
            }
        }
        public override int ViewportIndex
        {
            get
            {
                if (ParentGroup != null && ParentGroup.IsCooperation)
                    return ParentGroup.ViewportIndex;
                return base.ViewportIndex;
            }
            set
            {
                base.ViewportIndex = value;
            }
        }
        public bool AdjudicatorMode
        {
            get { return TargetMonitorUserOrGroupID != 0; }
            set
            {
                if (value && TargetMonitorUserOrGroupID == 0)
                    TargetMonitorUserOrGroupID = long.MaxValue;
                else if (!value)
                    TargetMonitorUserOrGroupID = 0;
            }
        }
        public bool IsMoniteringCooperationGroup
        {
            get { return TargetMonitorUserOrGroupID != long.MaxValue && TargetMonitorUserOrGroupID < 0; }
        }
        public bool IsMoniteringNoCooperationUser
        {
            get { return TargetMonitorUserOrGroupID != long.MaxValue && TargetMonitorUserOrGroupID > 0; }
        }
        public override string GetTaskGroupColumnText(TaskGroupColumn col)
        {
            if (col == TaskGroupColumn.项目及完整度)
            {
                if (ProjectInstance == null)
                    return "";
                string projName = ProjectInstance.Name;
                if (ParentGroup.IsCooperation)
                    projName = "";
                if (ProjectDescription == null)
                    return projName;
                if (m_MissingResourceFiles.Count == 0)
                    return projName + "(完整)";
                return projName + String.Format("({0}/{1})", ProjectDescription.ResourceFiles.Count - m_MissingResourceFiles.Count, ProjectDescription.ResourceFiles.Count);
            }
            return base.GetTaskGroupColumnText(col);
        }
    }
}
