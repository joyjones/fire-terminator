using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FireTerminator.Common.UI
{
    public partial class MessageControl : UserControl
    {
        public MessageControl()
        {
            InitializeComponent();
            DefaultMessageType = MessageType.用户消息;
        }

        public Color SelectedColor
        {
            get { return cmbMsgColor.Color; }
        }

        public MessageType DefaultMessageType
        {
            get;
            set;
        }

        public void ClearStudentList()
        {
            cmbStudentsList.Properties.Items.Clear();
            cmbStudentsList.Properties.Items.Add("全部学员");
            cmbStudentsList.SelectedIndex = 0;
        }

        public void AddStudent(long id, string name)
        {
            m_Students[id] = name;
            string item = String.Format("{0}({1})", name, id);
            cmbStudentsList.Properties.Items.Add(item);
        }

        public void RemoveStudent(long id)
        {
            if (m_Students.ContainsKey(id))
            {
                string item = String.Format("{0}({1})", m_Students[id], id);
                cmbStudentsList.Properties.Items.Remove(item);
                m_Students.Remove(id);
            }
        }

        private Dictionary<long, string> m_Students = new Dictionary<long, string>();
        private long m_SelectedStudentID = 0;

        private void cmbStudentsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string item = cmbStudentsList.SelectedItem as string;
            var mat = Regex.Match(item, @".+\((?'ID'\d+)\)");
            if (mat.Success)
                m_SelectedStudentID = long.Parse(mat.Groups["ID"].Value);
            else
                m_SelectedStudentID = 0;
        }

        private void txbMessage_EditValueChanged(object sender, EventArgs e)
        {

        }

        public void PushErrorMessage(MessageType type, string msg)
        {
            PushMessage(MessageLevel.错误, type, msg);
        }

        public void PushMessage(string msg)
        {
            PushMessage(MessageLevel.一般, MessageType.用户消息, ProjectDoc.Instance.Option.GetMessageColor(MessageLevel.一般), msg);
        }

        public void PushMessage(Color clr, string msg)
        {
            PushMessage(MessageLevel.一般, MessageType.用户消息, clr, msg);
        }

        public void PushMessage(MessageLevel lv, MessageType type, string msg)
        {
            PushMessage(lv, type, ProjectDoc.Instance.Option.GetMessageColor(lv), msg);
        }

        public void PushMessage(MessageLevel lv, MessageType type, Color clr, string msg)
        {
            this.InvokeEx(() =>
            {
                if (type == MessageType.服务端消息)
                    msg = "[系统]" + msg;
                else if (type != MessageType.用户消息)
                    msg = String.Format("[{0}]", type.ToString()) + msg;
                int start = rtxMessages.Text.Length;
                rtxMessages.AppendText(msg + "\r\n");
                rtxMessages.Select(start, msg.Length);
                if (clr == Color.Empty)
                    clr = ProjectDoc.Instance.Option.GetMessageColor(lv);
                rtxMessages.SelectionColor = clr;
                rtxMessages.SelectionLength = 0;
                rtxMessages.SelectionStart = rtxMessages.Text.Length;
                rtxMessages.ScrollToCaret();
            });
        }

        private void cmbMsgColor_EditValueChanged(object sender, EventArgs e)
        {
            ProjectDoc.Instance.Option.MessageTextColor = cmbMsgColor.Color;
        }

        private void bnSendMessage_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txbMessage.Text))
                return;
            string who;
            if (DefaultMessageType == MessageType.用户消息)
                who = "我: ";
            else
                who = "[系统]";
            PushMessage(who + txbMessage.Text);
            if (SendMessage != null)
                SendMessage(m_SelectedStudentID, SelectedColor, txbMessage.Text);
            txbMessage.Text = "";
        }
        public delegate void Delegate_SendMessage(long targetId, Color clr, string message);
        public event Delegate_SendMessage SendMessage;

        private void txbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                bnSendMessage_Click(null, null);
        }
    }
}
