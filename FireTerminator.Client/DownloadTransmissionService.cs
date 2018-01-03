using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FireTerminator.Common.Services;
using FireTerminator.Common;

namespace FireTerminator.Client
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DownloadTransmissionService : BaseFileTransmissionService
    {
        public static int ServicePort
        {
            get
            {
                int port = System.Diagnostics.Process.GetCurrentProcess().Id;
                if (port >= 8000 && port <= 8100)
                    port += 100;
                return port;
            }
        }
        protected override void OnMessage(MessageLevel lvl, string message)
        {
            MainForm.Instance.MessageController.PushMessage(lvl, MessageType.文件传输, message);
        }
        protected override void OnFileTransferFinished(FileTransferMessage request, RemoteTransferRequest req, bool succeeded, string targetFile)
        {
            base.OnFileTransferFinished(request, req, succeeded, targetFile);
            var res = req.GetResourceInfo(request.GUID);
            Program.Service.MainChannel.ReportDownloadedOneResourceFile(res.FileName);
        }
        protected override void OnTransmissionFinished(FileTransferMessage request, RemoteTransferRequest req)
        {
            base.OnTransmissionFinished(request, req);
            MainForm.Instance.OnTrainingProjectFileReady();
        }
    }
}
