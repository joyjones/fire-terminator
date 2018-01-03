using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using DevExpress.XtraTreeList.Nodes;

namespace FireTerminator.Common
{
    public enum ResourceKind
    {
        背景,
        图像,
        效果,
        视频,
        音频,
        遮罩,
        水带,
        文本,
    }

    public enum BodyOperation
    {
        None,
        Move,
        Rotate,
        Scale,
        PanView,
        ZoomView,
    }

    public enum BodyOperationPart
    {
        Nothing,
        Body,
        BorderL,
        BorderR,
        BorderU,
        BorderD,
        CornerLU,
        CornerRU,
        CornerLD,
        CornerRD,
        AddJoint,
        DelJoint,
    }

    public enum AnimBurstType
    {
        时间同步,
        热键触发,
    }

    public enum TransitionKind
    {
        Unknown,
        隐藏,
        位移,
        缩放,
        旋转,
        半透
    }
    
    [Flags]
    public enum FlippingState
    {
        正常 = 0,
        水平翻转 = 1,
        竖直翻转 = 2,
        水平和竖直翻转 = 3,
    }

    public enum LoginResult
    {
        登录成功 = 0,
        登录并注册成功,
        失败_帐号非法,
        失败_密码错误,
        失败_帐号不存在,
        失败_连接超时,
        失败_评审权限未开通,
        失败_未知错误,
    }

    public enum LoginStatus
    {
        离线,
        登录中,
        在线,
    }

    public enum LogoutReason
    {
        主动退出,
        超时踢出,
        更改用户,
        系统踢出,
        服务退出,
        异地登录,
    }

    public enum MessageLevel
    {
        一般,
        提示,
        警告,
        错误,
    }

    public enum MessageType
    {
        用户消息,
        服务端消息,
        网络服务,
        登录连接,
        任务操作,
        文件传输,
    }

    public enum TaskGroupMissionStatus
    {
        初始化,
        等待指定监控用户,
        等待指定训练项目,
        请求同步资源文件,
        等待训练开始,
        训练进行中,
        训练暂停,
    }
    
    [Flags]
    public enum FreeTaskPermission
    {
        禁止自由选择 = 0,
        自由选择窗口 = 1,
        自由选择场景 = 3,
        自由选择任务 = 7,
    }

    public enum PortID
    {
        LoginLink = 8001, // 登录连接
        MainService = 8002,
        DownloadTransmissionDetection = 8010, // 文件下载检测
    }

    public enum ModifyStatus
    {
        Unchanged,
        Appended,
        Modified,
        Deleted,
        DeletedInherit,
    }
}
