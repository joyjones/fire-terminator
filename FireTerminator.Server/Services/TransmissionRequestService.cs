using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common;
using FireTerminator.Common.Services;
using System.ServiceModel;
using System.IO;
using System.Threading;
using System.ServiceModel.Channels;

namespace FireTerminator.Server.Services
{
    public class TransmissionRequestService : ITransmissionRequestService
    {
        #region ITransmissionRequestService 成员
        public void QueryTransmission(int transPort, TransFileKind kind, string[] files)
        {
            var imp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            QueryTransmission(imp.Address, transPort, kind, files);
        }
        #endregion
        public void QueryTransmission(string ip, int transPort, TransFileKind kind, string[] files)
        {
            m_Files.Clear();
            foreach (var file in files)
            {
                string resFile;
                if (kind == TransFileKind.ResourceFile)
                    resFile = Options.UserResourceRootPath + file;
                else if (kind == TransFileKind.ProjectFile)
                    resFile = Options.DefaultProjectsRootPath + file;
                else
                    continue;
                if (File.Exists(resFile))
                {
                    var fi = new FileInfo(resFile);
                    var md5 = CommonMethods.GetFileMD5(resFile);
                    m_Files[resFile] = new TransferFileInfo(Guid.NewGuid(), kind, file, fi.Length, md5);
                }
            }
            StartTransmission(ip, transPort);
        }

        public void StartTransmission(string ip, int port)
        {
            string ipe = String.Format("{0}:{1}", ip, port);
            if (m_Files.Count == 0 || ProxyTransfers.ContainsKey(ipe))
                return;

            var binding = new NetTcpBinding(SecurityMode.None)
            {
                TransferMode = TransferMode.Streamed,
                SendTimeout = new TimeSpan(0, 30, 0)
            };
            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            //binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            //binding.OpenTimeout = TimeSpan.MaxValue;
            //binding.SendTimeout = TimeSpan.MaxValue;
            //binding.ReceiveTimeout = TimeSpan.MaxValue;
            //binding.MaxReceivedMessageSize = 1024 * 1024;
            //binding.MaxBufferPoolSize = 1024 * 1024;
            //binding.TransferMode = TransferMode.Streamed;
            // 创建客户端代理
            string addr = String.Format("net.tcp://{0}/DownloadTransmissionService", ipe);
            var transfer = ChannelFactory<IFileTransmissionService>.CreateChannel(binding, new EndpointAddress(addr));
            ProxyTransfers[ipe] = transfer;

            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                try
                {
                    if (TransmissionBegin != null)
                        TransmissionBegin(port);
                    var trans = obj as IFileTransmissionService;
                    if (trans.BeginTransferFiles(m_Files.Values.ToArray()))
                        TransferFiles(ipe, trans);
                    ProxyTransfers.Remove(ipe);
                    CommonMethods.CloseChannel(trans, 10, null);
                    if (TransmissionFinished != null)
                        TransmissionFinished(port);
                }
                catch (System.Exception ex)
                {
                    if (TransmissionFailed != null)
                        TransmissionFailed(port);
                }
            }), transfer);
        }

        private void TransferFiles(string ipe, IFileTransmissionService transfer)
        {
            //LogSink.Write(MessageType.文件传输, String.Format("开始向客户端{0}传输文件...", ipe));
            bool succeeded = true;
            int index = 0;
            foreach (var kv in m_Files)
            {
                try
                {
                    var msg = new FileTransferMessage();
                    msg.Index = index++;
                    msg.GUID = kv.Value.GUID;
                    msg.Content = new FileStream(kv.Key, FileMode.Open, FileAccess.Read, FileShare.Read);
                    transfer.TransferFile(msg);
                    msg.Content.Close();
                }
                catch// (Exception ex)
                {
                    //LogSink.Write(MessageLevel.错误, MessageType.文件传输, String.Format("向客户端{0}传输文件{1}失败！({2})", ipe, kv.Key, ex.Message));
                    succeeded = false;
                }
            }
            //if (succeeded)
            //    LogSink.Write(MessageType.文件传输, String.Format("向客户端{0}传输文件成功！", ipe));
        }


        private static Dictionary<string, IFileTransmissionService> ProxyTransfers = new Dictionary<string, IFileTransmissionService>();
        private Dictionary<string, TransferFileInfo> m_Files = new Dictionary<string, TransferFileInfo>();
        public delegate void Delegate_Transmission(int targetTransPort);
        public event Delegate_Transmission TransmissionBegin;
        public event Delegate_Transmission TransmissionFinished;
        public event Delegate_Transmission TransmissionFailed;
    }
}
