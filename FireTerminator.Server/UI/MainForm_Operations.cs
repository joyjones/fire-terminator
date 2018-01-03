using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FireTerminator.Common.Operations;
using FireTerminator.Common;
using FireTerminator.Common.Structures;
using FireTerminator.Server.Services;
using FireTerminator.Common.Services;
using FireTerminator.Common.Elements;

namespace FireTerminator.Server
{
    public partial class MainForm
    {
        private void InitEditViewOperator()
        {
            m_EditViewOperater = new ServerEditViewOperater(this, pnlMain, pnlPreview);
            m_EditViewOperater.ChkBtn_AnimElement = icAnimElement;
            m_EditViewOperater.ChkBtn_MoveElement = icMoveElement;
            m_EditViewOperater.ChkBtn_RotateElement = icRotateElement;
            m_EditViewOperater.ChkBtn_ScaleElement = icScaleElement;
            m_EditViewOperater.ChkBtn_DriveElement = icTargetMove;
            m_EditViewOperater.ChkBtn_ToolElement_Mask = icToolMask;
            m_EditViewOperater.ChkBtn_ToolElement_Waterbag = icToolWaterbag;
            m_EditViewOperater.ChkBtn_ToolElement_TipText = icToolTipText;
            m_EditViewOperater.Btn_CopyElement = iCopyElement;
            m_EditViewOperater.Btn_CutElement = iCutElement;
            m_EditViewOperater.Btn_PasteElement = iPasteElement;
            m_EditViewOperater.ProjectTree = trvProject;
            m_EditViewOperater.ResourceTree = trvResources;
            m_EditViewOperater.TransCtrl = null;
            m_EditViewOperater.PropertyBar = grdProperties;
            m_EditViewOperater.Cursor_ToolTip = toolTipOnCursor;
            m_EditViewOperater.IconImageIndices[ResourceKind.图像] = new int[] { 26, 30 };
            m_EditViewOperater.IconImageIndices[ResourceKind.背景] = new int[] { 61, 59 };
            m_EditViewOperater.IconImageIndices[ResourceKind.效果] = new int[] { 33, 37 };
            m_EditViewOperater.IconImageIndices[ResourceKind.视频] = new int[] { 62, 63 };
            m_EditViewOperater.IconImageIndices[ResourceKind.音频] = new int[] { 64, 65 };
            m_EditViewOperater.IconImageIndices[ResourceKind.遮罩] = new int[] { 34, 6 };
            m_EditViewOperater.IconImageIndices[ResourceKind.水带] = new int[] { 14, 6 };
            m_EditViewOperater.IconImageIndices[ResourceKind.文本] = new int[] { 54, 6 };
            m_EditViewOperater.ExtraIconImageIndices[0] = 7;
            m_EditViewOperater.ExtraIconImageIndices[1] = 58;
            m_EditViewOperater.ExtraIconImageIndices[2] = 59;
            m_EditViewOperater.ExtraIconImageIndices[3] = 60;
        }

        private void OperattionHistory_NewOperationPushed(Operation opt)
        {
            var eo = opt as Operation_Element;
            var pr = CurFocusTreeNodeProjReferrencer;
            UserTaskGroup grp = SelectedTaskGroup;
            LoginUserInfo user = SelectedUserInfo;
            if (grp == null && user != null)
                grp = user.ParentGroup;
            ServerLoginUserInfo suser = null;
            if (user != null)
                suser = user.LocalObj as ServerLoginUserInfo;
            ProjectReferrencer prTarget = pr;
            if (grp.IsCooperation)
                prTarget = grp;
            else if (suser == null)
                return;
            if (opt is Operation_Element)
            {
                var eop = opt as Operation_Element;
                if (!eop.DonotMakeUserDirty)
                    eop.Element.CreatorId = LoginUserInfo.SystemUserID;
                if (prTarget != null && prTarget.IsStarted)
                {
                    if (eop is Operation_Element_Create)
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_Create(prTarget, eop.Element));
                    else if (opt is Operation_Element_Delete)
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_Delete(prTarget, eop.Element.GUID));
                    else if (opt is Operation_Element_Drift)
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_Drift(prTarget, eop.Element));
                    else if (opt is Operation_Element_Flip)
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_Flip(prTarget, eop.Element));
                    else if (opt is Operation_Element_ChangeProperty)
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_ChangeProperty(prTarget, eop.Element, 
                            ((Operation_Element_ChangeProperty)opt).PropertyName,
                            ((Operation_Element_ChangeProperty)opt).ValueNew));
                    else if (opt is Operation_Element_ChangeMaskInfo)
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_SetMask(prTarget, eop.Element as ElementInfo_Mask));
                    else if (opt is Operation_Element_ChangeWaterbagInfo)
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_SetWaterbag(prTarget, eop.Element as ElementInfo_Waterbag));
                    else if (opt is Operation_Element_SetHotkeyAnimation)
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_HotkeyAnimation(prTarget, eop.Element));
                    //else if (opt is Operation_Element_ChangeLocation)
                    //    Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_Drive(prTarget, eop.Element,
                    //        ((Operation_Element_ChangeLocation)opt).LocationNew.X,
                    //        ((Operation_Element_ChangeLocation)opt).LocationNew.Y));
                    else
                        Program.UserMgr.PushNotification(new UserNotification_SetGroupSystemOperation_Modify(prTarget, eop.Element));
                }
            }
        }
    }
}
