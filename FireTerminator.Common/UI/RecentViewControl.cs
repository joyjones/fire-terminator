using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Ribbon;
using System.IO;

namespace FireTerminator.Common.UI
{
    public partial class RecentViewControl : RibbonApplicationUserControl
    {
        public RecentViewControl()
        {
            InitializeComponent();
            mruFileList = new MRUArrayList(splitContainer1.Panel1, imageCollection3.Images[0], imageCollection3.Images[1], imageCollection1.Images[0], false, true);
            mruFileList.LabelClicked += new EventHandler(mruFileList_LabelClicked);
            mruFolderList = new MRUArrayList(panel2, imageCollection3.Images[0], imageCollection3.Images[1], imageCollection1.Images[1], false, true);
            mruFolderList.LabelClicked += new EventHandler(mruFolderList_LabelClicked);
        }

        void mruFolderList_LabelClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", (string)sender);
            BackstageView.Ribbon.HideApplicationButtonContentControl();
        }

        void mruFileList_LabelClicked(object sender, EventArgs e)
        {
            //MainForm frm = (MainForm)BackstageView.Ribbon.FindForm();
            //frm.OpenFile((string)sender);
            BackstageView.Ribbon.HideApplicationButtonContentControl();
        }
        MRUArrayList mruFileList;
        MRUArrayList mruFolderList;
        public MRUArrayList MRUFileList { get { return mruFileList; } }
        public MRUArrayList MRUFolderList { get { return mruFolderList; } }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            spinEdit1.Enabled = checkEdit1.Checked;
            UpdateRecentItems();
        }
        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            UpdateRecentItems();
        }
        void ClearRecentItems()
        {
            if (BackstageView == null)
                return;
            for (int i = 0; i < BackstageView.Items.Count; )
            {
                BackstageViewButtonItem item = BackstageView.Items[i] as BackstageViewButtonItem;
                if ((item != null && item.Tag != null) || BackstageView.Items[i] is BackstageViewItemSeparator)
                {
                    if (item != null)
                        item.ItemClick -= new BackstageViewItemEventHandler(OnRecentItemClick);
                    BackstageView.Items.RemoveAt(i);
                }
                else
                    i++;
            }
        }
        void UpdateRecentItems()
        {
            BackstageView.BeginUpdate();
            try
            {
                ClearRecentItems();
                if (checkEdit1.Checked)
                    InitRecentItems();
            }
            finally
            {
                BackstageView.EndUpdate();
            }
        }
        void InitRecentItems()
        {
            BackstageView.Items.Insert(4, new BackstageViewItemSeparator());
            int itemCount = Math.Min(MRUFileList.Count, (int)spinEdit1.Value);
            for (int i = 0; i < itemCount; i++)
            {
                BackstageViewButtonItem item = new BackstageViewButtonItem();
                item.Caption = Path.GetFileName((string)MRUFileList[i]);
                item.Glyph = imageCollection3.Images[2];
                item.Tag = (string)MRUFileList[i];
                item.ItemClick += new BackstageViewItemEventHandler(OnRecentItemClick);
                BackstageView.Items.Insert(5 + i, item);
            }
        }
        void OnRecentItemClick(object sender, BackstageViewItemEventArgs e)
        {
            //MainForm frm = (MainForm)BackstageView.Ribbon.FindForm();
            //frm.OpenFile((string)e.Item.Tag);
            BackstageView.Ribbon.HideApplicationButtonContentControl();
        }
    }
}
