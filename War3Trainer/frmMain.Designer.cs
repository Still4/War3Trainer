namespace War3Trainer
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
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
            this.SplitterMain = new System.Windows.Forms.TableLayoutPanel();
            this.viewFunctions = new System.Windows.Forms.TreeView();
            this.cmdGetAllObjects = new System.Windows.Forms.Button();
            this.cmdModify = new System.Windows.Forms.Button();
            this.viewData = new War3Trainer.ucListViewEx();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtIntroduction = new System.Windows.Forms.TextBox();
            this.ToolContainer.ContentPanel.SuspendLayout();
            this.ToolContainer.TopToolStripPanel.SuspendLayout();
            this.ToolContainer.SuspendLayout();
            this.ToolStripMain.SuspendLayout();
            this.MenuMain.SuspendLayout();
            this.SplitterMain.SuspendLayout();
            this.viewData.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolContainer
            // 
            // 
            // ToolContainer.ContentPanel
            // 
            this.ToolContainer.ContentPanel.Controls.Add(this.ToolStripMain);
            this.ToolContainer.ContentPanel.Size = new System.Drawing.Size(538, 27);
            this.ToolContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.ToolContainer.Location = new System.Drawing.Point(0, 0);
            this.ToolContainer.Name = "ToolContainer";
            this.ToolContainer.Size = new System.Drawing.Size(538, 51);
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
            this.ToolStripMain.Size = new System.Drawing.Size(538, 27);
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
            this.MenuMain.Size = new System.Drawing.Size(538, 24);
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
            // SplitterMain
            // 
            this.SplitterMain.ColumnCount = 5;
            this.SplitterMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.SplitterMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.SplitterMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SplitterMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.SplitterMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.SplitterMain.Controls.Add(this.viewFunctions, 0, 0);
            this.SplitterMain.Controls.Add(this.cmdGetAllObjects, 3, 1);
            this.SplitterMain.Controls.Add(this.cmdModify, 4, 1);
            this.SplitterMain.Controls.Add(this.viewData, 2, 0);
            this.SplitterMain.Controls.Add(this.txtIntroduction, 2, 0);
            this.SplitterMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterMain.Location = new System.Drawing.Point(0, 51);
            this.SplitterMain.Name = "SplitterMain";
            this.SplitterMain.Padding = new System.Windows.Forms.Padding(4, 4, 8, 4);
            this.SplitterMain.RowCount = 2;
            this.SplitterMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.SplitterMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.SplitterMain.Size = new System.Drawing.Size(538, 395);
            this.SplitterMain.TabIndex = 0;
            this.SplitterMain.Paint += new System.Windows.Forms.PaintEventHandler(this.SplitterMain_Paint);
            // 
            // viewFunctions
            // 
            this.viewFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewFunctions.HideSelection = false;
            this.viewFunctions.Location = new System.Drawing.Point(7, 7);
            this.viewFunctions.Name = "viewFunctions";
            this.viewFunctions.Size = new System.Drawing.Size(204, 341);
            this.viewFunctions.TabIndex = 0;
            this.viewFunctions.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.viewFunctions_BeforeSelect);
            // 
            // cmdGetAllObjects
            // 
            this.cmdGetAllObjects.Location = new System.Drawing.Point(366, 360);
            this.cmdGetAllObjects.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.cmdGetAllObjects.Name = "cmdGetAllObjects";
            this.cmdGetAllObjects.Size = new System.Drawing.Size(75, 23);
            this.cmdGetAllObjects.TabIndex = 2;
            this.cmdGetAllObjects.Text = "刷新";
            this.cmdGetAllObjects.UseVisualStyleBackColor = true;
            this.cmdGetAllObjects.Click += new System.EventHandler(this.cmdGetAllObjects_Click);
            // 
            // cmdModify
            // 
            this.cmdModify.Location = new System.Drawing.Point(452, 360);
            this.cmdModify.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.cmdModify.Name = "cmdModify";
            this.cmdModify.Size = new System.Drawing.Size(75, 23);
            this.cmdModify.TabIndex = 3;
            this.cmdModify.Text = "修改";
            this.cmdModify.UseVisualStyleBackColor = true;
            this.cmdModify.Click += new System.EventHandler(this.cmdModify_Click);
            // 
            // viewData
            // 
            this.viewData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.SplitterMain.SetColumnSpan(this.viewData, 3);
            this.viewData.Controls.Add(this.txtInput);
            this.viewData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewData.FullRowSelect = true;
            this.viewData.GridLines = true;
            this.viewData.HideSelection = false;
            this.viewData.Location = new System.Drawing.Point(225, 7);
            this.viewData.MultiSelect = false;
            this.viewData.Name = "viewData";
            this.viewData.Size = new System.Drawing.Size(302, 341);
            this.viewData.TabIndex = 1;
            this.viewData.UseCompatibleStateImageBehavior = false;
            this.viewData.View = System.Windows.Forms.View.Details;
            this.viewData.Visible = false;
            this.viewData.Scrolling += new System.EventHandler(this.viewData_Scrolling);
            this.viewData.MouseUp += new System.Windows.Forms.MouseEventHandler(this.viewData_MouseUp);
            this.viewData.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.viewData_KeyPress);
            this.viewData.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.viewData_ColumnWidthChanging);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "修改项目名称";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "原值";
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "目标数值";
            this.columnHeader3.Width = 80;
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
            // txtIntroduction
            // 
            this.txtIntroduction.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SplitterMain.SetColumnSpan(this.txtIntroduction, 3);
            this.txtIntroduction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIntroduction.Location = new System.Drawing.Point(7, 354);
            this.txtIntroduction.Multiline = true;
            this.txtIntroduction.Name = "txtIntroduction";
            this.txtIntroduction.ReadOnly = true;
            this.txtIntroduction.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtIntroduction.Size = new System.Drawing.Size(353, 34);
            this.txtIntroduction.TabIndex = 4;
            this.txtIntroduction.Text = resources.GetString("txtIntroduction.Text");
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 446);
            this.Controls.Add(this.SplitterMain);
            this.Controls.Add(this.ToolContainer);
            this.Name = "frmMain";
            this.Text = "魔兽3内存修改器 v8";
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
            this.SplitterMain.ResumeLayout(false);
            this.SplitterMain.PerformLayout();
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
        private System.Windows.Forms.TableLayoutPanel SplitterMain;
        private System.Windows.Forms.TreeView viewFunctions;
        private System.Windows.Forms.Button cmdGetAllObjects;
        private System.Windows.Forms.Button cmdModify;
        private ucListViewEx viewData;
        private System.Windows.Forms.ToolStripButton cmdScanGame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox txtIntroduction;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.ToolStripMenuItem MenuDebug1;
        private System.Windows.Forms.ToolStripSeparator MenuSplitor1;
    }
}

