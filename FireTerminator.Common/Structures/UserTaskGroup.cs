using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FireTerminator.Common.Structures
{
    public enum TaskGroupColumn
    {
        名称 = 0,
        项目及完整度,
        任务名称,
        场景名称,
        窗口索引
    }

    [DataContract]
    public class UserTaskGroup : ProjectReferrencer
    {
        public UserTaskGroup(int id)
        {
            GroupID = id;
        }
        public LoginUserInfo GetUser(long accId)
        {
            LoginUserInfo user = null;
            Users.TryGetValue(accId, out user);
            return user;
        }
        public bool AddUser(LoginUserInfo user)
        {
            if (GetUser(user.AccountID) != null)
                return false;
            if (GroupID > 0 && Users.Count >= ProjectDoc.Instance.Option.MaxGroupUserCount)
                return false;
            user.Info.TaskGroupID = this.GroupID;
            user.ParentGroup = this;
            Users[user.AccountID] = user;
            return true;
        }
        public bool RemoveUser(long accId)
        {
            return Users.Remove(accId);
        }

        [DataMember]
        public int GroupID = -1;
        [DataMember]
        public Dictionary<long, LoginUserInfo> Users = new Dictionary<long, LoginUserInfo>();

        public override bool IsStarted
        {
            get { return base.IsStarted; }
            set
            {
                if (IsCooperation)
                    base.IsStarted = value;
                else
                {
                    foreach (var user in Users.Values)
                    {
                        user.IsStarted = value;
                    }
                }
            }
        }
        private bool m_IsCooperation = false;
        public bool IsCooperation
        {
            get { return m_IsCooperation; }
            set
            {
                if (m_IsCooperation != value)
                {
                    base.ProjectName = null;
                    foreach (var user in Users.Values)
                    {
                        user.ViewportIndex = -1;
                        user.ProjectName = null;
                    }
                    base.ViewportIndex = -1;
                    m_IsCooperation = value;
                    foreach (TaskGroupColumn col in Enum.GetValues(typeof(TaskGroupColumn)))
                    {
                        UpdateNode(col);
                    }
                }
            }
        }
        public override long[] UserIDs
        {
            get
            {
                return (from user in Users.Values
                        select user.AccountID).ToArray();
            }
        }
        public LoginUserInfo[] OnlineUsers
        {
            get
            {
                return (from user in Users.Values
                        where user.Status >= LoginStatus.登录中
                        select user).ToArray();
            }
        }
        public LoginUserInfo[] ActiveUsers
        {
            get
            {
                return (from user in Users.Values
                        where user.Status == LoginStatus.在线
                        select user).ToArray();
            }
        }
        public override ProjectInfo ProjectInstance
        {
            get
            {
                if (IsCooperation)
                    return base.ProjectInstance;
                return null;
            }
            internal set
            {
                if (IsCooperation)
                    base.ProjectInstance = value;
            }
        }
        public override string ProjectName
        {
            get
            {
                if (!IsCooperation)
                    return "";
                return base.ProjectName;
            }
            set
            {
                if (IsCooperation)
                    base.ProjectName = value;
                else
                {
                    ProjectInstance = null;
                    foreach (var user in Users.Values)
                    {
                        user.ProjectName = value;
                    }
                }
            }
        }
        public override string TaskName
        {
            get
            {
                if (!IsCooperation)
                    return "";
                return base.TaskName;
            }
            set
            {
                if (IsCooperation)
                    base.TaskName = value;
                else
                {
                    foreach (var user in Users.Values)
                    {
                        user.TaskName = value;
                    }
                }
            }
        }
        public override string SceneName
        {
            get
            {
                if (!IsCooperation)
                    return "";
                return base.SceneName;
            }
            set
            {
                if (IsCooperation)
                    base.SceneName = value;
                else
                {
                    foreach (var user in Users.Values)
                    {
                        user.SceneName = value;
                    }
                }
            }
        }
        public override int ViewportIndex
        {
            get
            {
                if (!IsCooperation)
                    return -1;
                return base.ViewportIndex;
            }
            set
            {
                if (IsCooperation)
                    base.ViewportIndex = value;
                else
                {
                    foreach (var user in Users.Values)
                    {
                        user.ViewportIndex = value;
                    }
                }
            }
        }
        public override void ResetDefaultTaskScene()
        {
            if (IsCooperation)
                base.ResetDefaultTaskScene();
            else
            {
                foreach (var user in Users.Values)
                {
                    user.ResetDefaultTaskScene();
                }
            }
        }
        public override string ToString()
        {
            if (IsCooperation)
                return Name + " - 合作";
            return Name;
        }
        public override string GetTaskGroupColumnText(TaskGroupColumn col)
        {
            if (col == TaskGroupColumn.名称)
                return this.ToString();
            else if (IsCooperation)
                return base.GetTaskGroupColumnText(col);
            return "";
        }
        public override void UpdateNode(TaskGroupColumn col)
        {
            base.UpdateNode(col);
        }
    }
}
