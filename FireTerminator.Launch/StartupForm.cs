using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using FireTerminator.Launch.Properties;

namespace FireTerminator.Launch
{
    public partial class StartupForm : Form
    {
        public StartupForm()
        {
            InitializeComponent();
        }

        public string SearchPath
        {
            get { return Application.StartupPath.TrimEnd('\\') + "\\"; }
        }
        public static readonly string AppFile_Client = "FireTerminator.Client.exe";
        public static readonly string AppFile_JudgeClient = "FireTerminator.Judge.exe";
        public static readonly string AppFile_Server = "FireTerminator.Server.exe";
        public static readonly string AppFile_Editor = "FireTerminator.Editor.exe";
        private bool TryStartupAppFile(string appFile)
        {
            return TryStartupAppFile(appFile, "");
        }
        private bool TryStartupAppFile(string appFile, string args)
        {
            var files = Directory.GetFiles(SearchPath, appFile, SearchOption.AllDirectories);
            if (files != null && files.Length > 0)
            {
                ProcessStartInfo pi = new ProcessStartInfo(files[0], args);
                pi.WorkingDirectory = Path.GetDirectoryName(files[0]);
                Process.Start(pi);
                return true;
            }
            MessageBox.Show("未能找到" + appFile + "！");
            return false;
        }

        private void bnStartupClient_Click(object sender, EventArgs e)
        {
            if (TryStartupAppFile(AppFile_Client))
                this.Close();
        }

        private void bnStartupJudgeClient_Click(object sender, EventArgs e)
        {
            if (TryStartupAppFile(AppFile_JudgeClient))
                this.Close();
        }

        private void bnStartupServer_Click(object sender, EventArgs e)
        {
            if (TryStartupAppFile(AppFile_Server))
                this.Close();
        }

        private void bnStartupEditor_Click(object sender, EventArgs e)
        {
            if (TryStartupAppFile(AppFile_Editor))
                this.Close();
        }

        private void StartupForm_Load(object sender, EventArgs e)
        {
        }

        private void bnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int m_CurMaxClientId = 10000;
        private void bnStartTestClients_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 50; ++i)
            {
                string args = String.Format("ip=127.0.0.1 user=tester{0} pass=", ++m_CurMaxClientId);
                TryStartupAppFile(AppFile_Client, args);
            }
        }
    }
}
