using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.Structures;
using FireTerminator.Server.Services;
using FireTerminator.Common.Services;
using FireTerminator.Common;

namespace FireTerminator.Server
{
    public class UserResSynchronizer
    {
        public UserResSynchronizer()
        {
            LoadingUserSlots = new ServerLoginUserInfo[MaxThreadCount];
            TransRequestService.TransmissionFinished += new TransmissionRequestService.Delegate_Transmission(OnTransmissionFinished);
            TransRequestService.TransmissionFailed += new TransmissionRequestService.Delegate_Transmission(OnTransmissionFailed);
        }

        public bool PushRequest(ProjectReferrencer pr)
        {
            bool succeeded = false;
            lock (WaitingUsers)
            {
                foreach (var id in pr.UserIDs)
                {
                    var suser = Program.UserMgr.GetLoginUserInfo(id);
                    if (suser.LoginInfo.MissingResourceFiles.Length > 0)
                    {
                        WaitingUsers.Add(suser);
                        succeeded = true;
                    }
                }
                pr.IsResSynchronizing = true;
            }
            return succeeded;
        }

        public void CancelRequest(ProjectReferrencer pr)
        {
            lock (WaitingUsers)
            {
                pr.IsResSynchronizing = false;
                foreach (var id in pr.UserIDs)
                {
                    var suser = Program.UserMgr.GetLoginUserInfo(id);
                    WaitingUsers.Remove(suser);
                }
            }
        }

        public void Tick()
        {
            for (int i = 0; WaitingUsers.Count > 0 && i < LoadingUserSlots.Length; ++i)
            {
                if (LoadingUserSlots[i] == null)
                {
                    ServerLoginUserInfo user = null;
                    lock (WaitingUsers)
                    {
                        user = WaitingUsers[0];
                        WaitingUsers.RemoveAt(0);
                    }
                    if (user.Status < LoginStatus.在线)
                        --i;
                    else
                    {
                        TransRequestService.QueryTransmission(user.LinkEndPoint.Address, user.LoginInfo.TransServicePort, TransFileKind.ResourceFile, user.LoginInfo.MissingResourceFiles);
                        LoadingUserSlots[i] = user;
                    }
                }
            }
        }

        private void OnTransmissionFinished(int targetTransPort)
        {
            for (int i = 0; i < LoadingUserSlots.Length; ++i)
            {
                var user = LoadingUserSlots[i];
                if (user != null && user.LoginInfo.TransServicePort == targetTransPort)
                    LoadingUserSlots[i] = null;
            }
        }

        private void OnTransmissionFailed(int targetTransPort)
        {
            OnTransmissionFinished(targetTransPort);
        }

        public int MaxThreadCount
        {
            get { return 1; }
        }
        private TransmissionRequestService TransRequestService = new TransmissionRequestService();
        private List<ServerLoginUserInfo> WaitingUsers = new List<ServerLoginUserInfo>();
        private ServerLoginUserInfo[] LoadingUserSlots = null;
    }
}
