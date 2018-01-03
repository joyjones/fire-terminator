using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FireTerminator.Common;
using System.Diagnostics;
using System.IO;

namespace FireTerminator.Editor
{
    public partial class PlayControlForm : Form
    {
        public PlayControlForm()
        {
            InitializeComponent();
        }

        public bool IsRecording
        {
            get { return chkRecord.Checked; }
        }

        private void bnQuit_Click(object sender, EventArgs e)
        {
            Program.Graphic.IsFullScreen = false;
        }

        private void chkRecord_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bnQuit.Enabled = !chkRecord.Checked;
                if (chkRecord.Checked)
                {
                    if (!chkPlay.Checked)
                        chkPlay.Checked = true;
                    chkRecord.BackColor = Color.Red;
                    if (GraphicPainter.StartSinkWriter() != 0)
                    {
                        MessageBox.Show("Failed to StartSinkWriter!");
                    }
                }
                else
                {
                    chkRecord.BackColor = Color.WhiteSmoke;
                    GraphicPainter.FinishSinkWriter();
                    string sourceFile = Options.AppRootPath + "output.wmv";
                    if (!File.Exists(sourceFile))
                    {
                        var di = new DirectoryInfo(Options.AppRootPath);
                        sourceFile = di.Parent.FullName.TrimEnd('\\') + "\\output.wmv";
                    }
                    if (File.Exists(sourceFile))
                    {
                        string targetName = String.Format("FireTerminator {0}.wmv", DateTime.Now.ToString()).Replace(':', '-').Replace('\\', '-').Replace('/', '-');
                        string targetFile = Options.RecordMovieTargetPath + targetName;
                        try
                        {
                            CommonMethods.InheritCreateFolder(targetFile, true);
                            File.Move(sourceFile, targetFile);
                        }
                        catch { }
                    }
                }

                //System.Windows.Forms.SendKeys.Send("^%p");
                //System.Windows.Forms.SendKeys.Flush();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkPlay_CheckedChanged(object sender, EventArgs e)
        {
            ((MainForm)Program.Graphic.MainForm).iPlayAnimation.Checked = chkPlay.Checked;
            if (chkPlay.Checked)
                chkPlay.BackColor = Color.GreenYellow;
            else
            {
                chkPlay.BackColor = Color.WhiteSmoke;
                if (chkRecord.Checked)
                    chkRecord.Checked = false;
            }
        }

        private void PlayControlForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
#if false
                chkRecord.Enabled = false;
                if (File.Exists(Options.FrapsAppFile))
                {
                    Process.Start(Options.FrapsAppFile);
                    tmTick.Enabled = true;
                }
#else
                chkRecord.Enabled = Program.Graphic.IsSinkWriterInitialSucceeded;
#endif
            }
            else
            {
#if false
                foreach (var prs in Process.GetProcesses())
                {
                    if (prs.ProcessName.ToLower().StartsWith("fraps"))
                        prs.Kill();
                }
                if (Directory.Exists(Options.FrapsAppMoviePath))
                {
                    string targetPath = Options.FrapsMovieTargetPath;
                    foreach (var file in Directory.GetFiles(Options.FrapsAppMoviePath).ToArray())
                    {
                        string targetFile = targetPath + file.Substring(Options.FrapsAppMoviePath.Length);
                        try
                        {
                            CommonMethods.InheritCreateFolder(targetFile, true);
                            File.Move(file, targetFile);
                        }
                        catch { }
                    }
                }
#else
#endif
                chkPlay.Checked = false;
            }
        }

        private void PlayControlForm_Shown(object sender, EventArgs e)
        {
            var size = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            int x = (size.Width - Width) / 2;
            int y = size.Height / 8 * 7 - Height / 2;
            Location = new Point(x, y);
        }

        private void tmTick_Tick(object sender, EventArgs e)
        {
            int count = 0;
            foreach (var prs in Process.GetProcesses())
            {
                string name = prs.ProcessName.ToLower();
                if (name == "frapsenportable")
                    count |= 1;
                else if (name == "fraps")
                    count |= 2;
            }
            if (count >= 3)
            {
                //tmTick.Enabled = false;
                chkRecord.Enabled = true;
            }
        }
    }
}
