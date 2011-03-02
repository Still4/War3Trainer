namespace War3Trainer
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.ToolContainer = new System.Windows.Forms.ToolStripContainer();
            this.ToolStripMain = new System.Windows.Forms.ToolStrip();
            this.labGameScanState = new System.Windows.Forms.ToolStripLabel();
            this.cmdScanGame = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuMain = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuDebug1 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSplitor1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.viewFunctions = new System.Windows.Forms.TreeView();
            this.cmdGetAllObjects = new System.Windows.Forms.Button();
            this.cmdModify = new System.Windows.Forms.Button();
            this.txtIntroduction = new System.Windows.Forms.TextBox();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.lblEmpty = new System.Windows.Forms.Label();
            this.viewData = new War3Trainer.ListViewEx();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colOriginalValue = new System.Windows.Forms.ColumnHeader();
            this.colUnsavedValue = new System.Windows.Forms.ColumnHeader();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.ToolContainer.ContentPanel.SuspendLayout();
            this.ToolContainer.TopToolStripPanel.SuspendLayout();
            this.ToolContainer.SuspendLayout();
            this.ToolStripMain.SuspendLayout();
            this.MenuMain.SuspendLayout();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.viewData.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolContainer
            // 
            // 
            // ToolContainer.ContentPanel
            // 
            this.ToolContainer.ContentPanel.Controls.Add(this.ToolStripMain);
            this.ToolContainer.ContentPanel.Size = new System.Drawing.Size(535, 27);
            this.ToolContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.ToolContainer.Location = new System.Drawing.Point(0, 0);
            this.ToolContainer.Name = "ToolContainer";
            this.ToolContainer.Size = new System.Drawing.Size(535, 51);
            this.ToolContainer.TabIndex = 2;
            this.ToolContainer.Text = "toolStripContainer1";
            // 
            // ToolContainer.TopToolStripPanel
            // 
            this.ToolContainer.TopToolStripPanel.Controls.Add(this.MenuMain);
            // 
            // ToolStripMain
            // 
            this.ToolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labGameScanState,
            this.cmdScanGame,
            this.toolStripSeparator1});
            this.ToolStripMain.Location = new System.Drawing.Point(0, 0);
            this.ToolStripMain.Name = "ToolStripMain";
            this.ToolStripMain.Padding = new System.Windows.Forms.Padding(3, 2, 0, 2);
            this.ToolStripMain.Size = new System.Drawing.Size(535, 27);
            this.ToolStripMain.TabIndex = 0;
            this.ToolStripMain.Text = "toolStrip1";
            // 
            // labGameScanState
            // 
            this.labGameScanState.Name = "labGameScanState";
            this.labGameScanState.Size = new System.Drawing.Size(65, 20);
            this.labGameScanState.Text = "游戏未运行";
            // 
            // cmdScanGame
            // 
            this.cmdScanGame.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.cmdScanGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cmdScanGame.Image = ((System.Drawing.Image)(resources.GetObject("cmdScanGame.Image")));
            this.cmdScanGame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cmdScanGame.Name = "cmdScanGame";
            this.cmdScanGame.Size = new System.Drawing.Size(57, 20);
            this.cmdScanGame.Text = "查找游戏";
            this.cmdScanGame.Click += new System.EventHandler(this.cmdScanGame_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // MenuMain
            // 
            this.MenuMain.Dock = System.Windows.Forms.DockStyle.None;
            this.MenuMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.MenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuHelp});
            this.MenuMain.Location = new System.Drawing.Point(0, 0);
            this.MenuMain.Name = "MenuMain";
            this.MenuMain.Size = new System.Drawing.Size(535, 24);
            this.MenuMain.TabIndex = 0;
            this.MenuMain.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuDebug1,
            this.MenuSplitor1,
            this.MenuFileExit});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(59, 20);
            this.MenuFile.Text = "文件(&F)";
            // 
            // MenuDebug1
            // 
            this.MenuDebug1.Name = "MenuDebug1";
            this.MenuDebug1.Size = new System.Drawing.Size(208, 22);
            this.MenuDebug1.Text = "内部-ReadFromGameMemory";
            this.MenuDebug1.Click += new System.EventHandler(this.MenuDebug1_Click);
            // 
            // MenuSplitor1
            // 
            this.MenuSplitor1.Name = "MenuSplitor1";
            this.MenuSplitor1.Size = new System.Drawing.Size(205, 6);
            // 
            // MenuFileExit
            // 
            this.MenuFileExit.Name = "MenuFileExit";
            this.MenuFileExit.Size = new System.Drawing.Size(208, 22);
            this.MenuFileExit.Text = "退出(&X)";
            this.MenuFileExit.Click += new System.EventHandler(this.MenuFileExit_Click);
            // 
            // MenuHelp
            // 
            this.MenuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuHelpAbout});
            this.MenuHelp.Name = "MenuHelp";
            this.MenuHelp.Size = new System.Drawing.Size(59, 20);
            this.MenuHelp.Text = "帮助(&H)";
            // 
            // MenuHelpAbout
            // 
            this.MenuHelpAbout.Name = "MenuHelpAbout";
            this.MenuHelpAbout.Size = new System.Drawing.Size(148, 22);
            this.MenuHelpAbout.Text = "关于修改器(&A)";
            this.MenuHelpAbout.Click += new System.EventHandler(this.MenuHelpAbout_Click);
            // 
            // viewFunctions
            // 
            this.viewFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewFunctions.HideSelection = false;
            this.viewFunctions.Location = new System.Drawing.Point(0, 0);
            this.viewFunctions.Name = "viewFunctions";
            this.viewFunctions.Size = new System.Drawing.Size(165, 341);
            this.viewFunctions.TabIndex = 0;
            this.viewFunctions.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.viewFunctions_BeforeSelect);
            // 
            // cmdGetAllObjects
            // 
            this.cmdGetAllObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdGetAllObjects.Location = new System.Drawing.Point(367, 411);
            this.cmdGetAllObjects.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.cmdGetAllObjects.Name = "cmdGetAllObjects";
            this.cmdGetAllObjects.Size = new System.Drawing.Size(75, 23);
            this.cmdGetAllObjects.TabIndex = 1;
            this.cmdGetAllObjects.Text = "刷新";
            this.cmdGetAllObjects.UseVisualStyleBackColor = true;
            this.cmdGetAllObjects.Click += new System.EventHandler(this.cmdGetAllObjects_Click);
            // 
            // cmdModify
            // 
            this.cmdModify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdModify.Location = new System.Drawing.Point(448, 411);
            this.cmdModify.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.cmdModify.Name = "cmdModify";
            this.cmdModify.Size = new System.Drawing.Size(75, 23);
            this.cmdModify.TabIndex = 2;
            this.cmdModify.Text = "修改";
            this.cmdModify.UseVisualStyleBackColor = true;
            this.cmdModify.Click += new System.EventHandler(this.cmdModify_Click);
            // 
            // txtIntroduction
            // 
            this.txtIntroduction.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtIntroduction.Location = new System.Drawing.Point(17, 14);
            this.txtIntroduction.Multiline = true;
            this.txtIntroduction.Name = "txtIntroduction";
            this.txtIntroduction.ReadOnly = true;
            this.txtIntroduction.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtIntroduction.Size = new System.Drawing.Size(139, 53);
            this.txtIntroduction.TabIndex = 0;
            this.txtIntroduction.Text = resources.GetString("txtIntroduction.Text");
            // 
            // splitMain
            // 
            this.splitMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitMain.Location = new System.Drawing.Point(8, 58);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.viewFunctions);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.lblEmpty);
            this.splitMain.Panel2.Controls.Add(this.viewData);
            this.splitMain.Panel2.Controls.Add(this.txtIntroduction);
            this.splitMain.Size = new System.Drawing.Size(515, 341);
            this.splitMain.SplitterDistance = 165;
            this.splitMain.SplitterWidth = 6;
            this.splitMain.TabIndex = 0;
            // 
            // lblEmpty
            // 
            this.lblEmpty.AutoSize = true;
            this.lblEmpty.Location = new System.Drawing.Point(167, 14);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Size = new System.Drawing.Size(113, 36);
            this.lblEmpty.TabIndex = 1;
            this.lblEmpty.Text = "没有可修改的项目，\r\n请在左侧功能列表中\r\n选择一个修改项。";
            // 
            // viewData
            // 
            this.viewData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colOriginalValue,
            this.colUnsavedValue});
            this.viewData.Controls.Add(this.txtInput);
            this.viewData.FullRowSelect = true;
            this.viewData.GridLines = true;
            this.viewData.HideSelection = false;
            this.viewData.Location = new System.Drawing.Point(17, 73);
            this.viewData.MultiSelect = false;
            this.viewData.Name = "viewData";
            this.viewData.Size = new System.Drawing.Size(302, 134);
            this.viewData.TabIndex = 2;
            this.viewData.UseCompatibleStateImageBehavior = false;
            this.viewData.View = System.Windows.Forms.View.Details;
            this.viewData.Scrolling += new System.EventHandler(this.viewData_Scrolling);
            this.viewData.MouseUp += new System.Windows.Forms.MouseEventHandler(this.viewData_MouseUp);
            this.viewData.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.viewData_KeyPress);
            this.viewData.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.viewData_ColumnWidthChanging);
            // 
            // colName
            // 
            this.colName.Text = "修改项目名称";
            this.colName.Width = 120;
            // 
            // colOriginalValue
            // 
            this.colOriginalValue.Text = "原值";
            this.colOriginalValue.Width = 80;
            // 
            // colUnsavedValue
            // 
            this.colUnsavedValue.Text = "目标数值";
            this.colUnsavedValue.Width = 80;
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(150, 40);
            this.txtInput.Margin = new System.Windows.Forms.Padding(0);
            this.txtInput.MaximumSize = new System.Drawing.Size(32768, 16);
            this.txtInput.MaxLength = 10;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(100, 21);
            this.txtInput.TabIndex = 0;
            this.txtInput.Text = "数值在这里";
            this.txtInput.Visible = false;
            this.txtInput.Leave += new System.EventHandler(this.txtInput_Leave);
            this.txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInput_KeyPress);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(535, 446);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.cmdModify);
            this.Controls.Add(this.cmdGetAllObjects);
            this.Controls.Add(this.ToolContainer);
            this.Name = "FrmMain";
            this.Text = "魔兽3内存修改器 v9";
            this.ToolContainer.ContentPanel.ResumeLayout(false);
            this.ToolContainer.ContentPanel.PerformLayout();
            this.ToolContainer.TopToolStripPanel.ResumeLayout(false);
            this.ToolContainer.TopToolStripPanel.PerformLayout();
            this.ToolContainer.ResumeLayout(false);
            this.ToolContainer.PerformLayout();
            this.ToolStripMain.ResumeLayout(false);
            this.ToolStripMain.PerformLayout();
            this.MenuMain.ResumeLayout(false);
            this.MenuMain.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel2.PerformLayout();
            this.splitMain.ResumeLayout(false);
            this.viewData.ResumeLayout(false);
            this.viewData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer ToolContainer;
        private System.Windows.Forms.MenuStrip MenuMain;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuFileExit;
        private System.Windows.Forms.ToolStripMenuItem MenuHelp;
        private System.Windows.Forms.ToolStripMenuItem MenuHelpAbout;
        private System.Windows.Forms.ToolStrip ToolStripMain;
        private System.Windows.Forms.ToolStripLabel labGameScanState;
        private System.Windows.Forms.TreeView viewFunctions;
        private System.Windows.Forms.Button cmdGetAllObjects;
        private System.Windows.Forms.Button cmdModify;
        private ListViewEx viewData;
        private System.Windows.Forms.ToolStripButton cmdScanGame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colOriginalValue;
        private System.Windows.Forms.ColumnHeader colUnsavedValue;
        private System.Windows.Forms.TextBox txtIntroduction;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.ToolStripMenuItem MenuDebug1;
        private System.Windows.Forms.ToolStripSeparator MenuSplitor1;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Label lblEmpty;
    }
}

