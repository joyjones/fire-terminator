using System;
using System.Windows.Forms;
using FireTerminator.Common.Structures;
using FireTerminator.Common;
using FireTerminator.Common.Services;
using System.ServiceModel;
using System.Configuration;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FireTerminator.Client
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
                ProjectDoc.Instance.Option = Option;

                CurConfigFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                StartUpTransService();

                StartupArgs.Clear();
                if (args != null)
                {
                    foreach (var arg in args)
                    {
                        var mat = Regex.Match(arg, @"(?'K'.+)=(?'V'.*)");
                        if (mat.Success)
                            StartupArgs[mat.Groups["K"].Value] = mat.Groups["V"].Value;
                    }
                }
                using (Graphic = new GraphicPainter())
                {
                    Graphic.IsFixedTimeStep = false;
                    Graphic.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 30);
                    Graphic.Run();
                }

                TransServerHost.StopService();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        public static void StartUpTransService()
        {
            var host = new ServerHost("文件传输");
            // 启动登录连接服务线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                while (true)
                {
                    var binding = new NetTcpBinding(SecurityMode.None)
                    {
                        Name = "TransferFileBinding",
                        MaxReceivedMessageSize = 1024 * 1024 * 1024 * 1024L,
                        TransferMode = TransferMode.Streamed,
                    };
                    if (!host.StartService(typeof(DownloadTransmissionService), typeof(IFileTransmissionService),
                        binding, DownloadTransmissionService.ServicePort, "DownloadTransmissionService"))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    TransServerHost = host;
                    break;
                }
            }));
        }
        public static void SaveConfigValue(string section, string value)
        {
            if (StartupArgs.Count == 0)
            {
                var node = CurConfigFile.AppSettings.Settings[section];
                if (node != null && node.Value != value)
                {
                    node.Value = value;
                    CurConfigFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }
        public static string GetStartupArgValue(string key)
        {
            string val;
            if (StartupArgs.TryGetValue(key, out val))
                return val;
            return "";
        }
        public static bool IsStartupBoolArgValueTrue(string key)
        {
            string val = GetStartupArgValue(key);
            if (val == "")
                return false;
            return val.ToLower() == "1" || val.ToLower() == "true";
        }
        public static UserTaskGroup GetTaskGroup(int groupId)
        {
            UserTaskGroup grp;
            TaskGroups.TryGetValue(groupId, out grp);
            return grp;
        }
        public static LoginUserInfo GetLoginUserInfo(long accId)
        {
            foreach (var grp in TaskGroups.Values)
            {
                var user = grp.GetUser(accId);
                if (user != null)
                    return user;
            }
            return null;
        }
        public static ProjectReferrencer TargetJudgementPR
        {
            get
            {
                if (CurUser == null)
                    return null;
                if (CurUser.IsMoniteringCooperationGroup)
                    return GetTaskGroup((int)CurUser.TargetMonitorUserOrGroupID * -1);
                else if (CurUser.IsMoniteringNoCooperationUser)
                    return GetLoginUserInfo(CurUser.TargetMonitorUserOrGroupID);
                return null;
            }
        }
        public static bool IsLoginJudgeMode
        {
            get { return IsStartupBoolArgValueTrue("JudgeMode"); }
        }

        public static OptionsClient Option = new OptionsClient();
        public static GraphicPainter Graphic;
        public static Configuration CurConfigFile = null;
        public static ServerHost TransServerHost = null;
        public static LoginUserInfo CurUser = new LoginUserInfo();
        public static ServiceClientProxy Service = new ServiceClientProxy();
        public static Dictionary<string, string> StartupArgs = new Dictionary<string, string>();
        public static Dictionary<int, UserTaskGroup> TaskGroups = new Dictionary<int, UserTaskGroup>();
        public static JudgementSettings JudgementSet = new JudgementSettings();
    }
}
