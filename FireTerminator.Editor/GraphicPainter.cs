using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FireTerminator.Common;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.Elements;
using FireTerminator.Common.UI;
using System.Runtime.InteropServices;

namespace FireTerminator.Editor
{
    public class GraphicPainter : GraphicsRenderer
    {
        public GraphicPainter()
        {
            IsSinkWriterInitialSucceeded = false;
        }
        [DllImport("FireTerminator.SinkWriter.dll")]
        public static extern void InitializeSinkWriter(uint width, uint height, uint fps, uint bps);
        [DllImport("FireTerminator.SinkWriter.dll")]
        public static extern int WriteFrame(uint[] buff);
        [DllImport("FireTerminator.SinkWriter.dll")]
        public static extern int StartSinkWriter();
        [DllImport("FireTerminator.SinkWriter.dll")]
        public static extern void FinishSinkWriter();

        private MainForm m_MainForm;
        private PlayControlForm m_PlayControlForm;
        private RenderTarget2D m_RenderTarget = null;
        private uint[] m_VideoBuffer = null;

        public override DevExpress.XtraBars.Ribbon.RibbonForm MainForm
        {
            get
            {
                lock (ProjectDoc.SyncObj)
                {
                    if (m_MainForm == null)
                        m_MainForm = new MainForm();
                }
                return m_MainForm;
            }
        }
        public override System.Windows.Forms.Panel PanelView
        {
            get { return m_MainForm.pnlMain; }
        }
        public override System.Windows.Forms.Panel PanelPreview
        {
            get { return m_MainForm.pnlPreview; }
        }

        private bool m_IsFullScreen = false;
        public override bool IsFullScreen
        {
            get { return m_IsFullScreen; }
            set
            {
                if (m_IsFullScreen != value)
                {
                    m_IsFullScreen = value;
                    m_MainForm.Visible = !m_IsFullScreen;
                    m_MainForm.icFullScreenPlay.Checked = m_IsFullScreen;
                    m_SrcGameForm.TopMost = false;// m_IsFullScreen;
                    m_SrcGameForm.Visible = m_IsFullScreen;
                    OnMainFormSizeChanged(null, null);
                    if (m_IsFullScreen)
                    {
                        if (m_PlayControlForm == null)
                            m_PlayControlForm = new PlayControlForm();
                        m_PlayControlForm.Show(m_SrcGameForm);
                    }
                    else if (m_PlayControlForm != null)
                    {
                        m_PlayControlForm.Hide();
                    }
                    System.Windows.Forms.Cursor.Show();
                }
            }
        }
        public bool IsSinkWriterInitialSucceeded
        {
            get;
            private set;
        }

        protected override void Initialize()
        {
            base.Initialize();

            m_MainForm.icFullScreenPlay.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(OnMainFormFullScreenPlayCheckedChanged);

            try
            {
                var size = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
                InitializeSinkWriter((uint)size.Width, (uint)size.Height, 10, 800000);
                IsSinkWriterInitialSucceeded = true;
            }
            catch
            {
            }
        }

        protected override void LoadContent()
        {
            ProjectDoc.Instance.ReloadResourceFiles();
        }

        protected override void UnloadContent()
        {
            ProjectDoc.Instance.ClearResourceFiles();
        }

        private void OnMainFormFullScreenPlayCheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IsFullScreen = ((DevExpress.XtraBars.BarCheckItem)e.Item).Checked;
        }

        protected override void OnMainFormSizeChanged(object sender, EventArgs e)
        {
            if (!Program.IsAppRunning)
                return;
            base.OnMainFormSizeChanged(sender, e);
            if (IsFullScreen)
            {
                m_RenderTarget = new RenderTarget2D(m_Graphics.GraphicsDevice,
                    m_Graphics.PreferredBackBufferWidth, m_Graphics.PreferredBackBufferHeight, 1,
                    m_Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat);
                if (m_VideoBuffer == null)
                    m_VideoBuffer = new uint[m_Graphics.PreferredBackBufferWidth * m_Graphics.PreferredBackBufferHeight];
            }
            else if (m_RenderTarget != null)
            {
                m_RenderTarget.Dispose();
                m_RenderTarget = null;
            }
        }

        private string VideoRecorder_SelectUserProFile(string[] profiles)
        {
            EncProfileSelector dlg = new EncProfileSelector(profiles);
            if (dlg.ShowDialog(m_SrcGameForm) != System.Windows.Forms.DialogResult.OK)
                return null;
            return dlg.SelectedResult;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (m_MainForm != null)
                m_MainForm.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void DrawProject()
        {
            if (ProjectDoc.Instance.SelectedSceneInfo == null)
                return;
            if (!IsFullScreen)
                base.DrawProject();
            else
            {
                GraphicsDevice.Viewport = ViewportMain;
                GraphicsDevice.SetRenderTarget(0, null);
                GraphicsDevice.Clear(Color.Black);
                ProjectDoc.Instance.SelectedSceneInfo.Draw(GraphicsDevice);
                GraphicsDevice.Present(null, null, (IntPtr)m_SrcGameForm.Handle);

                if (m_PlayControlForm.IsRecording)
                {
                    GraphicsDevice.SetRenderTarget(0, m_RenderTarget);
                    GraphicsDevice.Clear(Color.Black);
                    ProjectDoc.Instance.SelectedSceneInfo.Draw(GraphicsDevice);
                    GraphicsDevice.SetRenderTarget(0, null);

                    var txt = m_RenderTarget.GetTexture();
                    txt.GetData<uint>(m_VideoBuffer);
                    WriteFrame(m_VideoBuffer);
                }
            }
        }
    }
}
