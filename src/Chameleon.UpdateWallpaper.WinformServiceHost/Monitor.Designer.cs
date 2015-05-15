namespace Chameleon.UpdateWallpaper.WinformServiceHost
{
    partial class Monitor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Monitor));
            this.monitorIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkLaunchOnSystemStart = new System.Windows.Forms.CheckBox();
            this.lnkAbout = new System.Windows.Forms.LinkLabel();
            this.lblServiceStatus = new System.Windows.Forms.Label();
            this.lnkLog = new System.Windows.Forms.LinkLabel();
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // monitorIcon
            // 
            this.monitorIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(this.monitorIcon, "monitorIcon");
            this.monitorIcon.ContextMenuStrip = this.contextMenuStrip;
            this.monitorIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.monitorIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMonitorToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // openMonitorToolStripMenuItem
            // 
            this.openMonitorToolStripMenuItem.Name = "openMonitorToolStripMenuItem";
            resources.ApplyResources(this.openMonitorToolStripMenuItem, "openMonitorToolStripMenuItem");
            this.openMonitorToolStripMenuItem.Click += new System.EventHandler(this.openMonitorToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::Chameleon.UpdateWallpaper.WinformServiceHost.Properties.Resources.exit;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // chkLaunchOnSystemStart
            // 
            resources.ApplyResources(this.chkLaunchOnSystemStart, "chkLaunchOnSystemStart");
            this.chkLaunchOnSystemStart.Name = "chkLaunchOnSystemStart";
            this.chkLaunchOnSystemStart.UseVisualStyleBackColor = true;
            this.chkLaunchOnSystemStart.CheckedChanged += new System.EventHandler(this.chkLaunchOnSystemStart_CheckedChanged);
            // 
            // lnkAbout
            // 
            resources.ApplyResources(this.lnkAbout, "lnkAbout");
            this.lnkAbout.Name = "lnkAbout";
            this.lnkAbout.TabStop = true;
            this.lnkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAbout_LinkClicked);
            // 
            // lblServiceStatus
            // 
            resources.ApplyResources(this.lblServiceStatus, "lblServiceStatus");
            this.lblServiceStatus.Name = "lblServiceStatus";
            // 
            // lnkLog
            // 
            resources.ApplyResources(this.lnkLog, "lnkLog");
            this.lnkLog.Name = "lnkLog";
            this.lnkLog.TabStop = true;
            this.lnkLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLog_LinkClicked);
            // 
            // picStatus
            // 
            resources.ApplyResources(this.picStatus, "picStatus");
            this.picStatus.Name = "picStatus";
            this.picStatus.TabStop = false;
            // 
            // btnStop
            // 
            this.btnStop.Image = global::Chameleon.UpdateWallpaper.WinformServiceHost.Properties.Resources.stop;
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.Name = "btnStop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Image = global::Chameleon.UpdateWallpaper.WinformServiceHost.Properties.Resources.start;
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.Name = "btnStart";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnExit
            // 
            this.btnExit.Image = global::Chameleon.UpdateWallpaper.WinformServiceHost.Properties.Resources.exit;
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.Name = "btnExit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // Monitor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lnkLog);
            this.Controls.Add(this.picStatus);
            this.Controls.Add(this.lblServiceStatus);
            this.Controls.Add(this.lnkAbout);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.chkLaunchOnSystemStart);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Monitor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Minitor_FormClosing);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon monitorIcon;
        private System.Windows.Forms.CheckBox chkLaunchOnSystemStart;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.LinkLabel lnkAbout;
        private System.Windows.Forms.Label lblServiceStatus;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.LinkLabel lnkLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button btnExit;
    }
}

