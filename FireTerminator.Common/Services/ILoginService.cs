using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using FireTerminator.Common.Structures;
using System.Text.RegularExpressions;

namespace FireTerminator.Common.Services
{
    [ServiceContract(Namespace = "http://Fireterminator.Common.Services", CallbackContract = typeof(ILoginServiceCallback))]
    public interface ILoginService
    {
        [OperationContract(IsOneWay = true)]
        void Login(LoginInfo info);
    }

    public interface ILoginServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyLoginResult(RegisteredUserInfo user, LoginResult result);
        [OperationContract(IsOneWay = true)]
        void SendMessage(MessageLevel lv, MessageType type, string msg);
    }

    [DataContract]
    public class LoginInfo
    {
        public LoginInfo()
        {
        }
        public LoginInfo(long acct, string pass)
        {
            AccountName = "";
            AccountID = acct;
            Password = pass;
        }
        public LoginInfo(string acct, string pass)
        {
            if (IsNumberAccount(acct))
            {
                AccountName = "";
                AccountID = long.Parse(acct);
            }
            else
            {
                AccountName = acct.Replace(" ", "");
                AccountID = 0;
            }
            Password = pass;
        }
        [DataMember]
        public long AccountID = 0;
        [DataMember]
        public string AccountName = "";
        [DataMember]
        public string Password = "";
        [DataMember]
        public int TransServicePort = 0;
        [DataMember]
        public LoginStatus Status = LoginStatus.离线;
        [DataMember]
        public bool AdjudicatorMode = false;
        [DataMember]
        public string JudgementConfigFileMD5 = "";

        public bool IsAccountValid
        {
            get { return IsAccountIDValid || IsAccountNameValid; }
        }
        public bool IsAccountIDValid
        {
            get { return AccountID >= ProjectDoc.Instance.Option.UserAccountBeginID; }
        }
        public bool IsAccountNameValid
        {
            get { return !String.IsNullOrEmpty(AccountName); }
        }
        public static bool IsNumberAccount(string account)
        {
            if (String.IsNullOrEmpty(account))
                return false;
            return Regex.IsMatch(account, @"^\d+$");
        }
    }
}
