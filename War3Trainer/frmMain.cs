using System;
using System.Drawing;
using System.Windows.Forms;

namespace War3Trainer
{
    public partial class FrmMain : Form
    {
        private GameContext _currentGameContext;
        private GameTrainer _mainTrainer;

        public FrmMain()
        {
            InitializeComponent();

            try
            {
                System.Diagnostics.Process.EnterDebugMode();
            }
            catch (Exception ex)
            {
                this.labGameScanState.Text = ex.Message + "请以管理员身份运行";
            }

            FindGame();
            txtIntroduction.Select(0, 0);     // Cancle select all in introduction box
            SetRightGrid(RightFunction.Introduction);
        }

        /************************************************************************/
        /* Main functions                                                       */
        /************************************************************************/
        private void FindGame()
        {
            bool isRecognized = false;
            try
            {
                _currentGameContext = GameContext.FindGameRunning("war3", "game.dll");
                if (_currentGameContext != null)
                {
                    // Game online
                    labGameScanState.Text = "检测到游戏（"
                        + _currentGameContext.ProcessId.ToString()
                        + "），游戏版本 "
                        + _currentGameContext.ProcessVersion;

                    // Get a new trainer
                    GetAllObject();

                    isRecognized = true;
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
                _currentGameContext = null;
                labGameScanState.Text = "检测到游戏（"
                    + e.ProcessId.ToString()
                    + "），但版本（"
                    + e.FileVersion
                    + "）不被支持";
            }
            catch (WindowsApi.BadProcessIdException exception)
            {
                this._currentGameContext = null;
                this.labGameScanState.Text = "错误的进程Id：" + exception.ProcessId.ToString();
            }
            catch
            {
                // Why here?
                _currentGameContext = null;
                labGameScanState.Text = "检测游戏版本时发生严重错误，请重试上一次的操作";
            }

            // Enable buttons
            if (isRecognized)
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
            if (_currentGameContext == null)
                return;
            _mainTrainer = new GameTrainer(_currentGameContext);

            // Create function tree
            viewFunctions.Nodes.Clear();
            foreach (ITrainerNode CurrentFunction in _mainTrainer.GetFunctionList())
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
                SetRightGrid(RightFunction.Introduction);
            }
            else
            {
                // Fill address list
                FillAddressList(Node.NodeIndex);
                
                // Show address list
                if (viewData.Items.Count > 0)
                    SetRightGrid(RightFunction.EditTable);
                else
                    SetRightGrid(RightFunction.Empty);
            }            
        }

        private void FillAddressList(int functionNodeId)
        {
            // To set the right window
            viewData.Items.Clear();
            foreach (IAddressNode addressLine in _mainTrainer.GetAddressList())
            {
                if (addressLine.ParentIndex != functionNodeId)
                    continue;

                viewData.Items.Add(new ListViewItem(
                    new string[]
                    {
                        addressLine.Caption,    // Caption
                        "",                     // Original value
                        ""                      // Modified value
                    }));
                viewData.Items[viewData.Items.Count - 1].Tag = addressLine;
            }

            // To get memory content
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
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
                            ItemValue = mem.ReadInt32((IntPtr)AddressLine.Address)
                                / AddressLine.ValueScale;
                            break;
                        case AddressListValueType.Float:
                            ItemValue = mem.ReadFloat((IntPtr)AddressLine.Address)
                                / AddressLine.ValueScale;
                            break;
                        case AddressListValueType.Char4:
                            ItemValue = mem.ReadChar4((IntPtr)AddressLine.Address);
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
            using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
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
                            Int32 intValue;
                            if (!Int32.TryParse(ItemValueString, out intValue))
                                intValue = 0;
                            intValue = (Int32)(unchecked(intValue * AddressLine.ValueScale));
                            mem.WriteInt32((IntPtr)AddressLine.Address, intValue);
                            break;
                        case AddressListValueType.Float:
                            float floatValue;
                            if (!float.TryParse(ItemValueString, out floatValue))
                                floatValue = 0;
                            floatValue = unchecked(floatValue * AddressLine.ValueScale);
                            mem.WriteFloat((IntPtr)AddressLine.Address, floatValue);
                            break;
                        case AddressListValueType.Char4:
                            mem.WriteChar4((IntPtr)AddressLine.Address, ItemValueString);
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
            bool isSaved = true;
            foreach (ListViewItem CurrentItem in viewData.Items)
            {
                if (CurrentItem.SubItems[2].Text != "")
                {
                    isSaved = false;
                    break;
                }
            }

            // Save all if not saved
            if (!isSaved)
            {
                cmdModify_Click(this, null);
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

        private enum RightFunction
        {
            Empty,
            Introduction,
            EditTable,
        }

        private void SetRightGrid(RightFunction function)
        {
            this.splitMain.Panel2.SuspendLayout();
            this.viewData.SuspendLayout();

            txtIntroduction.Visible = function == RightFunction.Introduction;
            viewData.Visible = function == RightFunction.EditTable;
            lblEmpty.Visible = function == RightFunction.Empty;

            txtIntroduction.Dock = DockStyle.Fill;
            viewData.Dock = DockStyle.Fill;
            lblEmpty.Location = new Point(0, 0);

            this.viewData.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel2.PerformLayout();
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
            if (!Int32.TryParse(
                strIndex,
                System.Globalization.NumberStyles.HexNumber,
                System.Globalization.NumberFormatInfo.InvariantInfo,
                out nIndex))
            {
                nIndex = 0;
            }

            UInt32 Result = 0;
            try
            {
                using (WindowsApi.ProcessMemory mem = new WindowsApi.ProcessMemory(_currentGameContext.ProcessId))
                {
                    NewChildrenEventArgs args = new NewChildrenEventArgs();
                    War3Common.GetGameMemory(
                        _currentGameContext, ref args);
                    Result = War3Common.ReadFromGameMemory(
                        mem, _currentGameContext, args,
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
