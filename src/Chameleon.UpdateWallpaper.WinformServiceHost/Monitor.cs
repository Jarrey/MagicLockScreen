using Chameleon.UpdateWallpaper.WinformServiceHost.Properties;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Chameleon.UpdateWallpaper.WinformServiceHost
{
    public partial class Monitor : Form
    {
        public Monitor()
        {
            InitializeComponent();

            // set start position
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - 390, Screen.PrimaryScreen.Bounds.Height - 190);

            // Get service status, and initialize control status
            InitializeStatusControls(WebServiceApp.Instance.Status);

            WebServiceApp.Instance.StatusChanged += Instance_StatusChanged;

            // Set auto launch checkbox status
            this.chkLaunchOnSystemStart.Checked = HostSetting.CurrentSetting.AutoLaunch;
        }

        #region Event Handlers

        private void Instance_StatusChanged(object sender, ServiceStatus e)
        {
            InitializeStatusControls(e);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            WebServiceApp.Instance.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            WebServiceApp.Instance.Start(string.Format(Settings.Default.ServiceAddress, NetworkHelper.GetPort()));
        }

        private void monitorIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void Minitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.ExitConfirm, Resources.MessageBoxTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                WebServiceApp.Instance.Dispose();
                Application.Exit();
            }
        }

        private void openMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void lnkLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var rootAppender = ((Hierarchy)LogManager.GetRepository()).Root.Appenders.OfType<FileAppender>().FirstOrDefault();
            if (rootAppender != null && File.Exists(rootAppender.File))
            {
                Process.Start("notepad.exe", rootAppender.File);
            }
        }

        private void chkLaunchOnSystemStart_CheckedChanged(object sender, EventArgs e)
        {
            var chkClt = sender as CheckBox;
            if (chkClt != null)
            {
                if (chkClt.Checked)
                {
                    AutoLaunchRegistryHelper.EnableAutoLaunch();
                }
                else
                {
                    AutoLaunchRegistryHelper.DisableAutoLaunch();
                }

                HostSetting.CurrentSetting.AutoLaunch = chkClt.Checked = AutoLaunchRegistryHelper.GetAutoLaunch();
                HostSetting.SaveSetting();
            }
        }

        private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        #endregion

        private void InitializeStatusControls(ServiceStatus status)
        {
            switch (status)
            {
                case ServiceStatus.Running:
                    this.btnStart.Enabled = false;
                    this.btnStop.Enabled = true;
                    this.monitorIcon.BalloonTipText = this.lblServiceStatus.Text = string.Format("Service is running at {0}", WebServiceApp.Instance.ServiceAddress);
                    this.monitorIcon.ShowBalloonTip(10);
                    this.picStatus.Image = Resources.running;
                    break;
                case ServiceStatus.Stopped:
                    this.btnStart.Enabled = true;
                    this.btnStop.Enabled = false;
                    this.monitorIcon.Text = this.monitorIcon.BalloonTipText = this.lblServiceStatus.Text = "Service is stopped.";
                    this.monitorIcon.ShowBalloonTip(10);
                    this.picStatus.Image = Resources.stopped;
                    break;
            }
        }
    }
}
