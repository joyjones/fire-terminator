using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;

namespace FireTerminator.Common.Services
{
    public class RemoteTransferRequest
    {
        public RemoteTransferRequest(OperationContext context, TransferFileInfo[] infos)
        {
            EndPtMsgProperty = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            foreach (var info in infos)
            {
                info.TransferTime = DateTime.Now;
                Resources[info.GUID] = info;
            }
        }
        public RemoteEndpointMessageProperty EndPtMsgProperty;
        public Dictionary<Guid, TransferFileInfo> Resources = new Dictionary<Guid, TransferFileInfo>();
        public long TotalFilesLength
        {
            get { return Resources.Values.Select(res => res.FileLength).Sum(); }
        }
        public string Address
        {
            get { return EndPtMsgProperty.Address; }
        }
        public TransferFileInfo GetResourceInfo(Guid guid)
        {
            TransferFileInfo info = null;
            Resources.TryGetValue(guid, out info);
            return info;
        }
        public long CurTransferedSize
        {
            get;
            set;
        }
        public FileTransState CurTransState
        {
            get;
            set;
        }
    }

    public class BaseFileTransmissionService : IFileTransmissionService
    {
        public static Dictionary<string, RemoteTransferRequest> TransferRequests = new Dictionary<string, RemoteTransferRequest>();
        public static readonly int TransmissionBufferLen = 4096;

        public delegate void Delegate_OnFileDownloaded(FileTransferMessage request, string targetFile);
        public static event Delegate_OnFileDownloaded OnFileDownloaded;

        #region IFileTransmissionService 成员
        public virtual bool BeginTransferFiles(TransferFileInfo[] infos)
        {
            var desc = new RemoteTransferRequest(OperationContext.Current, infos);
            if (TransferRequests.ContainsKey(desc.Address))
                return false;
            TransferRequests[desc.Address] = desc;
            return true;
        }

        public virtual void TransferFile(FileTransferMessage request)
        {
            var emp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            RemoteTransferRequest req = null;
            if (!TransferRequests.TryGetValue(emp.Address, out req))
                return;
            var res = req.GetResourceInfo(request.GUID);
            if (res == null)
                return;
            if (request.Index == 0)
                OnTransmissionBegin(request, req);
            if (!OnFileTransferBegin(request, req))
                return;
            req.CurTransState = new FileTransState(res.FileName, res.FileLength, request.Index);
            OnMessage(String.Format("开始接收文件:{1}({0})...", res.Kind, res.FileName));

            string filePath = GetFileDirectory(res.Kind).TrimEnd('\\') + "\\" + res.FileName;
            CommonMethods.InheritCreateFolder(filePath, true);

            bool succeeded = true;
            try
            {
                req.CurTransState.curSize = 0;
                // 文件流传输
                using (var targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //定义文件缓冲区
                    byte[] buffer = new byte[TransmissionBufferLen];
                    int count = 0;
                    while ((count = request.Content.Read(buffer, 0, TransmissionBufferLen)) > 0)
                    {
                        targetStream.Write(buffer, 0, count);
                        OnFileTransferring(request, req, count);
                    }
                    request.Content.Close();
                }
            }
            catch (Exception ex)
            {
                OnMessage(MessageLevel.错误, String.Format("接收文件{0}失败：{1}", res.FileName, ex.Message));
                succeeded = false;
            }

            OnFileTransferFinished(request, req, succeeded, filePath);
            if (request.Index >= req.Resources.Count - 1)
            {
                OnTransmissionFinished(request, req);
            }
        }
        #endregion

        protected virtual void OnTransmissionBegin(FileTransferMessage request, RemoteTransferRequest req)
        {
            req.CurTransferedSize = 0;
        }

        protected virtual void OnTransmissionFinished(FileTransferMessage request, RemoteTransferRequest req)
        {
            req.CurTransState = null;
            req.CurTransferedSize = 0;
            TransferRequests.Remove(req.Address);
        }

        protected virtual bool OnFileTransferBegin(FileTransferMessage request, RemoteTransferRequest req)
        {
            //判断文件是否可读
            if (request.Content == null || !request.Content.CanRead)
            {
                var res = req.GetResourceInfo(request.GUID);
                OnMessage(String.Format("接收文件{0}失败：数据流不可读", res.FileName));
                return false;
            }
            return true;
        }

        protected virtual void OnFileTransferring(FileTransferMessage request, RemoteTransferRequest req, int bytescount)
        {
            req.CurTransState.curSize += bytescount;
            req.CurTransferedSize += bytescount;
        }

        protected virtual void OnFileTransferFinished(FileTransferMessage request, RemoteTransferRequest req, bool succeeded, string targetFile)
        {
            req.CurTransState.curSize = req.CurTransState.maxSize;
            req.CurTransferedSize = req.TotalFilesLength;
            if (succeeded)
            {
                if (OnFileDownloaded != null)
                    OnFileDownloaded(request, targetFile);
                var res = req.GetResourceInfo(request.GUID);
                OnMessage(String.Format("接收文件完成：{0}({1})", res.FileName, targetFile));
            }
        }

        protected virtual string GetFileDirectory(TransFileKind kind)
        {
            if (kind == TransFileKind.ProjectFile)
                return Options.DefaultProjectsRootPath;
            else if (kind == TransFileKind.ResourceFile)
                return Options.UserResourceRootPath;
            return System.Windows.Forms.Application.StartupPath.TrimEnd('\\') + "\\";
        }
        protected void OnMessage(string message)
        {
            OnMessage(MessageLevel.提示, message);
        }
        protected virtual void OnMessage(MessageLevel lvl, string message)
        {
        }
    }
}
