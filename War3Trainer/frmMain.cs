using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace War3Trainer
{
    public partial class frmMain : Form
    {
        clsGameContext CurrentGameContext = null;
        clsGameTrainer MainTrainer = null;

        public frmMain()
        {
            InitializeComponent();

            WindowsApi.ProcessToken.SetPrivilege();
            FindGame();
            txtIntroduction.Select(0, 0);     // Cancle select all
        }

        /************************************************************************/
        /* Main functions                                                       */
        /************************************************************************/
        private void FindGame()
        {
            bool IsRecognized = false;
            try
            {
                CurrentGameContext = clsGameContext.FindGameRunning("war3");
                if (CurrentGameContext != null)
                {
                    // Game online
                    labGameScanState.Text = "检测到游戏（"
                        + CurrentGameContext.ProcessID.ToString()
                        + "），游戏版本 "
                        + CurrentGameContext.ProcessVersion;

                    // Get a new trainer
                    GetAllObject();

                    IsRecognized = true;
                }
                else
                {
                    // Game offline
                    labGameScanState.Text = "游戏未运行，运行游戏后单击“查找游戏”";
                }
                
            }
            catch (UnkonwnGameVersionExpection e)
            {
                // Unknown game version
                CurrentGameContext = null;
                labGameScanState.Text = "检测到游戏（"
                        + e.ProcessId.ToString()
                        + "），但版本（"
                        + e.FileVersion
                        + "）不被支持";
            }
            catch
            {
                // Why here?
                CurrentGameContext = null;
                labGameScanState.Text = "检测游戏版本时发生严重错误，请重试上一次的操作";
            }

            // Enable buttons
            if (IsRecognized)
            {
                viewFunctions.Enabled = true;
                viewData.Enabled = true;
                cmdGetAllObjects.Enabled = true;
                cmdModify.Enabled = true;
            }
            else
            {
                viewFunctions.Enabled = false;
                viewData.Enabled = false;
                cmdGetAllObjects.Enabled = false;
                cmdModify.Enabled = false;
            }
        }

        private void GetAllObject()
        {
            // Get a new trainer
            if (CurrentGameContext == null)
                return;
            MainTrainer = new clsGameTrainer(CurrentGameContext);

            // Create function tree
            viewFunctions.Nodes.Clear();
            foreach (ITrainerNode CurrentFunction in MainTrainer.GetFunctionList())
            {
                TreeNode[] ParentNodes = viewFunctions.Nodes.Find(CurrentFunction.ParentIndex.ToString(), true);
                TreeNodeCollection ParentTree;
                if (ParentNodes.Length < 1)
                    ParentTree = viewFunctions.Nodes;
                else
                    ParentTree = ParentNodes[0].Nodes;
                
                ParentTree.Add(
                    CurrentFunction.NodeIndex.ToString(),
                    CurrentFunction.NodeTypeName)
                    .Tag = CurrentFunction;
            }
            viewFunctions.ExpandAll();

            // Switch to page 1
            TreeNode[] IntroductionNodes = viewFunctions.Nodes.Find("1", true);
            if (IntroductionNodes.Length > 0)
            {
                viewFunctions.SelectedNode = IntroductionNodes[0];
                SelectAFunction(IntroductionNodes[0]);
            }
        }

        // Re-query specific tree-node by FunctionListNode
        private void RefreshSelectedObject(ITrainerNode CurrentFunction)
        {
            TreeNode[] CurrentNodes = viewFunctions.Nodes.Find(CurrentFunction.NodeIndex.ToString(), true);
            TreeNode CurrentTree;
            if (CurrentNodes.Length < 1)
                return;
            else
                CurrentTree = CurrentNodes[0];

            CurrentTree.Text = CurrentFunction.NodeTypeName;
        }

        private void SelectAFunction(TreeNode FunctionNode)
        {
            ITrainerNode Node = FunctionNode.Tag as ITrainerNode;
            if (Node == null)
                return;
            
            // Show introduction page
            if (Node.NodeType == TrainerNodeType.Introduction)
            {
                viewData.Visible = false;
                txtIntroduction.Visible = true;
            }
            else
            {
                // Fill address list
                FillAddressList(Node.NodeIndex);
                
                // Show address list
                txtIntroduction.Visible = false;

                if (viewData.Items.Count > 0)
                    viewData.Visible = true;
                else
                    viewData.Visible = false;
            }            
        }

        private void FillAddressList(int FunctionNodeId)
        {
            // To set the right window
            var AddressListInThisTree =
                from AddressLine in MainTrainer.GetAddressList()
                where AddressLine.ParentIndex == FunctionNodeId
                select AddressLine;

            viewData.Items.Clear();
            foreach (IAddressNode AddressLine in AddressListInThisTree)
            {
                viewData.Items.Add(new ListViewItem(
                    new string[]
                    {
                        AddressLine.Caption,    // Caption
                        "",                     // Original value
                        ""                      // Modified value
                    }));
                viewData.Items[viewData.Items.Count - 1].Tag = AddressLine;
            }

            // To get memory content
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(CurrentGameContext.ProcessID))
            {
                foreach (ListViewItem CurrentItem in viewData.Items)
                {
                    IAddressNode AddressLine = CurrentItem.Tag as IAddressNode;
                    if (AddressLine == null)
                        continue;

                    Object ItemValue;
                    switch (AddressLine.ValueType)
                    {
                        case AddressListValueType.Integer:
                            ItemValue = Mem.ReadInt32((IntPtr)AddressLine.Address)
                                / AddressLine.ValueScale;
                            break;
                        case AddressListValueType.Float:
                            ItemValue = Mem.ReadFloat((IntPtr)AddressLine.Address)
                                / AddressLine.ValueScale;
                            break;
                        case AddressListValueType.Char4:
                            ItemValue = Mem.ReadChar4((IntPtr)AddressLine.Address);
                            break;
                        default:
                            ItemValue = "";
                            break;
                    }
                    CurrentItem.SubItems[1].Text = ItemValue.ToString();
                }
            }
        }

        // To apply the modifications
        private void ApplyModify()
        {
            using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(CurrentGameContext.ProcessID))
            {
                foreach (ListViewItem CurrentItem in viewData.Items)
                {
                    string ItemValueString = CurrentItem.SubItems[2].Text;
                    if (ItemValueString.Trim() == "")
                    {
                        // Not modified
                        continue;
                    }

                    IAddressNode AddressLine = CurrentItem.Tag as IAddressNode;
                    if (AddressLine == null)
                        continue;

                    switch (AddressLine.ValueType)
                    {
                        case AddressListValueType.Integer:
                            Int32 IntValue;
                            Int32.TryParse(ItemValueString, out IntValue);
                            IntValue = (Int32)(unchecked(IntValue * AddressLine.ValueScale));
                            Mem.WriteInt32((IntPtr)AddressLine.Address, IntValue);
                            break;
                        case AddressListValueType.Float:
                            float FloatValue;
                            float.TryParse(ItemValueString, out FloatValue);
                            FloatValue = unchecked(FloatValue * AddressLine.ValueScale);
                            Mem.WriteFloat((IntPtr)AddressLine.Address, FloatValue);
                            break;
                        case AddressListValueType.Char4:
                            Mem.WriteChar4((IntPtr)AddressLine.Address, ItemValueString);
                            break;
                    }
                    CurrentItem.SubItems[2].Text = "";
                }
            }
        }

        /************************************************************************/
        /* GUI                                                                  */
        /************************************************************************/
        private void MenuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
                
        private void MenuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("要求还挺高……" + System.Environment.NewLine + System.Environment.NewLine
                + "其实说明都写在软件第1页了，" + System.Environment.NewLine
                + "没啥好解释的。有更多问题来信" + System.Environment.NewLine
                + "吧：tctianchi@163.com",
                "修改器",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void cmdGetAllObjects_Click(object sender, EventArgs e)
        {
            try
            {
                GetAllObject();
            }
            catch (WindowsApi.BadProcessIdException)
            {
                FindGame();
            }
        }

        private void cmdScanGame_Click(object sender, EventArgs e)
        {
            FindGame();
        }

        private void cmdModify_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyModify();

                // Refresh left
                TreeNode SelectedNode = viewFunctions.SelectedNode;
                ITrainerNode FunctionNode = SelectedNode.Tag as ITrainerNode;
                if (FunctionNode != null)
                    RefreshSelectedObject(FunctionNode);

                // Refresh right
                SelectAFunction(SelectedNode);
            }
            catch (WindowsApi.BadProcessIdException)
            {
                FindGame();
            }
        }


        private void viewFunctions_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            // Check whether modification is not saved
            bool IsSaved = true;
            foreach (ListViewItem CurrentItem in viewData.Items)
            {
                if (CurrentItem.SubItems[2].Text != "")
                {
                    IsSaved = false;
                    break;
                }
            }
            if (!IsSaved)
            {
                DialogResult retToSave = MessageBox.Show(
                    "一些修改还没有保存，现在应用这些修改吗？",
                    "保存",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                switch (retToSave)
                {
                    case DialogResult.Yes:
                        cmdModify_Click(this, null);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }

            // Select another function
            try
            {
                SelectAFunction(e.Node);
            }
            catch (WindowsApi.BadProcessIdException)
            {
                FindGame();
            }
        }

        private Brush brushBlack = new SolidBrush(Color.FromKnownColor(KnownColor.ControlText));
        private void SplitterMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString(
                  "没有可修改的项目，" + Environment.NewLine
                + "请在左侧功能列表中" + Environment.NewLine
                + "选择一个修改项。",
                this.Font,
                brushBlack,
                viewData.Location);
        }

        //////////////////////////////////////////////////////////////////////////       
        // Make List view editable
        private void ReplaceInputTextbox()
        {
            if (viewData.SelectedItems.Count < 1)
                return;
            ListViewItem CurrentItem = viewData.SelectedItems[0];

            txtInput.Location = new Point(
                viewData.Columns[0].Width + viewData.Columns[1].Width,
                CurrentItem.Position.Y - 2);
            txtInput.Width = viewData.Columns[2].Width;
        }

        private void viewData_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((Keys)e.KeyChar)
            {
                case Keys.Enter:
                    viewData_MouseUp(sender, null);
                    e.Handled = true;
                    break;
            }
        }

        private void viewData_MouseUp(object sender, MouseEventArgs e)
        {
            if (viewData.SelectedItems.Count < 1)
                return;
            ListViewItem CurrentItem = viewData.SelectedItems[0];

            ReplaceInputTextbox();

            txtInput.Tag = CurrentItem;
            if (CurrentItem.SubItems[2].Text == "")
                txtInput.Text = CurrentItem.SubItems[1].Text;
            else
                txtInput.Text = CurrentItem.SubItems[2].Text;

            txtInput.Visible = true;
            txtInput.Focus();
            txtInput.Select(0, 0);  // Cancle select all
        }

        private void viewData_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ReplaceInputTextbox();
        }

        private void viewData_Scrolling(object sender, EventArgs e)
        {
            viewData.Focus();
        }

        private void txtInput_Leave(object sender, EventArgs e)
        {
            txtInput.Visible = false;
            ListViewItem CurrentItem = txtInput.Tag as ListViewItem;
            if (CurrentItem == null)
                return;

            if (CurrentItem.SubItems[1].Text != txtInput.Text)
                CurrentItem.SubItems[2].Text = txtInput.Text;
            else
                CurrentItem.SubItems[2].Text = "";
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((Keys)e.KeyChar)
            {
                case Keys.Enter:
                    // Commit
                    txtInput_Leave(sender, null);

                    // Move next
                    viewData.Focus();
                    if (viewData.SelectedItems.Count > 0)
                    {
                        int NextIndex = viewData.SelectedItems[0].Index + 1;
                        if (NextIndex < viewData.Items.Count)
                        {
                            viewData.Items[NextIndex].Selected = true;
                            viewData.Items[NextIndex].EnsureVisible();
                            viewData_MouseUp(sender, null);
                        }
                    }
                    
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    // Roll back
                    viewData_MouseUp(sender, null);
                    txtInput_Leave(sender, null);
                    e.Handled = true;
                    break;
            }
        }

        /************************************************************************/
        /* Debug                                                                */
        /************************************************************************/
        private void MenuDebug1_Click(object sender, EventArgs e)
        {
            string strIndex = Microsoft.VisualBasic.Interaction.InputBox(
                "nIndex = 0x?",
                "War3Common.ReadFromGameMemory(nIndex)",
                "0", -1, -1);
            if (strIndex == "")
                return;

            Int32 nIndex;
            Int32.TryParse(
                strIndex,
                System.Globalization.NumberStyles.HexNumber,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out nIndex);

            UInt32 Result = 0;
            try
            {
                using (WindowsApi.clsProcessMemory Mem = new WindowsApi.clsProcessMemory(CurrentGameContext.ProcessID))
                {
                    NewChildrenEventArgs Args = new NewChildrenEventArgs();
                    War3Common.GetGameMemory(
                        CurrentGameContext, ref Args);
                    Result = War3Common.ReadFromGameMemory(
                        Mem, CurrentGameContext, Args,
                        nIndex);
                }
            }
            catch (WindowsApi.BadProcessIdException)
            {
                FindGame();
            }

            MessageBox.Show(
                "0x" + Result.ToString("X"),
                "War3Common.ReadFromGameMemory(0x" + strIndex + ")");
        }
    }
}
