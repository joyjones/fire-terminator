using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using FireTerminator.Common.RenderResources;

namespace FireTerminator.Common
{
    [Flags]
    public enum ProgramType
    {
        客户端 = 1,
        服务端 = 1 << 1,
        编辑器 = 1 << 2,
        Any = 客户端 | 服务端 | 编辑器
    }
    public class ProgramUsageAttribute : Attribute
    {
        public ProgramUsageAttribute(ProgramType type)
        {
            ProgramT = type;
        }
        public ProgramType ProgramT
        {
            get;
            private set;
        }
    }

    public class Options
    {
        public Options()
        {
            ViewportFrameColor = Color.White;
            ViewportFrameSelectedColor = Color.GreenYellow;
            ViewportFrameSelectedAnimModeColor = Color.Red;
            ElementSelectedFrameColor = Color.Yellow;
            MaxOperationHistoryStepsCount = 200;
            AnimFrameCountPerSecond = 25;
            AnimFrameRowsUpToDown = false;
            ScaleElementInProportionAtCorner = true;
            UseElementPixelSelection = true;
            UseSafeViewportSwitching = false;
            ElementScaleFrameThickness = 6;
            MaxViewportScale = 10;
            MinViewportScale = 0.001F;
            TransitionViewBackColor = Color.FromArgb(255, 51, 67, 77);
            TransitionViewRowSeperateLineColor = Color.FromArgb(255, 23, 39, 49);
            TransitionViewBackColorFocus = Color.FromArgb(255, 67, 83, 93);
            TransitionRangeColor = Color.FromArgb(255, 158, 209, 165);
            TransitionRangeColorSelected = Color.FromArgb(255, 224, 128, 128);

            DefaultElementCaptionScale = 0.5F;
            AutoScaleElementCaption = true;
            OperationSyncTimeInterval = 50;

            MessageTextColor = Color.Black;
            MessageTextColor_SystemPrompt = Color.Green;
            MessageTextColor_SystemWarning = Color.Orange;
            MessageTextColor_SystemError = Color.Red;

            UserAccountBeginID = 1000;
            MaxGroupUserCount = 100;
            WaterbagJointColorUV = new PointF(0.9896F, 0.6F);
        }
        #region 公共属性
        [Browsable(false)]
        public static string AppRootPath
        {
            get { return System.Windows.Forms.Application.StartupPath.TrimEnd('\\') + "\\"; }
        }
        [Category("路径"), DisplayName("用户资源根目录")]
        public static string UserResourceRootPath
        {
            get { return AppRootPath + ResourceRootPathName + "\\" + UserResourcePathName + "\\"; }
        }
        [Category("路径"), DisplayName("系统资源根目录")]
        public static string SystemResourceRootPath
        {
            get { return AppRootPath + ResourceRootPathName + "\\" + SystemResourcePathName + "\\"; }
        }
        [Category("路径"), DisplayName("默认工程配置文件目录")]
        public static string DefaultProjectsRootPath
        {
            get { return AppRootPath + ProjectRootPathName + "\\"; }
        }
        [Category("路径"), Browsable(false), DisplayName("视频录制软件路径")]
        public static string FrapsAppFile
        {
            get { return SystemResourceRootPath + "\\fraps\\FrapsEnPortable.exe"; }
        }
        [Category("路径"), Browsable(false), DisplayName("视频录制源文件存放目录")]
        public static string FrapsAppMoviePath
        {
            get { return SystemResourceRootPath + "\\fraps\\Movies\\"; }
        }
        [Category("路径"), Browsable(false), DisplayName("视频录制文件目标存放目录")]
        public static string RecordMovieTargetPath
        {
            get { return AppRootPath + "Records\\"; }
        }
        [Category("操作"), DisplayName("最大撤销记录数")]
        public int MaxOperationHistoryStepsCount
        {
            get;
            set;
        }
        [Category("操作"), DisplayName("操作同步时间间隔")]
        public int OperationSyncTimeInterval
        {
            get;
            set;
        }
        [Category("操作"), DisplayName("边角处按比例缩放元素")]
        public bool ScaleElementInProportionAtCorner
        {
            get;
            set;
        }
        [Category("操作"), DisplayName("元素缩放边框宽度")]
        public int ElementScaleFrameThickness
        {
            get;
            set;
        }
        [Category("操作"), DisplayName("使用像素点选"), Description("在点选元素透明部分时进行像素级穿透，进而可选中被遮盖在后面的元素。")]
        public bool UseElementPixelSelection
        {
            get;
            set;
        }
        [Category("操作"), DisplayName("仅鼠标左键切换窗口"), Description("是否仅在进行鼠标左键单击操作时，才切换当前操作窗口，以严格约束当前窗口的选定。")]
        public bool UseSafeViewportSwitching
        {
            get;
            set;
        }
        public List<string> ServerIPs = new List<string>();
        #endregion
        #region 编辑视图属性
        [Category("颜色-视图"), DisplayName("窗口分割线颜色")]
        public Color ViewportFrameColor
        {
            get;
            set;
        }
        [Category("颜色-视图"), DisplayName("当前窗口边框色")]
        public Color ViewportFrameSelectedColor
        {
            get;
            set;
        }
        [Category("颜色-视图"), DisplayName("当前动画模式窗口边框色")]
        public Color ViewportFrameSelectedAnimModeColor
        {
            get;
            set;
        }
        [Category("颜色-视图"), DisplayName("当前元素边框色")]
        public Color ElementSelectedFrameColor
        {
            get;
            set;
        }
        [Category("颜色-视图"), DisplayName("水带关节颜色采样点")]
        public PointF WaterbagJointColorUV
        {
            get;
            set;
        }
        [Category("颜色-动画面板"), DisplayName("行背景色")]
        public Color TransitionViewBackColor
        {
            get;
            set;
        }
        [Category("颜色-动画面板"), DisplayName("行分割线颜色")]
        public Color TransitionViewRowSeperateLineColor
        {
            get;
            set;
        }
        [Category("颜色-动画面板"), DisplayName("当前选择行背景色")]
        public Color TransitionViewBackColorFocus
        {
            get;
            set;
        }
        [Category("颜色-动画面板"), DisplayName("变换区间颜色")]
        public Color TransitionRangeColor
        {
            get;
            set;
        }
        [Category("颜色-动画面板"), DisplayName("当前选择变换区间颜色")]
        public Color TransitionRangeColorSelected
        {
            get;
            set;
        }
        [Category("颜色-消息"), DisplayName("当前消息文本色")]
        public Color MessageTextColor
        {
            get;
            set;
        }
        [Category("颜色-消息"), DisplayName("系统提示消息颜色"), ReadOnly(true)]
        public virtual Color MessageTextColor_SystemPrompt
        {
            get;
            set;
        }
        [Category("颜色-消息"), DisplayName("系统警告消息颜色"), ReadOnly(true)]
        public virtual Color MessageTextColor_SystemWarning
        {
            get;
            set;
        }
        [Category("颜色-消息"), DisplayName("系统错误消息颜色"), ReadOnly(true)]
        public virtual Color MessageTextColor_SystemError
        {
            get;
            set;
        }
        [Category("动画"), DisplayName("每秒播放帧数")]
        public int AnimFrameCountPerSecond
        {
            get;
            set;
        }
        [Category("动画"), DisplayName("帧动画行从上至下")]
        public bool AnimFrameRowsUpToDown
        {
            get;
            set;
        }
        [Category("视图"), DisplayName("视图最大放大倍数")]
        public float MaxViewportScale
        {
            get;
            set;
        }
        [Category("视图"), DisplayName("视图最大缩小倍数")]
        public float MinViewportScale
        {
            get;
            set;
        }
        [Category("视图"), DisplayName("元素标题默认缩放比")]
        public float DefaultElementCaptionScale
        {
            get;
            set;
        }
        [Category("视图"), DisplayName("元素标题大小自动改变")]
        public bool AutoScaleElementCaption
        {
            get;
            set;
        }
        [Category("视图"), DisplayName("皮肤样式")]
        public string ProgramSkin
        {
            get { return DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveSkinName; }
            set { DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(value); }
        }
        #endregion
        #region 服务端属性
        [Browsable(false)]
        public virtual long UserAccountBeginID
        {
            get;
            set;
        }
        [Browsable(false)]
        public virtual int MaxGroupUserCount
        {
            get;
            set;
        }
        #endregion
        public Color GetMessageColor(MessageLevel lvl)
        {
            switch (lvl)
            {
                case MessageLevel.一般:
                    return MessageTextColor;
                case MessageLevel.提示:
                    return MessageTextColor_SystemPrompt;
                case MessageLevel.警告:
                    return MessageTextColor_SystemWarning;
                case MessageLevel.错误:
                    return MessageTextColor_SystemError;
            }
            return Color.Black;
        }
        public virtual bool Load()
        {
            if (!File.Exists(AppRootPath + ConfigFileName))
                return false;
            XmlDocument doc = new XmlDocument();
            doc.Load(AppRootPath + ConfigFileName);
            foreach (var pi in this.GetType().GetProperties())
            {
                if (!pi.CanWrite)
                    continue;
                var cats = pi.GetCustomAttributes(typeof(CategoryAttribute), false);
                var disps = pi.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                if (cats.Length > 0 && disps.Length > 0)
                {
                    var cat = cats[0] as CategoryAttribute;
                    var disp = disps[0] as DisplayNameAttribute;
                    var nodes = doc.DocumentElement.GetElementsByTagName(cat.Category);
                    if (nodes.Count > 0)
                    {
                        var node = nodes[0] as XmlElement;
                        var snodes = node.GetElementsByTagName(disp.DisplayName);
                        if (snodes.Count > 0)
                        {
                            var snode = snodes[0];
                            if (pi.PropertyType == typeof(Color))
                            {
                                var mat = Regex.Match(snode.InnerText, @"(?'R'\d+),(?'G'\d+),(?'B'\d+),(?'A'\d+)");
                                if (mat.Success)
                                {
                                    var clr = Color.FromArgb(
                                        int.Parse(mat.Groups["A"].Value),
                                        int.Parse(mat.Groups["R"].Value),
                                        int.Parse(mat.Groups["G"].Value),
                                        int.Parse(mat.Groups["B"].Value));
                                    pi.SetValue(this, clr, null);
                                }
                            }
                            else if (pi.PropertyType == typeof(string))
                                pi.SetValue(this, snode.InnerText, null);
                            else if (pi.PropertyType == typeof(float))
                                pi.SetValue(this, float.Parse(snode.InnerText), null);
                            else if (pi.PropertyType == typeof(int))
                                pi.SetValue(this, int.Parse(snode.InnerText), null);
                        }
                    }
                }
            }
            return true;
        }
        public virtual void Save()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<FireTerminatorConfigs></FireTerminatorConfigs>");
            foreach (var pi in this.GetType().GetProperties())
            {
                if (!pi.CanWrite)
                    continue;
                XmlElement node = null;
                var cats = pi.GetCustomAttributes(typeof(CategoryAttribute), false);
                var disps = pi.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                if (cats.Length > 0 && disps.Length > 0)
                {
                    var cat = cats[0] as CategoryAttribute;
                    var disp = disps[0] as DisplayNameAttribute;
                    var nodes = doc.DocumentElement.GetElementsByTagName(cat.Category);
                    if (nodes.Count > 0)
                        node = nodes[0] as XmlElement;
                    else
                    {
                        node = doc.CreateElement(cat.Category);
                        doc.DocumentElement.AppendChild(node);
                    }
                    var snode = doc.CreateElement(disp.DisplayName);
                    object value = pi.GetValue(this, null);
                    if (value != null)
                    {
                        string txtVal = "";
                        if (value is Color)
                        {
                            var clr = (Color)value;
                            txtVal = String.Format("{0},{1},{2},{3}", clr.R, clr.G, clr.B, clr.A);
                        }
                        else
                            txtVal = value.ToString();
                        var tnode = doc.CreateTextNode(txtVal);
                        snode.AppendChild(tnode);
                        node.AppendChild(snode);
                    }
                }
            }
            doc.Save(AppRootPath + ConfigFileName);
        }
        [Browsable(false)]
        public virtual string ConfigFileName
        {
            get { return null; }
        }
        [Browsable(false)]
        public virtual string LayoutConfigFileName
        {
            get { return ""; }
        }
        [Browsable(false)]
        public string LayoutConfigFile
        {
            get { return AppRootPath + LayoutConfigFileName; }
        }
        public static readonly string ResourceRootPathName = "Content";
        public static readonly string ProjectRootPathName = "Projects";
        public static readonly string UserResourcePathName = "data";
        public static readonly string SystemResourcePathName = "depend";
    }
}
