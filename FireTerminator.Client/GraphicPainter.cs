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
using FireTerminator.Common.Transitions;
using FireTerminator.Common.RenderResources;
using FireTerminator.Common.Operations;

namespace FireTerminator.Client
{
    public class GraphicPainter : GraphicsRenderer
    {
        public GraphicPainter()
        {
        }

        private MainForm m_MainForm;
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

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ProjectDoc.Instance.ReloadResourceFiles();
        }

        protected override void UnloadContent()
        {
            ProjectDoc.Instance.ClearResourceFiles();
        }
    }
}
