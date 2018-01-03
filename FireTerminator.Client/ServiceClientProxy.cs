using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FireTerminator.Common;
using FireTerminator.Common.Services;
using System.ServiceModel;
using System.Windows.Forms;
using FireTerminator.Common.Structures;

namespace FireTerminator.Client
{
    public class ServiceClientProxy
    {
        public ServiceClientProxy()
        {
        }

        public ILoginService LoginChannel
        {
            get;
            protected set;
        }

        public IMainService MainChannel
        {
            get;
            protected set;
        }

        public ITransmissionRequestService TransmissionChannel
        {
            get;
            protected set;
        }

        public string ServerIP
        {
            get;
            protected set;
        }
        public void Login(string serverIP, string account, string password, bool adjudicatorMode, Action failAction)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                try
                {
                    string ip = obj as string;
                    if (LoginChannel == null)
                    {
                        string ipe = String.Format("{0}:{1}", ip, (int)PortID.LoginLink);
                        //string ipe = String.Format("localhost:{0}", (int)PortID.LoginLink);

                        var binding = new NetTcpBinding(SecurityMode.None);
                        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                        binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                        binding.OpenTimeout = TimeSpan.MaxValue;
                        binding.SendTimeout = TimeSpan.MaxValue;
                        binding.ReceiveTimeout = TimeSpan.MaxValue;
                        binding.MaxReceivedMessageSize = 1024 * 1024;
                        binding.MaxBufferPoolSize = 1024 * 1024;

                        var inst = new InstanceContext(MainForm.Instance.FrmLogin);
                        var endpt = new EndpointAddress(string.Format("{0}://{1}/LoginService", binding.Scheme, ipe));
                        var fac = new DuplexChannelFactory<ILoginService>(inst, binding);
                        LoginChannel = fac.CreateChannel(endpt);

                        ((IContextChannel)LoginChannel).OperationTimeout = new TimeSpan(0, 0, 10);
                        ((IContextChannel)LoginChannel).Closed += new EventHandler(ProxyService_Closed);
                        ((IContextChannel)LoginChannel).Faulted += new EventHandler(ProxyService_Faulted);
                    }

                    var info = new LoginInfo(account, password)
                    {
                        AdjudicatorMode = adjudicatorMode,
                        TransServicePort = DownloadTransmissionService.ServicePort,
                        JudgementConfigFileMD5 = adjudicatorMode ? JudgementSettings.ConfigFileMD5 : ""
                    };
                    LoginChannel.Login(info);
                    ServerIP = ip;
                }
                catch// (Exception ex)
                {
                    LoginChannel = null;
                    if (failAction != null)
                    {
                        failAction();
                    }
                }
            }), serverIP);
        }

        private void ProxyService_Closed(object sender, EventArgs e)
        {
        }
        private void ProxyService_Faulted(object sender, EventArgs e)
        {
            var login = MainForm.Instance.FrmLogin;
            if (login.Visible)
            {
                login.InvokeEx(() =>
                {
                    login.lblUpdateStatus.Text = "与服务器的连接已断开，请稍后再试...";
                    login.UIEnabled = true;
                });
            }
        }

        public void UserEnter()
        {
            try
            {
                if (MainChannel == null)
                {
                    string ipe = String.Format("{0}:{1}", ServerIP, (int)PortID.MainService);
                    //string ipe = String.Format("localhost:{0}", (int)PortID.MainService);
                    InstanceContext inst = new InstanceContext(MainForm.Instance);
                    var binding = new NetTcpBinding(SecurityMode.None);
                    binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                    binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                    binding.MaxReceivedMessageSize = 1024 * 1024 * 1024;
                    binding.MaxBufferPoolSize = 1024 * 1024 * 1024;
                    MainChannel = DuplexChannelFactory<IMainService>.CreateChannel(inst, binding, new EndpointAddress(string.Format("{0}://{1}/MainService", binding.Scheme, ipe)));
                    ((IContextChannel)MainChannel).OperationTimeout = new TimeSpan(0, 0, 10);
                    ((IContextChannel)MainChannel).Closed += new EventHandler(ProxyMainService_Closed);
                    ((IContextChannel)MainChannel).Faulted += new EventHandler(ProxyMainService_Faulted);
                }

                if (TransmissionChannel == null)
                {
                    var binding = new NetTcpBinding(SecurityMode.None);
                    string addr = String.Format("{0}://{1}:{2}/TransmissionRequestService", binding.Scheme, ServerIP, (int)PortID.DownloadTransmissionDetection);
                    TransmissionChannel = ChannelFactory<ITransmissionRequestService>.CreateChannel(binding, new EndpointAddress(addr));
                }
                // 启动心跳线程
                ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
                {
                    while (MainChannel != null && LoginChannel != null)
                    {
                        try
                        {
                            if (!MainChannel.BeatingHeart(Program.CurUser.Info.LinkPort))
                            {
                                throw new System.ServiceModel.ServiceActivationException();
                            }
                        }
                        catch
                        {
                            MainForm.Instance.InvokeEx(() => MainForm.Instance.ShowLoginForm());
                        }
#if DEBUG
                        break;
#else
                        Thread.Sleep(2000);
#endif
                    }
                }));
            }
            catch (Exception ex)
            {
                //LogSink.Write(MessageLevel.错误, MessageType.登录连接, "创建或连接服务失败：" + ex.Message);
            }
        }

        void ProxyMainService_Faulted(object sender, EventArgs e)
        {
        }

        private void ProxyMainService_Closed(object sender, EventArgs e)
        {
        }

        public void Logout(bool requestServer)
        {
            Program.CurUser.Info = null;
            ProjectDoc.Instance.CurEditUserID = 0;
            try
            {
                if (requestServer && MainChannel != null)
                    MainChannel.Logout();
            }
            catch { }
            finally
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            AutoResetEvent evt = new AutoResetEvent(false);
            if (MainChannel != null)
            {
                CommonMethods.CloseChannel(MainChannel, 10, evt);
                evt.WaitOne();
            }
            MainChannel = null;
            if (LoginChannel != null)
            {
                CommonMethods.CloseChannel(LoginChannel, 10, evt);
                evt.WaitOne();
            }
            LoginChannel = null;
            if (TransmissionChannel != null)
            {
                CommonMethods.CloseChannel(TransmissionChannel, 10, evt);
                evt.WaitOne();
            }
            TransmissionChannel = null;
        }

        public void QueryTransmissionFile(TransFileKind kind, string file)
        {
            if (!String.IsNullOrEmpty(file))
                QueryTransmissionFile(kind, new string[] { file });
        }

        public void QueryTransmissionFile(TransFileKind kind, string[] files)
        {
            if (TransmissionChannel != null && files != null && files.Length > 0)
                TransmissionChannel.QueryTransmission(DownloadTransmissionService.ServicePort, kind, files);
        }
    }
}
