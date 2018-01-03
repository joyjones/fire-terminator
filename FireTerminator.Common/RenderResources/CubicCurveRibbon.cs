using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FireTerminator.Common.RenderResources
{
    public class CubicCurveRibbon
    {
        public CubicCurveRibbon(ViewportInfo vi)
        {
            ParentViewport = vi;
            ResetLifeTime = 0;
            Width = ProjectDoc.Instance.DefaultCubicCurveRibbonWidth;
            FillColor = ProjectDoc.Instance.DefaultCubicCurveRibbonColor;
            DetailPrecision = 0.02F;
        }
        public CubicCurveRibbon(ViewportInfo vi, float lifeTime)
            : this(vi)
        {
            LifeTime = lifeTime;
        }
        public ViewportInfo ParentViewport
        {
            get;
            private set;
        }
        private float m_LifeTime = -1;
        public float LifeTime
        {
            get { return m_LifeTime; }
            set
            {
                if (value < 0)
                    value = -1;
                if (m_LifeTime != value)
                {
                    m_LifeTime = value;
                    if (ResetLifeTime == 0 && m_LifeTime > 0)
                        ResetLifeTime = m_LifeTime;
                }
            }
        }
        public float ResetLifeTime
        {
            get;
            private set;
        }
        public float Width
        {
            get;
            set;
        }
        public Color FillColor
        {
            get;
            set;
        }
        public float DetailPrecision
        {
            get;
            set;
        }
        public bool IsActive
        {
            get { return LifeTime < 0 || ResetLifeTime > 0; }
        }
        protected CubicCurve3 m_Curve = new CubicCurve3();
        private List<VertexPositionColor> m_Vectors = new List<VertexPositionColor>();
        private static short[] sm_Indices = new short[] { 0, 1, 2, 1, 3, 2 };

        public void Reset()
        {
            m_Curve.RemoveAll();
            m_Vectors.Clear();
            ResetLifeTime = 0;
        }
        public void AppendPoint(float x, float y)
        {
            m_Curve.AppendNode(new Vector3(x, y, 0));
            BuildRibbon();
        }
        private void BuildRibbon()
        {
            m_Vectors.Clear();
            List<Vector3> nodes = new List<Vector3>();
            int count = (int)(m_Curve.MaxDistance / DetailPrecision);
            for (int i = 0; i <= count; ++i)
            {
                nodes.Add(m_Curve.GetPosition(i / (float)count));
            }
            if (nodes.Count <= 1)
            {
                return;
            }
            List<Vector3> poses = new List<Vector3>();
            float rw = ParentViewport.GetLengthRate(true, Width);
            for (int i = 0; i < nodes.Count; ++i)
            {
                Vector3 p1 = nodes[i];
                Vector3 p2;
                if (i == nodes.Count - 1)
                    p2 = nodes[i - 1];
                else
                    p2 = nodes[i + 1];
                Vector3 dir1 = p2 - p1;
                dir1.Normalize();
                Vector3 sp1, sp2;
                if (i == 0)
                {
                    sp1 = Vector3.Cross(dir1, Vector3.UnitZ) * rw;
                    sp2 = sp1 * -1;
                }
                else if (i == nodes.Count - 1)
                {
                    sp2 = Vector3.Cross(dir1, Vector3.UnitZ) * rw;
                    sp1 = sp2 * -1;
                }
                else
                {
                    Vector3 p0 = nodes[i - 1];
                    Vector3 dir0 = p1 - p0;
                    dir0.Normalize();
                    sp1 = dir1 - dir0;
                    if (sp1.Length() == 0)
                        sp1 = Vector3.Cross(dir0, Vector3.UnitZ) * rw;
                    else
                    {
                        sp1.Normalize();
                        sp1 *= rw;
                    }
                    sp2 = sp1 * -1;
                    if (Vector3.Cross(dir0, sp1).Z > 0)
                    {
                        var sp = sp1;
                        sp1 = sp2;
                        sp2 = sp;
                    }
                }
                poses.Add(p1 + sp1);
                poses.Add(p1 + sp2);
            }
            for (int i = 0; i < poses.Count - 2; i += 2)
            {
                for (int j = 0; j < sm_Indices.Length; ++j)
                {
                    var pos = poses[i + sm_Indices[j]];
                    var posAbs = ParentViewport.GetRateLocation(true, new System.Drawing.PointF(pos.X, pos.Y));
                    m_Vectors.Add(new VertexPositionColor(new Vector3(posAbs.X, posAbs.Y, 0), FillColor));
                }
            }
        }
        public void Update(float elapsedTime)
        {
            if (LifeTime > 0)
            {
                ResetLifeTime -= elapsedTime;
                if (ResetLifeTime < 0)
                    ResetLifeTime = 0;
            }
        }
        public void Draw()
        {
            BasicEffect effect = ParentViewport.ParentSceneInfo.UsingEffect;
            if (IsActive && effect != null && m_Vectors.Count > 0)
            {
                effect.LightingEnabled = false;
                effect.TextureEnabled = false;
                effect.Begin();
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();
                    effect.GraphicsDevice.VertexDeclaration = new VertexDeclaration(effect.GraphicsDevice, VertexPositionColor.VertexElements);
                    effect.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, m_Vectors.ToArray(), 0, m_Vectors.Count / 3);
                    pass.End();
                }
                effect.End();
            }
        }
    }
}
