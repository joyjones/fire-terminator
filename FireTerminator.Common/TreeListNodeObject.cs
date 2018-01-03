using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraTreeList.Nodes;

namespace FireTerminator.Common
{
    public class TreeListNodeObject
    {
        public TreeListNodeObject()
        {
        }
        public TreeListNodeObject(TreeListNodeObject obj)
        {
            TreeNode = obj.TreeNode;
            m_Name = obj.Name;
            m_IsSelected = obj.IsSelected;
        }
        [Browsable(false)]
        public TreeListNode TreeNode
        {
            get;
            set;
        }
        private string m_Name = "";
        public virtual string Name
        {
            get { return m_Name; }
            set
            {
                if (!String.IsNullOrEmpty(value) && m_Name != value)
                {
                    m_Name = value;
                    if (TreeNode != null)
                        TreeNode.SetValue(0, m_Name);
                }
            }
        }
        private bool m_IsSelected = false;
        [Browsable(false)]
        public virtual bool IsSelected
        {
            get
            {
                if (TreeNode != null)
                    return TreeNode.Selected;
                return m_IsSelected;
            }
            set
            {
                m_IsSelected = value;
                if (m_IsSelected && TreeNode != null && !TreeNode.Selected)
                {
                    try
                    {
                        var form = TreeNode.TreeList.Parent.FindForm();
                        form.Invoke(new Action(() =>
                        {
                            TreeNode.Selected = true;
                        }));
                        //System.Diagnostics.Debug.WriteLine("Selected Node: " + (Name.Length > 0 ? Name : this.ToString()));
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
