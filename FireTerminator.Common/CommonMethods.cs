using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using System.Text.RegularExpressions;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;

namespace FireTerminator.Common
{
    public static class CommonMethods
    {
        #region 扩展方法
        public static object InvokeEx(this Control obj, Action action)
        {
            return obj.Invoke(action);
        }
        public static IAsyncResult BeginInvokeEx(this Control obj, Action action)
        {
            return obj.BeginInvoke(action);
        }
        #endregion
        #region 网络操作
        public static void CloseChannel(object chnl, int timeOutSeconds, AutoResetEvent evtFinish)
        {
            var channel = chnl as IContextChannel;
            if (channel != null)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
                {
                    try
                    {
                        channel.Close(TimeSpan.FromSeconds(timeOutSeconds));
                    }
                    catch { }
                    finally
                    {
                        if (evtFinish != null)
                            evtFinish.Set();
                    }
                }));
            }
        }
        #endregion

        #region 文件操作
        public static void InheritCreateFolder(string path, bool isFile)
        {
            char ch = '\\';
            int n = 0;
            if (!path.StartsWith("http:"))
            {
                path = path.Replace('/', '\\');
                if (path.StartsWith("\\\\"))
                {
                    n = path.IndexOf("\\", 2);
                    if (n < 0)// 共享根目录无需检测
                        return;
                    ++n;
                }
            }
            else
            {
                ch = '/';
                n = 7;
            }
            if (isFile)
                path.TrimEnd(ch);
            else if (!path.EndsWith(ch.ToString()))
                path += ch.ToString();
            while (n < path.Length)
            {
                int np = path.IndexOf(ch, n);
                if (np < 0)
                    break;
                if (np > 0)
                {
                    string subdir = path.Substring(0, np + 1);
                    if (!Directory.Exists(subdir))
                        Directory.CreateDirectory(subdir);
                }
                n = np + 1;
            }
        }
        public static void InheritDeleteFolder(string destpath, bool deleteme)
        {
            if (!Directory.Exists(destpath))
                return;
            var fs = Directory.GetFiles(destpath, "*", SearchOption.TopDirectoryOnly);
            var ds = Directory.GetDirectories(destpath, "*", SearchOption.TopDirectoryOnly);
            foreach (var f in fs)
            {
                if (!File.Exists(f))
                    continue;
                File.SetAttributes(f, FileAttributes.Normal);
                File.Delete(f);
            }
            foreach (var d in ds)
                InheritDeleteFolder(d, true);
            if (deleteme)
            {
                do
                {
                    try
                    {
                        if (Directory.Exists(destpath))
                            Directory.Delete(destpath, true);
                        break;
                    }
                    catch (System.Exception) { }
                } while (true);
            }
        }
        public static readonly string[] ImageFileExts = new string[] { ".bmp", ".jpg", ".jpeg", ".png", ".tga", ".gif" };
        public static readonly string[] CompressFileExts = new string[] { ".rar", ".zip", ".7z" };
        public static string GetFileMD5(string filename)
        {
            if (!File.Exists(filename))
                return "";
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = File.ReadAllBytes(filename);
            byte[] r = md5.ComputeHash(bytes);
            return BitConverter.ToString(r).Replace("-", "");
        }
        public static bool CompareFiles(string srcFile, string targetFile)
        {
            if (!File.Exists(targetFile))
                return false;
            FileInfo srcInfo = new FileInfo(srcFile);
            FileInfo targetInfo = new FileInfo(targetFile);
            // 长度检验
            if (srcInfo.Length != targetInfo.Length)
                return false;
            // 时间戳检验
            if (srcInfo.LastWriteTime != targetInfo.LastWriteTime)
            {
                // MD5码校验
                string mdOld = GetFileMD5(targetFile);
                string mdNew = GetFileMD5(srcFile);
                return mdOld == mdNew;
            }
            return true;
        }
        #endregion
        #region 其他操作
        public static void GainInnerFittableRegion(ref Size sizeIn, Size sizeOut, out Point offset)
        {
            SizeF _sizeIn = sizeIn;
            SizeF _sizeOut = sizeOut;
            PointF _offset;
            GainInnerFittableRegion(ref _sizeIn, _sizeOut, out _offset);
            sizeIn.Width = (int)_sizeIn.Width;
            sizeIn.Height = (int)_sizeIn.Height;
            offset = new Point((int)_offset.X, (int)_offset.Y);
        }
        public static void GainInnerFittableRegion(ref SizeF sizeIn, SizeF sizeOut, out PointF offset)
        {
            float w1 = sizeIn.Width;
            float h1 = sizeIn.Height;
            float w2 = sizeOut.Width;
            float h2 = sizeOut.Height;
            float offX, offY;
            GainInnerFittableRegion(ref w1, ref h1, w2, h2, out offX, out offY);
            sizeIn.Width = w1;
            sizeIn.Height = h1;
            offset = new PointF(offX, offY);
        }
        public static void GainInnerFittableRegion(ref float w1, ref float h1, float w2, float h2, out float offsetX, out float offsetY)
        {
            float r1 = w1 / h1;
            float r2 = w2 / h2;
            float w, h;
            if (r1 == r2)
            {
                w = w2;
                h = h2;
                offsetX = offsetY = 0;
            }
            else if (r1 < r2)
            {
                w = h2 / h1 * w1;
                h = h2;
                offsetX = (w2 - w) / 2;
                offsetY = 0;
            }
            else// if (r1 > r2)
            {
                w = w2;
                h = w2 / w1 * h1;
                offsetX = 0;
                offsetY = (h2 - h) / 2;
            }
            w1 = w; h1 = h;
        }
        public static void GainOutterFittableRegion(ref Size sizeIn, Size sizeOut, out Point offset)
        {
            SizeF _sizeIn = sizeIn;
            SizeF _sizeOut = sizeOut;
            PointF _offset;
            GainOutterFittableRegion(ref _sizeIn, _sizeOut, out _offset);
            sizeIn.Width = (int)_sizeIn.Width;
            sizeIn.Height = (int)_sizeIn.Height;
            offset = new Point((int)_offset.X, (int)_offset.Y);
        }
        public static void GainOutterFittableRegion(ref SizeF sizeIn, SizeF sizeOut, out PointF offset)
        {
            float w1 = sizeIn.Width;
            float h1 = sizeIn.Height;
            float w2 = sizeOut.Width;
            float h2 = sizeOut.Height;
            float offX, offY;
            GainOutterFittableRegion(ref w1, ref h1, w2, h2, out offX, out offY);
            sizeIn.Width = w1;
            sizeIn.Height = h1;
            offset = new PointF(offX, offY);
        }
        public static void GainOutterFittableRegion(ref float w1, ref float h1, float w2, float h2, out float offsetX, out float offsetY)
        {
            float r1 = w1 / h1;
            float r2 = w2 / h2;
            float w, h;
            if (r1 == r2)
            {
                w = w2;
                h = h2;
                offsetX = offsetY = 0;
            }
            else if (r1 < r2)
            {
                w = w2;
                h = w2 / w1 * h1;
                offsetX = 0;
                offsetY = (h2 - h) / 2;
            }
            else// if (r1 > r2)
            {
                w = h2 / h1 * w1;
                h = h2;
                offsetX = (w2 - w) / 2;
                offsetY = 0;
            }
            w1 = w; h1 = h;
        }
        public static void ClampValue(ref float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
        }
        public static float ClampValue(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        public static bool GetFormatFloat2(string text, ref PointF pos)
        {
            float x = 0, y = 0;
            if (GetFormatFloat2(text, ref x, ref y))
            {
                pos.X = x;
                pos.Y = y;
                return true;
            }
            return false;
        }
        public static bool GetFormatFloat2(string text, ref SizeF size)
        {
            float w = 0, h = 0;
            if (GetFormatFloat2(text, ref w, ref h))
            {
                size.Width = w;
                size.Height = h;
                return true;
            }
            return false;
        }
        public static bool GetFormatFloat2(string text, ref float v1, ref float v2)
        {
            var mat = Regex.Match(text, @"(?'X'-?\d+(\.\d+)?)\s*,\s*(?'Y'-?\d+(\.\d+)?)");
            if (mat.Success)
            {
                v1 = Convert.ToSingle(mat.Groups["X"].Value);
                v2 = Convert.ToSingle(mat.Groups["Y"].Value);
                return true;
            }
            return false;
        }

        public static bool GetFormatInt2(string text, ref Point pos)
        {
            int x = 0, y = 0;
            if (GetFormatInt2(text, ref x, ref y))
            {
                pos.X = x;
                pos.Y = y;
                return true;
            }
            return false;
        }
        public static bool GetFormatInt2(string text, ref Size size)
        {
            int w = 0, h = 0;
            if (GetFormatInt2(text, ref w, ref h))
            {
                size.Width = w;
                size.Height = h;
                return true;
            }
            return false;
        }
        public static bool GetFormatInt2(string text, ref int v1, ref int v2)
        {
            var mat = Regex.Match(text, @"(?'X'-?\d+(\.\d+)?)\s*,\s*(?'Y'-?\d+(\.\d+)?)");
            if (mat.Success)
            {
                v1 = Convert.ToInt32(mat.Groups["X"].Value);
                v2 = Convert.ToInt32(mat.Groups["Y"].Value);
                return true;
            }
            return false;
        }
        public static Microsoft.Xna.Framework.Graphics.Color ConvertColor(System.Drawing.Color clr)
        {
            return new Microsoft.Xna.Framework.Graphics.Color(clr.R, clr.G, clr.B, clr.A);
        }
        public static System.Drawing.Color ConvertColor(Microsoft.Xna.Framework.Graphics.Color clr)
        {
            return System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
        }
        #endregion
    }
}
