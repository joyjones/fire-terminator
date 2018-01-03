using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;

namespace FireTerminator.Common.Structures
{
    [Flags]
    public enum UserPermission
    {
        空 = 0,
        任务选择 = 1,
        评审 = 1 << 1,
        默认 = 任务选择 | 评审
    }

    [DataContract]
    public class RegisteredUserInfo
    {
        public RegisteredUserInfo()
        {
        }
        public RegisteredUserInfo(XmlElement node)
        {
            LoadXml(node);
        }
        public RegisteredUserInfo(long account, string name, string password)
        {
            AccountID = account;
            Name = name;
            Password = password;
        }
        [DataMember]
        public long AccountID = 0;
        [DataMember]
        public string Name = "";
        [DataMember]
        public string Password = "";
        [DataMember]
        public bool SexMale = true;
        [DataMember]
        public int LinkPort = 0;
        [DataMember]
        public int TaskGroupID = -1;
        [DataMember]
        public UserPermission Permission = UserPermission.默认;

        public override string ToString()
        {
            return Name;
        }
        public void LoadXml(XmlElement node)
        {
            AccountID = long.Parse(node.GetAttribute("ID"));
            Name = node.GetAttribute("Name");
            Password = node.GetAttribute("Password");
            TaskGroupID = int.Parse(node.GetAttribute("TaskGroupID"));
            SexMale = bool.Parse(node.GetAttribute("SexMale"));
        }
        public void SaveXml(XmlElement node)
        {
            node.SetAttribute("ID", AccountID.ToString());
            node.SetAttribute("Name", Name);
            node.SetAttribute("Password", Password);
            node.SetAttribute("TaskGroupID", TaskGroupID.ToString());
            node.SetAttribute("SexMale", SexMale.ToString());
        }
    }
}
