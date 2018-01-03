using System;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using FireTerminator.Common;
using FireTerminator.Common.Services;
using FireTerminator.Server.Services;

namespace FireTerminator.Server
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                DevExpress.UserSkins.BonusSkins.Register();
                DevExpress.Skins.SkinManager.EnableFormSkins();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (!SecurityGrandDog.Instance.StartupCheck())
                    return;
                ProjectDoc.Instance.Option = Option;
                ProjectDoc.Instance.LoadProjectsDescriptions();
                UserMgr.Load();
                using (GraphicPainter gp = new GraphicPainter())
                {
                    Graphic = gp;
                    gp.IsFixedTimeStep = false;
                    gp.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 30);
                    gp.Run();
                }
                UserMgr.Save();

                ServerHostLogin.StopService();
                ServerHostMain.StopService();
                ServerHostTrans.StopService();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
            finally
            {
                IsAppRunning = false;
                SecurityGrandDog.Instance.Close();
            }
        }

        public static OptionsServer Option = new OptionsServer();
        public static UserManager UserMgr = new UserManager();
        private static ServerHost ServerHostLogin = null;
        private static ServerHost ServerHostMain = null;
        private static ServerHost ServerHostTrans = null;
        public static UserResSynchronizer ResSynchronizer = new UserResSynchronizer();
        public static bool IsAppRunning = true;
        public static GraphicPainter Graphic = null;

        public static void OnStartUpLoginService(ServerHost host)
        {
            // 启动登录连接服务线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                while (IsAppRunning)
                {
                    var binding = new NetTcpBinding(SecurityMode.None);
                    binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                    binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                    binding.OpenTimeout = TimeSpan.MaxValue;
                    binding.SendTimeout = TimeSpan.MaxValue;
                    binding.ReceiveTimeout = TimeSpan.MaxValue;
                    binding.MaxReceivedMessageSize = 1024 * 1024;
                    binding.MaxBufferPoolSize = 1024 * 1024;
                    if (!host.StartService(typeof(LoginService), typeof(ILoginService), binding, (int)PortID.LoginLink, "LoginService"))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    ServerHostLogin = host;
                    break;
                }
            }));
        }
        public static void OnStartUpMainService(ServerHost host)
        {
            // 启动主服务线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                while (IsAppRunning)
                {
                    var binding = new NetTcpBinding(SecurityMode.None);
                    binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                    binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                    binding.OpenTimeout = TimeSpan.MaxValue;
                    binding.SendTimeout = TimeSpan.MaxValue;
                    binding.ReceiveTimeout = TimeSpan.MaxValue;
                    binding.MaxReceivedMessageSize = 1024 * 1024 * 1024;
                    binding.MaxBufferPoolSize = 1024 * 1024 * 1024;
                    if (!host.StartService(typeof(MainService), typeof(IMainService), binding, (int)PortID.MainService, "MainService"))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    ServerHostMain = host;
                    break;
                }
            }));
            // 启动登录断连检测线程
            UserMgr.StartUserKickingDetectingThread();
        }
        public static void OnStartUpTransmissionRequestService(ServerHost host)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                while (IsAppRunning)
                {
                    if (!host.StartService(typeof(TransmissionRequestService), typeof(ITransmissionRequestService),
                        new NetTcpBinding(SecurityMode.None), (int)PortID.DownloadTransmissionDetection, "TransmissionRequestService"))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    ServerHostTrans = host;
                    break;
                }
            }));
        }
    }
}
