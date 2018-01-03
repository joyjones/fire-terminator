using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMEncoderLib;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace FireTerminator.Common
{
//可以调用WMEncoder SDK：Interop.WMEncoderLib.dll，但是在录制屏幕的过程中，需要客户端安装Windows Media Encoder 9软件。
//调用起来也比较方便，其实WMEncoder SDK本身已经包含了帮助手册，内面有C++,C#,VB的调用方法。
    public class VideoRecorder
    {
        public VideoRecorder()
        {
            try
            {
                m_Encoder = new WMEncoder();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("视频解码器启动失败，视频录制功能将不能正常使用！\r\n" + ex.Message, "视频解码器启动失败",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }
        public void Finish()
        {
            m_Encoder.Stop();
        }
        public void BeginRecord(string profile)
        {
            // Create WMEncoder object. 
            //Get group collection 
            IWMEncSourceGroupCollection SrcGrpColl = m_Encoder.SourceGroupCollection;
            //Add group into collection 
            IWMEncSourceGroup SrcGrp = SrcGrpColl.Add("SG_1");
            //Add audio source and video source into group 
            //IWMEncSource SrcAud = SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_AUDIO);
            //SrcAud.SetInput("Default_Audio_Device", "Device", "");
            IWMEncVideoSource2 SrcVid = (IWMEncVideoSource2)SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_VIDEO);
            SrcVid.SetInput("ScreenCapture1", "ScreenCap", "");
            //Load profile config. 
            if (String.IsNullOrEmpty(profile))
            {
                if (SelectUserProFile != null)
                {
                    List<string> lstPFs = new List<string>();
                    foreach (IWMEncProfile pro in m_Encoder.ProfileCollection)
                        lstPFs.Add(pro.Name);
                    profile = SelectUserProFile(lstPFs.ToArray());
                }
            }
            if (!String.IsNullOrEmpty(profile))
            {
                foreach (IWMEncProfile pro in m_Encoder.ProfileCollection)
                {
                    if (pro.Name == profile)
                    {
                        SrcGrp.set_Profile(pro);
                        break;
                    }
                }
            }
            //Pro.LoadFromFile(prxFileName);
            //SrcGrp.set_Profile(Pro);
            //Add the display information of output video file. 
            IWMEncDisplayInfo Descr = m_Encoder.DisplayInfo;
            Descr.Title = "Screen Recorder Video";
            Descr.Author = Environment.UserName;
            Descr.Description = "";
            Descr.Rating = "";
            Descr.Copyright = "";
            //IWMEncAttributes Attr = Encoder.Attributes; 
            //Attr.Add("URL", "IP address"); 
            //Prepare to encode 
            //videoEncoder.PrepareToEncode(true);
            //Record start 
            //Specify output file path 
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
            m_Encoder.File.LocalFileName = String.Format("{0}RecordFile{1}.wmv", SavePath, CurNewFileTailIndex);
            m_Encoder.Start();
        }
        public int CurNewFileTailIndex
        {
            get
            {
                var files = Directory.GetFiles(SavePath, "RecordFile*.wmv", SearchOption.TopDirectoryOnly).ToList();
                for (int i = 0; i < files.Count; ++i)
                    files[i] = Path.GetFileName(files[i]);
                for (int i = 1; ; ++i)
                {
                    string fmt = "RecordFile" + i + ".wmv";
                    if (!files.Contains(fmt))
                        return i;
                }
            }
        }

        public string SavePath
        {
            get
            {
                return System.Windows.Forms.Application.StartupPath + "\\Records\\";
            }
        }

        public string[] EncProfiles
        {
            get
            {
                List<string> lstPFs = new List<string>();
                try
                {
                    foreach (IWMEncProfile pro in m_Encoder.ProfileCollection)
                        lstPFs.Add(pro.Name);
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("catched in m_Encoder.ProfileCollection\r\n" + ex.Message);
                }
                return lstPFs.ToArray();
            }
        }
        private WMEncoder m_Encoder = null;
        public delegate string Delegate_SelectUserProFile(string[] profiles);
        public event Delegate_SelectUserProFile SelectUserProFile;
        private static VideoRecorder m_Instance = null;
        private static object SyncObj = new object();
        public static VideoRecorder Instance
        {
            get
            {
                lock (SyncObj)
                {
                    if (m_Instance == null)
                        m_Instance = new VideoRecorder();
                }
                return m_Instance;
            }
        }
    }

    public sealed class API 
    { 
        public static int WM_KEYDOWN = 0x0100; 
        public static int WM_KEYUP = 0x0101; 
        public static int WM_SYSKEYDOWN = 0x0104; 
        public static int WM_SYSKEYUP = 0x0105; 

        public static int WM_MOUSEMOVE = 0x0200; 
        public static int WM_LBUTTONDOWN = 0x0201; 
        public static int WM_LBUTTONUP = 0x0202; 
        public static int WM_LBUTTONDBLCLK = 0x0203; 
        public static int WM_RBUTTONDOWN = 0x0204; 
        public static int WM_RBUTTONUP = 0x0205; 
        public static int WM_RBUTTONDBLCLK = 0x0206; 
        public static int WM_USER = 0x0400; 

        public static int MK_LBUTTON = 0x0001; 
        public static int MK_RBUTTON = 0x0002; 
        public static int MK_SHIFT = 0x0004; 
        public static int MK_CONTROL = 0x0008; 
        public static int MK_MBUTTON = 0x0010; 

        public static int MK_XBUTTON1 = 0x0020; 
        public static int MK_XBUTTON2 = 0x0040; 

        [DllImport("user32.dll")] 
        public static extern int SendMessage(int hWnd,int Msg,int wParam,int lParam); 
    } 
}
