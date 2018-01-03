using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FireTerminator.Common.Structures;

namespace FireTerminator.Server
{
    public partial class JudgementSetting : Form
    {
        public JudgementSetting()
        {
            InitializeComponent();
            m_ClassTable.Columns.Add(new DataColumn("类别编号", typeof(int)));
            m_ClassTable.Columns.Add(new DataColumn("类别名称", typeof(string)));
            m_ClassTable.Columns.Add(new DataColumn("类别分值", typeof(int)));
        }
        private DataTable m_ClassTable = new DataTable();
        private JudgementSettings JudgeSetBackup = new JudgementSettings();
        public DataTable Table
        {
            get { return gdcItems.DataSource as DataTable; }
        }
        private void BuildClassTable()
        {
            m_ClassTable.Rows.Clear();
            var classes = Program.Option.JudgementSet.JudgeClasses;
            JudgeSetBackup.ApplyChanges(classes);
            gdcItems.DataSource = JudgeSetBackup.BuildTable(null);
            foreach (var cls in classes)
            {
                var row = m_ClassTable.NewRow();
                row[0] = cls.ID;
                row[1] = cls.Name;
                row[2] = cls.Score;
                m_ClassTable.Rows.Add(row);
            }
            gdcClasses.DataSource = m_ClassTable;
        }
        private void ApplyClassChanges()
        {
            List<int> cids = new List<int>();
            foreach (DataRow row in m_ClassTable.Rows)
            {
                int cid = (int)row[0];
                cids.Add(cid);
                JudgeClass jc = JudgeSetBackup.GetJudgeClass(cid);
                if (jc == null)
                    jc = JudgeSetBackup.AddJudgeClass(cid, (string)row[1], (int)row[2]);
                else
                {
                    jc.Name = (string)row[1];
                    jc.Score = (int)row[2];
                }
                jc.Items.Clear();
                foreach (DataRow irow in Table.Rows)
                {
                    if (cid == (int)irow[0])
                    {
                        irow[1] = jc.Name;
                        irow[2] = jc.Score;
                        jc.Items.Add(new JudgeItem(jc, (int)irow[3], (string)irow[4], (float)irow[5], (string)irow[6]));
                    }
                }
            }
            List<int> wasteCIds = new List<int>();
            foreach (var cls in JudgeSetBackup.JudgeClasses)
            {
                if (!cids.Contains(cls.ID))
                    wasteCIds.Add(cls.ID);
            }
            foreach (int cid in wasteCIds)
            {
                JudgeSetBackup.RemoveJudgeClass(cid);
            }
            gdcItems.DataSource = JudgeSetBackup.BuildTable(null);
        }
        private void ApplyChanges()
        {
            ApplyClassChanges();
            JudgeSetBackup.ApplyItemsFromTable(Table);
            Program.Option.JudgementSet.ApplyChanges(JudgeSetBackup.JudgeClasses);
        }
        private void JudgementSetting_Load(object sender, EventArgs e)
        {
            BuildClassTable();
            gdcItems.DataSource = JudgeSetBackup.BuildTable(null);
        }

        private void bnAddClass_Click(object sender, EventArgs e)
        {
            var row = m_ClassTable.NewRow();
            int id = JudgeSetBackup.NewClassID;
            row[0] = id;
            row[1] = "新类别" + id;
            row[2] = 0;
            m_ClassTable.Rows.Add(row);
            ApplyClassChanges();
            gdcClasses.RefreshDataSource();
        }

        private void bnDelClass_Click(object sender, EventArgs e)
        {
            if (m_ClassTable.Rows.Count == 1)
            {
                MessageBox.Show("不允许将类别删空。");
                return;
            }
            else
            {
                int row = gridView_Classes.FocusedRowHandle;
                m_ClassTable.Rows.RemoveAt(row);
                ApplyClassChanges();
                gdcClasses.RefreshDataSource();
                gdcItems.RefreshDataSource();
            }
        }

        private void bnAddItem_Click(object sender, EventArgs e)
        {
            int crow = gridView_Classes.FocusedRowHandle;
            if (crow < 0)
            {
                MessageBox.Show("请先选中一个类别");
                return;
            }
            DataRow row = m_ClassTable.Rows[crow];
            int cid = (int)row[0];
            var cls = JudgeSetBackup.GetJudgeClass(cid);
            int index = Table.Rows.Count;
            for (int i = 0; i < Table.Rows.Count; ++i)
            {
                int _cid = (int)Table.Rows[i][0];
                if (cid == _cid)
                    index = i;
            }
            row = Table.NewRow();
            row[0] = cid;
            row[1] = cls.Name;
            row[2] = cls.Score;
            row[3] = index + 1;
            row[4] = "未命名项目";
            row[5] = 0.0F;
            row[6] = "";
            Table.Rows.InsertAt(row, index);
            for (int i = 0; i < Table.Rows.Count; ++i)
            {
                Table.Rows[i][3] = i + 1;
            }
            JudgeSetBackup.ApplyItemsFromTable(Table);
            //gdcItems.RefreshDataSource();
            gridView_Items.FocusedRowHandle = index;
        }

        private void bnDelItem_Click(object sender, EventArgs e)
        {
            int row = gridView_Items.FocusedRowHandle;
            if (row >= 0)
            {
                Table.Rows.RemoveAt(row);
                JudgeSetBackup.ApplyItemsFromTable(Table);
                gdcItems.RefreshDataSource();
            }
        }

        private void bnOK_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            Program.Option.JudgementSet.Save();
            this.DialogResult = DialogResult.OK;
        }
    }
}
