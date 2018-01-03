using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;

namespace FireTerminator.Common.RenderResources
{
    public class CubicCurve3
    {
	    public class SubPoint
	    {
		    public Vector3 Pos = new Vector3(0);
		    public Vector3 Velocity1 = new Vector3(0);
            public Vector3 Velocity2 = new Vector3(0);
		    public float Distance = 0;
	    };

        public static Matrix MatHermite =
            new Matrix( 2,-2, 1, 1,
                       -3, 3,-2,-1,
                        0, 0, 1, 0,
                        1, 0, 0, 0);
        public static Vector3 CalcVel(Vector3 vAim, Vector3 vSrc, float fZoom)
        {
            Vector3 vVec = vAim - vSrc;
            vVec.Normalize();
            return vVec * fZoom;
        }
        // cubic curve defined by 2 positions and 2 velocities
        public static Vector3 GetPositionOnCubic(Vector3 startPos, Vector3 startVel, Vector3 endPos, Vector3 endVel, float time)
        {
	        Matrix m = new Matrix(
                startPos.X, startPos.Y, startPos.Z, 0,
		        endPos.X,	endPos.Y,	endPos.Z,   0,
		        startVel.X,	startVel.Y,	startVel.Z, 0,
		        endVel.X,	endVel.Y,	endVel.Z,   1);
	        m = MatHermite * m;

            Vector3 timeVector = new Vector3(time * time * time, time * time, time);
            timeVector = Vector3.Transform(timeVector, m);
	        return timeVector;
        }

	    public CubicCurve3()
        {
	    }
        public void AppendNode(Vector3 pos)
        {
            InsertNode(m_aNodeArray.Count, pos);
        }
	    public virtual void InsertNode(int idx, Vector3 pos)
        {
            int nNodeCnt = m_aNodeArray.Count;
            if (idx < 0)
                idx = 0;
            if (idx > nNodeCnt)
                idx = nNodeCnt;
            m_aNodeArray.Insert(idx, new SubPoint());
            nNodeCnt = m_aNodeArray.Count;
            SetPosition(idx, pos);
            BuildVelocity(idx);
            if (idx > 1)
                BuildVelocity(idx - 1);
            if (idx < nNodeCnt)
                BuildVelocity(idx + 1);
        }
	    public virtual void DeleteNode(int idx)
        {
	        if ( idx >= 0 && idx < m_aNodeArray.Count )
	        {
		        m_fMaxDistance -= m_aNodeArray[idx].Distance;
		        m_aNodeArray.RemoveAt(idx);
		        int nCnt = m_aNodeArray.Count;
		        if ( nCnt > 0 )
		        {
			        if ( idx == 0 )
				        m_aNodeArray[idx].Velocity1 = Vector3.Zero;
			        if ( idx == nCnt )
                        m_aNodeArray[nCnt - 1].Velocity2 = Vector3.Zero;
		        }
		        //if ( idx > 0 )
		        //	setPosition(idx-1, m_aNodeArray[idx-1].vPos);
		        //if ( idx >= 0 )
		        //	setPosition(idx, m_aNodeArray[idx].vPos);
	        }
        }
	    public virtual void SetPosition(int idx, Vector3 pos)
        {
            int nNodeCnt = m_aNodeArray.Count;
            if (idx >= 0 && idx < nNodeCnt)
            {
                SubPoint sTheNode = m_aNodeArray[idx];
                sTheNode.Pos = pos;
                if (idx > 0)
                    m_aNodeArray[idx - 1].Distance = (m_aNodeArray[idx - 1].Pos - pos).Length();
                if (idx < nNodeCnt - 1)
                    sTheNode.Distance = (sTheNode.Pos - m_aNodeArray[idx + 1].Pos).Length();
                m_fMaxDistance = 0;
                for (int i = 0; i < nNodeCnt; ++i)
                    m_fMaxDistance += m_aNodeArray[i].Distance;

                if (idx <= 1 || idx >= nNodeCnt - 2)
                    IsTangentOnTail = m_bTangentOnTail;
            }
        }
        public Vector3 GetPosition(float time)
        {
            int resultIndex;
            Vector3 resultVel;
            return GetPosition(time, out resultIndex, out resultVel);
        }
	    public virtual Vector3 GetPosition(float fTime, out int resultIndex, out Vector3 resultVel)
        {
	        float fDist = fTime * m_fMaxDistance;
	        float fCurDist = 0;
	        int nNodeCnt = m_aNodeArray.Count;
	        int i = 0;
	        while (i < nNodeCnt && fCurDist + m_aNodeArray[i].Distance <= fDist )
	        {
		        fCurDist += m_aNodeArray[i].Distance;
		        i++;
	        }
	        if ( i < nNodeCnt - 1 )
	        {
		        float fNextFar = m_aNodeArray[i].Distance;
		        float t = fNextFar != 0 ? ((fDist - fCurDist) / fNextFar) : 0;
		        Vector3 startVel = m_aNodeArray[i].Velocity2;
		        Vector3 endVel = -1 * m_aNodeArray[i+1].Velocity1;
                resultIndex = i;
                resultVel = startVel;
		        return GetPositionOnCubic(m_aNodeArray[i].Pos, startVel, m_aNodeArray[i+1].Pos, endVel, t);
	        }
            else if (nNodeCnt > 1)
            {
                resultIndex = nNodeCnt - 1;
                resultVel = m_aNodeArray[nNodeCnt - 2].Velocity2;
                return m_aNodeArray[nNodeCnt - 1].Pos;
            }
            else
            {
                resultIndex = -1;
                resultVel = Vector3.Zero;
            }
	        return Vector3.Zero;
        }
	    public virtual List<Vector3> GenerateLineNodes(float fDistStep /*= 1.f*/, bool bGetLessNode /*= TRUE*/)
        {
            // 生成步幅为fDistStep的最少数量的直线段节点集合
            var result = new List<Vector3>();
            if (fDistStep <= 0)
                fDistStep = m_fMaxDistance / 100.0f;
            for (float d = 0; d <= m_fMaxDistance; d += fDistStep)
            {
                float t = d / m_fMaxDistance;
                Vector3 vPos = GetPosition(t);
                int nNowSize = result.Count;
                if (nNowSize < 2 || d == m_fMaxDistance)
                    result.Add(vPos);
                else
                {
                    if (!bGetLessNode)
                        result.Add(vPos);
                    else
                    {
                        Vector3 vPrePos1 = result[nNowSize - 2];
                        Vector3 vPrePos2 = result[nNowSize - 1];
                        Vector3 vInv1 = vPrePos2 - vPrePos1;
                        Vector3 vInv2 = vPos - vPrePos2;
                        float fVal = Vector3.Cross(vInv1, vInv2).LengthSquared();
                        if (fVal > 1e-3f * fDistStep)
                            result.Add(vPos);
                    }
                }
                if (d < m_fMaxDistance - m_fMaxDistance * 0.0001f && d + fDistStep > m_fMaxDistance)
                    d = m_fMaxDistance - fDistStep;
            }
            return result;
        }
	    public virtual void BuildVelocity(int idx)
        {
	        int nNodeCnt = m_aNodeArray.Count;
	        if (idx < 0 || idx >= nNodeCnt) return;

	        SubPoint sNode = m_aNodeArray[idx];
	        if ( idx == 0 )
	        {
		        // 调整第一个节点
		        sNode.Velocity1 = Vector3.Zero;
		        if ( nNodeCnt > 1 )
			        sNode.Velocity2 = CalcVel(m_aNodeArray[1].Pos, sNode.Pos, sNode.Distance);
		        // 调整最后一个节点
		        if ( nNodeCnt == 2 )
		        {
			        m_aNodeArray[1].Velocity1 = CalcVel(sNode.Pos, m_aNodeArray[1].Pos, sNode.Distance);
			        m_aNodeArray[1].Velocity2 = Vector3.Zero;
		        }
	        }
	        else if ( idx == nNodeCnt - 1 )
	        {
		        // 调整第一个节点
		        if ( idx == 1 )
		        {
			        m_aNodeArray[0].Velocity1 = Vector3.Zero;
			        m_aNodeArray[0].Velocity2 = CalcVel(sNode.Pos, m_aNodeArray[0].Pos, m_aNodeArray[0].Distance);
		        }
		        // 调整最后一个节点
		        if ( idx > 0 )
		        {
			        sNode.Velocity1 = CalcVel(m_aNodeArray[idx-1].Pos, sNode.Pos, m_aNodeArray[idx-1].Distance);
			        sNode.Velocity2 = Vector3.Zero;
		        }
	        }
	        else
	        {
		        Vector3 v1 = m_aNodeArray[idx-1].Pos - sNode.Pos;
		        Vector3 v2 = m_aNodeArray[idx+1].Pos - sNode.Pos;
                var n1 = v1; n1.Normalize();
                var n2 = v2; n2.Normalize();
                Vector3 vTg = n2 - n1; vTg.Normalize();
		        sNode.Velocity2 = vTg * sNode.Distance;
		        sNode.Velocity1 = -1 * vTg * sNode.Distance;
		        // 调整第一个节点
		        if ( idx == 1 )
		        {
			        m_aNodeArray[0].Velocity1 = Vector3.Zero;
			        m_aNodeArray[0].Velocity2 = CalcVel(sNode.Pos, m_aNodeArray[0].Pos, m_aNodeArray[0].Distance);
		        }
		        // 调整最后一个节点
		        if ( idx == nNodeCnt - 2 )
		        {
			        m_aNodeArray[nNodeCnt-1].Velocity1 = CalcVel(sNode.Pos, m_aNodeArray[nNodeCnt-1].Pos, sNode.Distance);
			        m_aNodeArray[nNodeCnt-1].Velocity2 = Vector3.Zero;
		        }
	        }
        }
	    public void SetBeginVel(int idx, Vector3 vel, bool bWithEnd)// = TRUE)
        {
            if (idx >= 0 && idx < m_aNodeArray.Count)
            {
                Vector3 vPreVel = m_aNodeArray[idx].Velocity1;
                if (vPreVel == vel)
                    return;
                if (vel.Length() < 0.0001f)
                {
                    m_aNodeArray[idx].Velocity1 = m_aNodeArray[idx].Velocity1;
                    m_aNodeArray[idx].Velocity1.Normalize();
                }
                else
                {
                    m_aNodeArray[idx].Velocity1 = vel;
                    if (bWithEnd)
                    {
                        Vector3 vCrs = Vector3.Cross(vel, vPreVel);
                        float fSrcLen2 = vPreVel.LengthSquared();
                        float fAimLen2 = vel.LengthSquared();
                        if (fSrcLen2 != 0 && fAimLen2 != 0 && vCrs.Length() > 0.001f)
                        {
                            float f3rdLen2 = (vPreVel - vel).LengthSquared();
                            float fAngle = (float)Math.Acos((fSrcLen2 + fAimLen2 - f3rdLen2) / (2 * Math.Sqrt(fSrcLen2 * fAimLen2)));
                            if (fAngle != 0)
                            {
                                Matrix matRot = Matrix.CreateFromAxisAngle(vCrs, -fAngle);
                                m_aNodeArray[idx].Velocity2 = Vector3.Transform(m_aNodeArray[idx].Velocity2, matRot);
                            }
                        }
                    }
                }
            }
        }
	    public void SetEndVel(int idx, Vector3 vel, bool bWithBegin)// = TRUE)
        {
            if (idx >= 0 && idx < m_aNodeArray.Count)
            {
                Vector3 vPreVel = m_aNodeArray[idx].Velocity2;
                if (vPreVel == vel)
                    return;
                if (vel.Length() < 0.0001f)
                {
                    m_aNodeArray[idx].Velocity2 = m_aNodeArray[idx].Velocity2;
                    m_aNodeArray[idx].Velocity2.Normalize();
                }
                else
                {
                    m_aNodeArray[idx].Velocity2 = vel;
                    if (bWithBegin)
                    {
                        Vector3 vCrs = Vector3.Cross(vel, vPreVel);
                        float fSrcLen2 = vPreVel.LengthSquared();
                        float fAimLen2 = vel.LengthSquared();
                        if (fSrcLen2 != 0 && fAimLen2 != 0 && vCrs.Length() > 0.001f)
                        {
                            float f3rdLen2 = (vPreVel - vel).LengthSquared();
                            float fAngle = (float)Math.Acos((fSrcLen2 + fAimLen2 - f3rdLen2) / (2 * Math.Sqrt(fSrcLen2 * fAimLen2)));
                            if (fAngle != 0)
                            {
                                Matrix matRot = Matrix.CreateFromAxisAngle(vCrs, -fAngle);
                                m_aNodeArray[idx].Velocity1 = Vector3.Transform(m_aNodeArray[idx].Velocity1, matRot);
                            }
                        }
                    }
                }
            }
        }
	    public void BuildVelocities()
        {
            for (int i = 0; i < m_aNodeArray.Count; ++i)
                BuildVelocity(i);
        }

	    public bool IsTangentOnTail
        {
            get { return m_bTangentOnTail; }
            set
            {
                m_bTangentOnTail = value;
                int nNodeCnt = m_aNodeArray.Count;
                if (!m_bTangentOnTail && nNodeCnt > 0)
                {
                    BuildVelocity(0);
                    if (nNodeCnt > 1)
                        BuildVelocity(nNodeCnt - 1);
                }
            }
        }
	    public void CopyFrom(CubicCurve3 r)
        {
            m_aNodeArray.Clear();
            for (int i = 0; i < r.m_aNodeArray.Count; ++i)
                m_aNodeArray.Add(r.m_aNodeArray[i]);
            m_fMaxDistance = r.m_fMaxDistance;
            m_bTangentOnTail = r.m_bTangentOnTail;
        }
	    public virtual void RemoveAll()
        {
            m_aNodeArray.Clear();
            m_fMaxDistance = 0.0f;
        }
        public SubPoint this[int i]
        {
            get
            {
                if (i >= 0 && i < m_aNodeArray.Count)
                    return m_aNodeArray[i];
                return null;
            }
        }

	    public int NodeCount
        {
		    get { return m_aNodeArray.Count; }
	    }
	    public float MaxDistance
        {
		    get { return m_fMaxDistance; }
	    }

        // 顶点
	    protected List<SubPoint> m_aNodeArray = new List<SubPoint>();
        // 线段集合总长度
	    protected float	m_fMaxDistance = 0;
        // 在首尾处设定切线
	    protected bool m_bTangentOnTail = false;
    };

    public class CubicCurve2 : CubicCurve3
    {
	    public CubicCurve2()
        {
	        m_bTangentOnTail = true;
	        InsertNode(0, new Vector3(0,m_fValueBgn,0));
            InsertNode(1, new  Vector3(m_fHoriLength, m_fValueEnd, 0));
	        BuildVelocities();
        }
	    public CubicCurve2(float fVal1, float fVal2, float fAspect, float fMin /*= FLT_MIN*/, float fMax /*= FLT_MAX*/)
        {
            m_fValueBgn = fVal1;
            m_fValueEnd = fVal2;
	        m_bTangentOnTail = true;
            m_fValueMin = fMin == float.MinValue ? (-Math.Abs(Math.Max(fVal1, fVal2)) * 10) : fMin;
            m_fValueMax = fMax == float.MaxValue ? (Math.Abs(Math.Max(fVal1, fVal2)) * 10) : fMax;
	        m_fValueMax = m_fValueMax < m_fValueMin ? m_fValueMin : m_fValueMax;
	        SetViewAspect(fAspect);
	        InsertNode(0, new Vector3(0,m_fValueBgn,0));
            InsertNode(1, new Vector3(m_fHoriLength, m_fValueEnd, 0));
	        BuildVelocities();
        }
	    public override void InsertNode(int idx, Vector3 pos)
        {
	        if ( idx == 0 ) idx = 1;
	        Vector3 okpos = pos;
	        okpos.X = okpos.X < 0 ? 0 : okpos.X;
	        okpos.X = okpos.X > m_fHoriLength ? m_fHoriLength : okpos.X;
	        okpos.Y = okpos.Y < m_fValueMin ? m_fValueMin : okpos.Y;
	        okpos.Y = okpos.Y > m_fValueMax ? m_fValueMax : okpos.Y;
            okpos.Z = 0;
            base.InsertNode(idx, okpos);
	        m_bStraightMode = false;
        }
	    public override void DeleteNode(int idx)
        {
	        if ( idx != 0 && idx != NodeCount - 1 )
                base.DeleteNode(idx);
        }
	    public override void SetPosition(int idx, Vector3 pos)
        {
	        Vector3 okpos = pos;
	        okpos.Z = 0;
	        if ( idx == 0 )
		        okpos.X = 0;
	        else if ( idx == NodeCount - 1 )
		        okpos.X = m_fHoriLength;
	        okpos.Y = okpos.Y < m_fValueMin ? m_fValueMin : okpos.Y;
	        okpos.Y = okpos.Y > m_fValueMax ? m_fValueMax : okpos.Y;
	        if ( !m_bStraightMode )
	        {
		        base.SetPosition(idx, okpos);
		        if ( idx == 0 )
			        m_fValueBgn = okpos.Y;
		        else if ( idx == m_aNodeArray.Count - 1 )
			        m_fValueEnd = okpos.Y;
	        }
	        else
	        {
                base.SetPosition(0, new Vector3(0, okpos.Y, 0));
                base.SetPosition(1, new Vector3(m_fHoriLength, okpos.Y, 0));
		        m_fValueBgn = m_fValueEnd = okpos.Y;
	        }
        }
	    public override Vector3 GetPosition(float fTime, out int resultIndex, out Vector3 resultVel)
        {
            if (m_bStraightMode || fTime == 0)
            {
                resultIndex = -1;
                resultVel = Vector3.Zero;
                return new Vector3(0, BeginValue, 0);
            }
            return base.GetPosition(fTime, out resultIndex, out resultVel);
        }
	    public override List<Vector3> GenerateLineNodes(float fDistStep /*= 1.f*/, bool bGetLessNode)// = TRUE)
        {
	        if ( !m_bStraightMode )
		        return base.GenerateLineNodes(fDistStep, bGetLessNode);
	        else
	        {
		        var result = new List<Vector3>();
                System.Diagnostics.Debug.Assert(m_aNodeArray.Count == 2);
		        for ( int i = 0; i < m_aNodeArray.Count; ++i )
                    result.Add(m_aNodeArray[i].Pos);
                return result;
	        }
        }
	    public override void RemoveAll()
        {
	        base.RemoveAll();
	        InsertNode(0, new Vector3(0,m_fValueBgn,0));
            InsertNode(1, new Vector3(m_fHoriLength, m_fValueEnd, 0));
	        BuildVelocities();
        }
	    public float GetValue(float fTime)
        {
            if (m_bStraightMode || fTime == 0)
                return BeginValue;
            return GetPosition(fTime).Y;
        }
	    public void CopyFrom(CubicCurve2 r)
        {
            m_aNodeArray.Clear();
            for (int i = 0; i < r.m_aNodeArray.Count; ++i)
                m_aNodeArray.Add(r.m_aNodeArray[i]);
            m_fMaxDistance = r.m_fMaxDistance;
            m_bTangentOnTail = r.m_bTangentOnTail;
            m_fValueBgn = r.m_fValueBgn;
            m_fValueEnd = r.m_fValueEnd;
            m_fValueMin = r.m_fValueMin;
            m_fValueMax = r.m_fValueMax;
            m_fHoriLength = r.m_fHoriLength;
        }
	    public void SetViewAspect(float fAspect)
        {
            // w / h
            if (fAspect > 0)
            {
                float fSrcLen = m_fHoriLength;
                m_fHoriLength = ValueRange * fAspect;
                float fZoom = m_fHoriLength / fSrcLen;
                if (fZoom != 1)
                {
                    for (int i = 0; i < m_aNodeArray.Count; ++i)
                    {
                        Vector3 v = m_aNodeArray[i].Pos;
                        v.X *= fZoom;
                        SetPosition(i, v);
                        m_aNodeArray[i].Velocity1.X *= fZoom;
                        m_aNodeArray[i].Velocity2.X *= fZoom;
                    }
                }
            }
        }
	    public void SetValue(float fBgn, float fEnd, float fRand)
        {
            BeginValue = fBgn;
            EndValue = fEnd;
            ValRand = fRand;
        }
	    public void SetValRange(float fMin, float fMax)
        {
	        fMax = fMax < fMin ? fMin : fMax;
	        m_fValueMin = fMin;
	        m_fValueMax = fMax;
	        for ( int i = 0; i < m_aNodeArray.Count; ++i )
	        {
		        Vector3 pos = m_aNodeArray[i].Pos;
		        if ( pos.Y < fMin )
		        {
			        pos.Y = fMin;
			        base.SetPosition(i, pos);
		        }
		        else if ( pos.Y > fMax )
		        {
			        pos.Y = fMax;
                    base.SetPosition(i, pos);
		        }
	        }
        }

        public bool IsStraightMode
        {
            get { return m_bStraightMode; }
            set
            {
                if (value)
                {
                    m_fValueEnd = m_fValueBgn;
                    RemoveAll();
                }
                m_bStraightMode = value;
            }
	    }
	    public bool IsRandomOnHead
        {
		    get { return m_bRandomOnHead; }
		    set { m_bRandomOnHead = value; }
	    }
	    public float BeginValue
        {
            get
            {
		        if ( m_bRandomOnHead )
                {
                    var rnd = new System.Random();
                    return m_fValueBgn + m_fValueRandomRange * (((float)((rnd.Next() % 2) - 1) * (float)rnd.Next()) / 0x7fff);
                }
		        return m_fValueBgn;
            }
	        set
            {
                value = value < m_fValueMin ? m_fValueMin : value;
                value = value > m_fValueMax ? m_fValueMax : value;
                m_fValueBgn = value;
                SetPosition(0, new Vector3(0, m_fValueBgn, 0));
            }
	    }
	    public float EndValue
        {
		    get { return m_fValueEnd; }
	        set
            {
                value = value < m_fValueMin ? m_fValueMin : value;
                value = value > m_fValueMax ? m_fValueMax : value;
                m_fValueEnd = value;
                SetPosition(m_aNodeArray.Count - 1, new Vector3(m_fHoriLength, m_fValueEnd, 0));
            }
	    }
	    public float MinValue
        {
		    get { return m_fValueMin; }
	    }
	    public float MaxValue
        {
		    get { return m_fValueMax; }
	    }
	    public float ValueRange
        {
            get
            {
		        float v = m_fValueMax - m_fValueMin;
		        //if ( fabsf(v) < 2.f )
		        //	return 2.f;
		        return v;
            }
	    }
	    public float ValRand
        {
		    get { return m_fValueRandomRange; }
		    set { m_fValueRandomRange = value; }
	    }
	    public float HoriLength
        {
		    get { return m_fHoriLength; }
        }
        public string ValueString
        {
            get
            {
                // bTan;bRdHead;bStaight;fHorLen;fMin;fMax;{vPos;vVel1;vVel2}
                string strVal = String.Format("T:{0};R:{1},{2};S:{3};H:{4};A:{5};B:{6};", m_bTangentOnTail, m_bRandomOnHead, m_fValueRandomRange, m_bStraightMode, m_fHoriLength, m_fValueMin, m_fValueMax);
                strVal += "{";
                int nCnt = m_aNodeArray.Count;
                for (int i = 0; i < nCnt; ++i)
                {
                    SubPoint sp = m_aNodeArray[i];
                    strVal += String.Format("({0},{1})[{2},{3},{4}][{5},{6},{7}]", sp.Pos.X, sp.Pos.Y, sp.Velocity1.X, sp.Velocity1.Y, sp.Velocity1.Z, sp.Velocity2.X, sp.Velocity2.Y, sp.Velocity2.Z);
                    if (i < nCnt - 1)
                        strVal += ";";
                    else
                        strVal += "}";
                }
                return strVal;
            }
            set
            {
                string strVal = value;
                var mat = Regex.Match(value, "T:(?'T'.+?);R:(?'R1'.+?),(?'R2'.+?);S:(?'S'.+?);H:(?'H'.+?);A:(?'A'.+?);B:(?'B'.+?);");
                if (mat.Success)
                {
                    m_bTangentOnTail = bool.Parse(mat.Groups["T"].Value);
                    m_bRandomOnHead = bool.Parse(mat.Groups["R1"].Value);
                    m_fValueRandomRange = float.Parse(mat.Groups["R2"].Value);
                    m_bStraightMode = bool.Parse(mat.Groups["S"].Value);
                    m_fHoriLength = float.Parse(mat.Groups["H"].Value);
                    m_fValueMin = float.Parse(mat.Groups["A"].Value);
                    m_fValueMax = float.Parse(mat.Groups["B"].Value);

                    base.RemoveAll();
                    var subMat = Regex.Match(value, @"\((?'A'.+?),(?'B'.+?)\)\[(?'X1'.+?),(?'Y1'.+?),(?'Z1'.+?)\]\[(?'X2'.+?),(?'Y2'.+?),(?'Z2'.+?)\]");
                    while (subMat.Success)
                    {
                        SubPoint sp = new SubPoint();
                        sp.Pos.X = float.Parse(subMat.Groups["A"].Value);
                        sp.Pos.Y = float.Parse(subMat.Groups["B"].Value);
                        sp.Velocity1.X = float.Parse(subMat.Groups["X1"].Value);
                        sp.Velocity1.Y = float.Parse(subMat.Groups["Y1"].Value);
                        sp.Velocity1.Z = float.Parse(subMat.Groups["Z1"].Value);
                        sp.Velocity2.X = float.Parse(subMat.Groups["X2"].Value);
                        sp.Velocity2.Y = float.Parse(subMat.Groups["Y2"].Value);
                        sp.Velocity2.Z = float.Parse(subMat.Groups["Z2"].Value);
                        m_aNodeArray.Add(sp);

                        subMat = subMat.NextMatch();
                    }
                    m_fMaxDistance = 0;
                    for (int i = 0; i < m_aNodeArray.Count - 1; ++i)
                    {
                        m_aNodeArray[i].Distance = (m_aNodeArray[i + 1].Pos - m_aNodeArray[i].Pos).Length();
                        m_fMaxDistance += m_aNodeArray[i].Distance;
                        if (i == 0)
                            m_fValueBgn = m_aNodeArray[i].Pos.Y;
                        if (i == m_aNodeArray.Count - 2)
                            m_fValueEnd = m_aNodeArray[m_aNodeArray.Count - 1].Pos.Y;
                    }
                }
            }
        }

        // 数值初值
        protected float m_fValueBgn = 0;
        // 数值末值
        protected float m_fValueEnd = 0;
        // 数值最大值
        protected float m_fValueMin = -10;
        // 数值最小值
        protected float m_fValueMax = 10;
        // 水平线长度
        protected float m_fHoriLength = 20;
        // 数值随机幅度
        protected float m_fValueRandomRange = 0;
        // 随机幅度只作用于第一个顶点
        protected bool m_bRandomOnHead = true;
        // 平直模式
        protected bool m_bStraightMode = false;
    };
}
