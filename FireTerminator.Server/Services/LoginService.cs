using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using FireTerminator.Common;
using FireTerminator.Common.Services;
using FireTerminator.Common.Structures;
using System.ServiceModel.Channels;

namespace FireTerminator.Server.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class LoginService : ILoginService
    {
        public void Login(LoginInfo info)
        {
            ILoginServiceCallback curGuest = OperationContext.Current.GetCallbackChannel<ILoginServiceCallback>();
            if (!info.IsAccountValid)
            {
                curGuest.NotifyLoginResult(null, LoginResult.失败_帐号非法);
                return;
            }
            RegisteredUserInfo uinfo = null;
            LoginResult result = LoginResult.登录成功;
            if (info.IsAccountIDValid)
            {
                uinfo = Program.UserMgr.GetRegisteredUserInfo(info.AccountID);
                if (uinfo == null)
                {
                    curGuest.NotifyLoginResult(null, LoginResult.失败_帐号不存在);
                    return;
                }
            }
            else if (info.IsAccountNameValid)
            {
                uinfo = Program.UserMgr.GetRegisteredUserInfo(info.AccountName);
                if (uinfo == null)
                {
                    uinfo = Program.UserMgr.RegisterUser(info, OperationContext.Current);
                    result = LoginResult.登录并注册成功;
                }
            }
            if (info.Password != uinfo.Password)
            {
                curGuest.NotifyLoginResult(null, LoginResult.失败_密码错误);
                return;
            }
            if (info.AdjudicatorMode && (uinfo.Permission & UserPermission.评审) == 0)
            {
                curGuest.NotifyLoginResult(null, LoginResult.失败_评审权限未开通);
                return;
            }
            var suser = Program.UserMgr.GetLoginUserInfo(uinfo.AccountID);
            if (suser != null)
            {
                if (suser.IsOnline)
                    Program.UserMgr.KickUser(suser.AccountID, 0, LogoutReason.异地登录, null);
                else
                {
                    suser.LoginInfo.AdjudicatorMode = info.AdjudicatorMode;
                    suser.LoginInfo.TransServicePort = info.TransServicePort;
                    suser.LoginInfo.JudgementConfigFileMD5 = info.JudgementConfigFileMD5;
                }
            }
            suser = Program.UserMgr.GetLoginUserInfo(OperationContext.Current);
            if (suser != null)
            {
                if (suser.AccountID != uinfo.AccountID)
                    Program.UserMgr.KickUser(suser.AccountID, 5, LogoutReason.更改用户, null);
                else
                    curGuest.NotifyLoginResult(uinfo, result);
            }
            suser = Program.UserMgr.GetLoginUserInfo(uinfo.AccountID);
            if (suser.LoginInfo.AdjudicatorMode)
            {
                if (!Program.UserMgr.SetUserTaskGroup(suser.AccountID, 0))
                {
                    curGuest.NotifyLoginResult(null, LoginResult.失败_未知错误);
                    return;
                }
            }
            Program.UserMgr.OnUserLoginSucceeded(uinfo.AccountID, OperationContext.Current);
            curGuest.NotifyLoginResult(uinfo, result);
        }
    }
}
