using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Transitions;
using Microsoft.Xna.Framework.Input;

namespace FireTerminator.Common
{
    public abstract class GraphicsRenderer : Microsoft.Xna.Framework.Game
    {
        public GraphicsRenderer()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            ViewportMain.X = 0;
            ViewportMain.Y = 0;
            ViewportMain.Width = 0;
            ViewportMain.Height = 0;
            IsFullScreen = false;
        }

        protected GraphicsDeviceManager m_Graphics;
        protected System.Windows.Forms.Form m_SrcGameForm;
        protected ResourceInfo m_PreviewResourceInfo;
        protected Viewport ViewportMain = new Viewport();
        protected System.Windows.Forms.Keys m_PressingKeys = System.Windows.Forms.Keys.None;
        protected List<CubicCurveRibbon> m_CurveRibbons = new List<CubicCurveRibbon>();

        public virtual TransitionGraphics TransDrawer
        {
            get;
            set;
        }
        public virtual ResourceInfo PreviewResourceInfo
        {
            get { return m_PreviewResourceInfo; }
            set
            {
                if ((m_PreviewResourceInfo == null && value != null) ||
                    (m_PreviewResourceInfo != null && value == null) ||
                    (m_PreviewResourceInfo != null && value != null && m_PreviewResourceInfo.FullFilePath != value.FullFilePath))
                {
                    if (m_PreviewResourceInfo != null)
                        m_PreviewResourceInfo.Unload();
                    if (value == null)
                        m_PreviewResourceInfo = null;
                    else
                        m_PreviewResourceInfo = System.Activator.CreateInstance(ResourceInfo.ResTypes[value.Kind], value.ParentGroup, value.FullFilePath) as ResourceInfo;
                    if (m_PreviewResourceInfo != null)
                        m_PreviewResourceInfo.Load();
                }
            }
        }
        public virtual bool IsFullScreen
        {
            get;
            set;
        }
        public virtual System.Windows.Forms.Form FullScreenForm
        {
            get { return m_SrcGameForm; }
        }
        public abstract DevExpress.XtraBars.Ribbon.RibbonForm MainForm
        {
            get;
        }
        public abstract System.Windows.Forms.Panel PanelView
        {
            get;
        }
        public abstract System.Windows.Forms.Panel PanelPreview
        {
            get;
        }
        public CubicCurveRibbon CreatingRibbon
        {
            get;
            private set;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            ProjectDoc.Instance.HostGame = this;
            m_SrcGameForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
            m_SrcGameForm.ShowInTaskbar = false;
            m_SrcGameForm.Shown += new EventHandler(GameWindowForm_Shown);
            m_SrcGameForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            m_SrcGameForm.Cursor = System.Windows.Forms.Cursors.Default;

            MainForm.HandleDestroyed += new EventHandler(OnMainFormHandleDestroyed);
            MainForm.LocationChanged += new EventHandler(OnMainFormLocationChanged);
            MainForm.SizeChanged += new EventHandler(OnMainFormSizeChanged);
            PanelView.SizeChanged += new EventHandler(OnMainFormSizeChanged);
            PanelPreview.SizeChanged += new EventHandler(OnPanelPreviewSizeChanged);
            MainForm.Show();
            OnPanelPreviewSizeChanged(null, null);
        }

        protected virtual void OnPanelPreviewSizeChanged(object sender, EventArgs e)
        {
            var vi = ProjectDoc.Instance.PreviewViewport;
            vi.ViewportPtr.X = 0;
            vi.ViewportPtr.Y = 0;
            vi.ViewportPtr.Width = PanelPreview.Size.Width;
            vi.ViewportPtr.Height = PanelPreview.Size.Height;
        }

        private void OnMainFormHandleDestroyed(object sender, EventArgs e)
        {
            this.Exit();
        }

        private void GameWindowForm_Shown(object sender, EventArgs e)
        {
            ((System.Windows.Forms.Form)sender).Hide();
        }

        protected virtual void OnMainFormSizeChanged(object sender, EventArgs e)
        {
            if (MainForm.WindowState == System.Windows.Forms.FormWindowState.Minimized)
                return;
            System.Drawing.Size size;
            if (!IsFullScreen)
                size = PanelView.Size;
            else
            {
                m_SrcGameForm.Location = new System.Drawing.Point(0, 0);
                size = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            }
            if (size.Width > 0 && size.Height > 0)
            {
                m_SrcGameForm.Size = size;
                m_Graphics.PreferredBackBufferWidth = m_SrcGameForm.Size.Width;
                m_Graphics.PreferredBackBufferHeight = m_SrcGameForm.Size.Height;
                UpdateMainViewportSize();
                OnMainFormLocationChanged(sender, e);
            }
        }

        protected virtual void OnMainFormLocationChanged(object sender, EventArgs e)
        {
            if (MainForm.WindowState == System.Windows.Forms.FormWindowState.Minimized)
                return;
            m_SrcGameForm.Location = PanelView.PointToScreen(new System.Drawing.Point(0, 0));
            m_Graphics.ApplyChanges();
        }

        protected virtual void UpdateMainViewportSize()
        {
            ProjectDoc.Instance.ResolutionRatio = new System.Drawing.Size(m_Graphics.PreferredBackBufferWidth, m_Graphics.PreferredBackBufferHeight);
            if (IsFullScreen)
            {
                ViewportMain.Width = m_SrcGameForm.Width;
                ViewportMain.Height = m_SrcGameForm.Height;
            }
            else
            {
                ViewportMain.Width = PanelView.Width;
                ViewportMain.Height = PanelView.Height;
            }
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            try
            {
                if (!ProductInfo.ValidateProductLimit())
                    return;
                
                float elapsedTime = gameTime.ElapsedGameTime.Milliseconds * 0.001F;

                UpdateProject(elapsedTime);
                UpdateTransitions(elapsedTime);
                UpdatePreview(elapsedTime);
                UpdateOthers(elapsedTime);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            base.Update(gameTime);
        }
        protected virtual void UpdateProject(float elapsedTime)
        {
            var proj = ProjectDoc.Instance.SelectedProject;
            if (proj != null)
            {
                proj.Update(elapsedTime);

                var ks = Keyboard.GetState().GetPressedKeys();
                var kr = System.Windows.Forms.Keys.None;
                if (ks.Length > 0)
                {
                    foreach (var k in ks)
                    {
                        if (k == Keys.LeftControl || k == Keys.RightControl)
                            kr |= System.Windows.Forms.Keys.Control;
                        else if (k == Keys.LeftAlt || k == Keys.RightAlt)
                            kr |= System.Windows.Forms.Keys.Alt;
                        else if (k == Keys.LeftShift || k == Keys.RightShift)
                            kr |= System.Windows.Forms.Keys.Shift;
                        else
                            kr |= (System.Windows.Forms.Keys)k;
                    }
                }
                if (m_PressingKeys != kr)
                {
                    m_PressingKeys = kr;
                    proj.OnKeyDown(kr);
                }
            }
        }
        protected virtual void UpdateTransitions(float elapsedTime)
        {
            if (TransDrawer != null)
                TransDrawer.Update(elapsedTime);
        }
        protected virtual void UpdatePreview(float elapsedTime)
        {
            if (PreviewResourceInfo != null && PreviewResourceInfo.PreviewElement != null)
            {
                float zero = 0;
                PreviewResourceInfo.PreviewElement.Update(elapsedTime, ref zero);
            }
        }
        private float m_fGCElapsedTime = 0;
        private float m_fRefTestElapsedTime = 0;
        protected virtual void UpdateOthers(float elapsedTime)
        {
            for (int i = 0; i < m_CurveRibbons.Count; ++i)
            {
                m_CurveRibbons[i].Update(elapsedTime);
                if (!m_CurveRibbons[i].IsActive)
                {
                    m_CurveRibbons[i].Reset();
                    m_CurveRibbons.RemoveAt(i--);
                }
            }
            if (CreatingRibbon != null)
            {
                CreatingRibbon.Update(elapsedTime);
            }
            m_fGCElapsedTime += elapsedTime;
            if (m_fGCElapsedTime >= 10)
            {
                m_fGCElapsedTime = 0;
                GC.Collect();
            }
            m_fRefTestElapsedTime += elapsedTime;
            if (m_fRefTestElapsedTime >= 5)
            {
                int count = ProjectDoc.Instance.CheckResourceFinalizations();
                if (count > 0)
                {
                    //System.Diagnostics.Debug.WriteLine(String.Format("{0}:Released {1} Resource(s).", DateTime.Now.ToShortTimeString(), count));
                }
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            PrepareRenderStates();
            try
            {
                DrawProject();
                DrawTransitions();
                DrawPreview();

                base.Draw(gameTime);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        protected virtual void PrepareRenderStates()
        {
            GraphicsDevice.RenderState.AlphaBlendEnable = true;
            GraphicsDevice.RenderState.SourceBlend = Blend.BothSourceAlpha;
            GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RenderState.MultiSampleAntiAlias = true;
            //GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
        }
        protected virtual void DrawProject()
        {
            GraphicsDevice.Viewport = ViewportMain;
            GraphicsDevice.Clear(Color.Black);
            if (ProjectDoc.Instance.SelectedSceneInfo != null)
            {
                ProjectDoc.Instance.SelectedSceneInfo.Draw(GraphicsDevice);
                DrawProjectOthers();
            }
            var rect = new Rectangle(0, 0, PanelView.Width, PanelView.Height);
            GraphicsDevice.Present(rect, rect, (IntPtr)PanelView.Handle);
        }
        protected virtual void DrawTransitions()
        {
            if (TransDrawer != null && !IsFullScreen)
            {
                TransDrawer.Draw(GraphicsDevice);
            }
        }
        protected virtual void DrawPreview()
        {
            if (!IsFullScreen)
            {
                GraphicsDevice.Clear(Color.Black);
                GraphicsDevice.Viewport = ProjectDoc.Instance.PreviewViewport.ViewportPtr;
                if (PreviewResourceInfo != null)
                    PreviewResourceInfo.PreviewElement.Draw();
                var rect = new Rectangle(0, 0, PanelPreview.Width, PanelPreview.Height);
                GraphicsDevice.Present(rect, rect, (IntPtr)PanelPreview.Handle);
            }
        }
        protected virtual void DrawProjectOthers()
        {
            foreach (var ribbon in m_CurveRibbons)
            {
                if (ProjectDoc.Instance.SelectedViewportInfo == ribbon.ParentViewport)
                    ribbon.Draw();
            }
            if (CreatingRibbon != null)
            {
                CreatingRibbon.Draw();
            }
        }
        public CubicCurveRibbon CreateCurveRibbon()
        {
            if (CreatingRibbon != null)
                ConfirmCurveRibbon(-1);
            var vi = ProjectDoc.Instance.SelectedViewportInfo;
            if (vi == null)
                return null;
            CreatingRibbon = new CubicCurveRibbon(vi);
            return CreatingRibbon;
        }
        public void ConfirmCurveRibbon(float lifeTime)
        {
            if (CreatingRibbon != null)
            {
                CreatingRibbon.LifeTime = lifeTime;
                m_CurveRibbons.Add(CreatingRibbon);
                CreatingRibbon = null;
            }
        }
    }
}
