using System;
using System.Windows.Forms;
using FireTerminator.Common;

namespace FireTerminator.Editor
{
#if WINDOWS || XBOX
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

                using (Graphic = new GraphicPainter())
                {
                    Graphic.IsFixedTimeStep = false;
                    Graphic.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 30);
                    Graphic.Run();
                }
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
        public static bool IsAppRunning = true;
        public static OptionsEditor Option = new OptionsEditor();
        public static GraphicPainter Graphic;
    }
#endif
}

