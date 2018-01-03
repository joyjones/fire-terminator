using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FireTerminator.Common.Structures;
using System.Drawing;
using System.Runtime.Serialization;
using FireTerminator.Common.Transitions;
using FireTerminator.Common.Elements;
using Microsoft.Xna.Framework;

namespace FireTerminator.Common.Services
{
    [ServiceContract(Namespace = "http://www.fireterminator.com/Fireterminator.Common.Services", CallbackContract = typeof(IServiceCallback))]
    public partial interface IMainService
    {
        // 心跳调用
        [OperationContract]
        bool BeatingHeart(int linkPort);
        // 登出
        [OperationContract(IsOneWay = true)]
        void Logout();
        // 发送聊天消息
        [OperationContract(IsOneWay = true)]
        void SendMessage(long targetId, Color color, string message);
        // 报告缺失资源文件
        [OperationContract(IsOneWay = true)]
        void ReportMissingResourceFiles(string[] files);
        // 报告成功下载一个资源文件
        [OperationContract(IsOneWay = true)]
        void ReportDownloadedOneResourceFile(string file);
        // 请求修改密码
        [OperationContract]
        bool ModifyPassword(string newPassword);
        // 请求切换训练任务
        [OperationContract(IsOneWay = true)]
        void RequestChangeTrainningTask(string taskName, string sceneName, int viewIndex);
        // 请求开始或停止训练任务
        [OperationContract(IsOneWay = true)]
        void RequestStartOrStopMission(bool start);
        // 请求监控目标分组或用户
        [OperationContract(IsOneWay = true)]
        void RequestMonitorGroupOrUser(long targetId, bool isGroup);

        // 同步元素创建信息
        [OperationContract(IsOneWay = true)]
        void SetElementOperation_Init(ElementCreateInfo[] infos);
        // 设置同步创建新元素
        [OperationContract(IsOneWay = true)]
        void SetElementOperation_Create(ElementCreateInfo info);
        // 设置同步删除元素
        [OperationContract(IsOneWay = true)]
        void SetElementOperation_Delete(Guid guid);
        // 设置同步变换元素
        [OperationContract(IsOneWay = true)]
        void SetElementOperation(Guid guid, ElementTransitionInfo info, SizeF scaleOnSrcBackImage);
        // 设置同步上浮或下沉元素
        [OperationContract(IsOneWay = true)]
        void SetElementDepthLevel(Guid guid, int depth);
        // 设置同步翻转元素
        [OperationContract(IsOneWay = true)]
        void SetElementFlipState(Guid guid, FlippingState state);
        // 设置同步遮罩元素信息
        [OperationContract(IsOneWay = true)]
        void SetToolElementInfo_Mask(Guid guid, Vector2[] vecs);
        // 设置同步水带元素信息
        [OperationContract(IsOneWay = true)]
        void SetToolElementInfo_Waterbag(Guid guid, float width, Vector2[] vecs);
        // 设置元素属性
        [OperationContract(IsOneWay = true)]
        void SetElementProperty(Guid guid, string propertyName, object newValue);
        // 远程移动元素
        [OperationContract(IsOneWay = true)]
        void SetElementTargetLocation(Guid guid, int x, int y);
        // 设置热键动画
        [OperationContract(IsOneWay = true)]
        void SetElementHotkeyAnimation(Guid guid, int keycode, float time);
    }

    public partial interface IServiceCallback
    {
        // 设置当前任务分组及用户
        [OperationContract(IsOneWay = true)]
        void SetCurrentTaskGroup(UserTaskGroup group);
        // 设置当前任务分组的项目文件及md5校验码
        [OperationContract(IsOneWay = true)]
        void SetTrainingProject(string projectFile, string fileMd5, bool coOperation);
        // 改变任务分组任务状态
        [OperationContract(IsOneWay = true)]
        void SetTaskGroupMission(string task, string scene, int viewportIdx);
        // 添加分组用户
        [OperationContract(IsOneWay = true)]
        void OnAppendedTaskGroupUser(LoginUserInfo user);
        // 移除分组用户
        [OperationContract(IsOneWay = true)]
        void OnRemovedTaskGroupUser(long userId);
        // 开始训练
        [OperationContract(IsOneWay = true)]
        void StartMission();
        // 暂停训练
        [OperationContract(IsOneWay = true)]
        void PauseMission(bool bPause);
        // 停止训练
        [OperationContract(IsOneWay = true)]
        void StopMission();
        // 系统或用户消息
        [OperationContract(IsOneWay = true)]
        void OnReceiveMessage(long fromUserId, MessageLevel lv, MessageType type, Color color, string message);
        // 改变用户状态
        [OperationContract(IsOneWay = true)]
        void OnUserStatusChanged(long userId, LoginStatus status);
        // 改变任务自由选择权限
        [OperationContract(IsOneWay = true)]
        void OnFreeTaskPermissionChanged(FreeTaskPermission permission);
        // 开始监控用户操作
        [OperationContract(IsOneWay = true)]
        void StartMonitor();
        // 终止监控用户操作
        [OperationContract(IsOneWay = true)]
        void TerminateMonitor();
        // 同步场景动画时间
        [OperationContract(IsOneWay = true)]
        void SyncPlayingViewportProgress(int playIndex, float time);
        // 合作组员创建元素
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_Create(long userId, ElementCreateInfo info);
        // 合作组员删除元素
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_Delete(Guid guid);
        // 合作组员修改元素
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_Modify(Guid guid, ElementTransitionInfo info, SizeF scale);
        // 合作组员浮动元素深度
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_Drift(Guid guid, int depth);
        // 合作组员操作遮罩元素
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_MaskTool(Guid guid, Vector2[] vecs);
        // 合作组员操作水带元素
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_WaterbagTool(Guid guid, float width, Vector2[] vecs);
        // 合作组员设置元素属性
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_ChangeProperty(Guid guid, string propertyName, object value);
        // 合作组员翻转元素
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_Flip(Guid guid, FlippingState state);
        // 合作组员远程移动元素
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_Drive(Guid guid, int x, int y);
        // 合作组员触发元素按键动画
        [OperationContract(IsOneWay = true)]
        void SetGroupUserOperation_HotkeyAnimation(Guid guid, float time);
        // 同步完整分组信息
        [OperationContract(IsOneWay = true)]
        void SyncUserTaskGroups(UserTaskGroup[] grps);
        // 回复监控请求
        [OperationContract(IsOneWay = true)]
        void ReplyMonitorTargetGroupOrUser(long targetId, bool isGroup, bool succeeded);
        // 绘制点评绘制样条
        [OperationContract(IsOneWay = true)]
        void DrawRemarkRibbon(float xr, float yr);
        // 结束创建点评绘制样条
        [OperationContract(IsOneWay = true)]
        void FinishRemarkRibbon(float lifeTime);
        // 同步评审配置信息
        [OperationContract(IsOneWay = true)]
        void SyncJudgementConfigContext(string context);
    }

    [DataContract]
    public class ElementCreateInfo
    {
        public ElementCreateInfo(ElementInfo e)
        {
            GUID = e.GUID;
            ResKind = e.Resource.Kind;
            ResPathFile = e.Resource.SubPathFileName;
            TransInfo = e.BaseTrans;
            ManualScaleOnSrcBackImage = e.ManualScaleOnSrcBackImage;
        }
        [DataMember]
        public Guid GUID;
        [DataMember]
        public ResourceKind ResKind;
        [DataMember]
        public string ResPathFile = "";
        [DataMember]
        public ElementTransitionInfo TransInfo = new ElementTransitionInfo();
        [DataMember]
        public SizeF ManualScaleOnSrcBackImage = new SizeF(1, 1);
    }
}
