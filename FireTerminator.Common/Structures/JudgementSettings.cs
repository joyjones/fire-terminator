using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Data;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace FireTerminator.Common.Structures
{
    public class JudgeClass
    {
        public JudgeClass(int id, string name, int score)
        {
            ID = id;
            Name = name;
            Score = score;
        }
        public JudgeClass(JudgeClass cls)
        {
            ID = cls.ID;
            Name = cls.Name;
            Score = cls.Score;
            foreach (var i in cls.Items)
            {
                Items.Add(new JudgeItem(this, i));
            }
        }
        public JudgeItem GetItem(int id)
        {
            foreach (var item in Items)
            {
                if (item.ID == id)
                    return item;
            }
            return null;
        }
        public int ID = 0;
        public string Name = "";
        public int Score = 0;
        public List<JudgeItem> Items = new List<JudgeItem>();
    }
    public class JudgeDeduction
    {
        public JudgeDeduction(string desc, float score)
        {
            Description = desc;
            DuductScore = score;
        }
        public override string ToString()
        {
            return String.Format("{0}：扣{1}分", Description, DuductScore);
        }
        public string Description = "";
        public float DuductScore = 0;
    }
    public class JudgeItem
    {
        public JudgeItem(JudgeClass parent, int id, string name, float score)
        {
            ParentClass = parent;
            ID = id;
            Name = name;
            Score = score;
        }
        public JudgeItem(JudgeClass parent, int id, string name, float score, string desc)
            : this(parent, id, name, score)
        {
            DeductionText = desc;
        }
        public JudgeItem(JudgeClass parent, JudgeItem item)
            : this(parent, item.ID, item.Name, item.Score, item.DeductionText)
        {
        }
        public JudgeClass ParentClass = null;
        public int ID = 0;
        public string Name = "";
        public float Score = 0;
        public List<JudgeDeduction> Deductions = new List<JudgeDeduction>();
        public string DeductionText
        {
            get
            {
                string text = "";
                foreach (var d in Deductions)
                    text += String.Format("{0}；\r\n", d.ToString());
                return text.TrimEnd('\r', '\n');
            }
            set
            {
                Deductions.Clear();
                foreach (var sect in value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var strs = sect.TrimEnd('；').Split('：');
                    var desc = strs[0];
                    float score = 0;
                    if (strs.Length > 1)
                    {
                        var mat = Regex.Match(strs[1], @"扣(?'S'-?\d+(\.\d+)?)分");
                        if (mat.Success)
                            float.TryParse(mat.Groups["S"].Value, out score);
                    }
                    Deductions.Add(new JudgeDeduction(desc, score));
                }
            }
        }
    }
    public class JudgementSettings
    {
        public JudgementSettings()
        {
        }
        private Dictionary<int, JudgeClass> m_Classes = new Dictionary<int, JudgeClass>();
        public static string ConfigFile
        {
            get { return System.Windows.Forms.Application.StartupPath + "\\FireTerminator.judgement.xml"; }
        }
        public static string ConfigFileMD5
        {
            get { return CommonMethods.GetFileMD5(ConfigFile); }
        }
        public static string ConfigFileContext
        {
            get
            {
                if (File.Exists(ConfigFile))
                {
                    using (var sr = new StreamReader(ConfigFile, Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
                return "";
            }
        }

        public JudgeClass[] JudgeClasses
        {
            get { return m_Classes.Values.ToArray(); }
        }
        public int NewClassID
        {
            get
            {
                int id = 1;
                while (m_Classes.ContainsKey(id))
                    ++id;
                return id;
            }
        }
        public int NewItemID
        {
            get { return MaxItemID + 1; }
        }
        public int MaxItemID
        {
            get
            {
                int id = 0;
                foreach (var cls in m_Classes.Values)
                {
                    foreach (var itm in cls.Items)
                    {
                        if (itm.ID > id)
                            id = itm.ID;
                    }
                }
                return id;
            }
        }
        public JudgeClass GetJudgeClass(int id)
        {
            JudgeClass result = null;
            m_Classes.TryGetValue(id, out result);
            return result;
        }
        public JudgeClass AddJudgeClass(int id, string name, int score)
        {
            if (GetJudgeClass(id) != null)
                return null;
            var cls = new JudgeClass(id, name, score);
            m_Classes[id] = cls;
            return cls;
        }
        public bool RemoveJudgeClass(int id)
        {
            var cls = GetJudgeClass(id);
            if (cls == null || m_Classes.Count == 1)
                return false;
            var wasteItems = new List<JudgeItem>(cls.Items.ToArray());
            m_Classes.Remove(cls.ID);
            cls = m_Classes.ElementAt(0).Value;
            foreach (var item in wasteItems)
            {
                item.ParentClass = cls;
                cls.Items.Add(item);
            }
            int iid = 0;
            foreach (var c in m_Classes.Values)
            {
                foreach (var i in c.Items)
                    i.ID = ++iid;
            }
            return true;
        }
        public void ApplyChanges(JudgeClass[] classes)
        {
            m_Classes.Clear();
            foreach (var cls in classes)
                m_Classes[cls.ID] = new JudgeClass(cls);
        }
        public void ApplyItemsFromTable(DataTable tb)
        {
            foreach (var cls in m_Classes.Values)
            {
                cls.Items.Clear();
            }
            int iIndex = 0;
            foreach (DataRow row in tb.Rows)
            {
                int cId = (int)row[0];
                JudgeClass jc = GetJudgeClass(cId);
                if (jc != null)
                {
                    int iId = ++iIndex;
                    row[3] = iId;
                    string iName = (string)row[4];
                    float iScore = (float)row[5];

                    var item = new JudgeItem(jc, iId, iName, iScore);
                    item.DeductionText = (string)row[6];
                    jc.Items.Add(item);
                }
            }
        }
        public DataTable BuildTable(float[] scores)
        {
            var tb = new DataTable();
            tb.Columns.Clear();
            tb.Columns.Add(new DataColumn("类别编号", typeof(int)));
            tb.Columns.Add(new DataColumn("类别名称", typeof(string)));
            tb.Columns.Add(new DataColumn("类别分值", typeof(int)));
            tb.Columns.Add(new DataColumn("项目序号", typeof(int)));
            tb.Columns.Add(new DataColumn("项目名称", typeof(string)));
            tb.Columns.Add(new DataColumn("分数", typeof(float)));
            tb.Columns.Add(new DataColumn("评判说明", typeof(string)));
            tb.Columns.Add(new DataColumn("得分", typeof(float)));

            foreach (var cls in m_Classes.Values)
            {
                foreach (var itm in cls.Items)
                {
                    var row = tb.NewRow();
                    row[0] = cls.ID;
                    row[1] = cls.Name;
                    row[2] = cls.Score;
                    row[3] = itm.ID;
                    row[4] = itm.Name;
                    row[5] = itm.Score;
                    row[6] = itm.DeductionText;
                    if (scores == null || scores.Length < itm.ID)
                        row[7] = 0;
                    else
                        row[7] = scores[itm.ID - 1];
                    tb.Rows.Add(row);
                }
            }
            return tb;
        }
        public void Load()
        {
            m_Classes.Clear();
            if (!System.IO.File.Exists(ConfigFile))
                return;
            XmlDocument xml = new XmlDocument();
            xml.Load(ConfigFile);
            foreach (XmlElement cnode in xml.DocumentElement.GetElementsByTagName("Class"))
            {
                int cId = Convert.ToInt32(cnode.GetAttribute("ID"));
                string cName = cnode.GetAttribute("Name");
                int cScore = Convert.ToInt32(cnode.GetAttribute("Score"));
                JudgeClass jc = new JudgeClass(cId, cName, cScore);
                foreach (XmlElement inode in cnode.GetElementsByTagName("Item"))
                {
                    int iId = Convert.ToInt32(inode.GetAttribute("ID"));
                    string iName = inode.GetAttribute("Name");
                    int iScore = Convert.ToInt32(inode.GetAttribute("Score"));
                    JudgeItem ji = new JudgeItem(jc, iId, iName, iScore);
                    foreach (XmlElement dnode in inode.GetElementsByTagName("Deduct"))
                    {
                        string dDesc = dnode.GetAttribute("Desc");
                        float dScore = Convert.ToSingle(dnode.GetAttribute("Score"));
                        var jd = new JudgeDeduction(dDesc, dScore);
                        ji.Deductions.Add(jd);
                    }
                    jc.Items.Add(ji);
                }
                m_Classes[jc.ID] = jc;
            }
        }
        public void Save()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<FireTerminatorJudgement></FireTerminatorJudgement>");
            foreach (var cls in m_Classes.Values)
            {
                XmlElement cnode = xml.CreateElement("Class");
                cnode.SetAttribute("ID", cls.ID.ToString());
                cnode.SetAttribute("Name", cls.Name);
                cnode.SetAttribute("Score", cls.Score.ToString());
                foreach (var itm in cls.Items)
                {
                    XmlElement inode = xml.CreateElement("Item");
                    inode.SetAttribute("ID", itm.ID.ToString());
                    inode.SetAttribute("Name", itm.Name);
                    inode.SetAttribute("Score", itm.Score.ToString());
                    foreach (var duc in itm.Deductions)
                    {
                        XmlElement dnode = xml.CreateElement("Deduct");
                        dnode.SetAttribute("Desc", duc.Description);
                        dnode.SetAttribute("Score", duc.DuductScore.ToString());
                        inode.AppendChild(dnode);
                    }
                    cnode.AppendChild(inode);
                }
                xml.DocumentElement.AppendChild(cnode);
            }
            xml.Save(ConfigFile);
        }
    }
}
