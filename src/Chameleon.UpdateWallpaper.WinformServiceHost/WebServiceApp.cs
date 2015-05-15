using Chameleon.UpdateWallpaper.Service;
using log4net;
using Microsoft.Owin.Hosting;
using System;
using System.IO;
using System.Windows.Forms;
using Chameleon.UpdateWallpaper.WinformServiceHost.Properties;

namespace Chameleon.UpdateWallpaper.WinformServiceHost
{
    internal enum ServiceStatus
    {
        Running,
        Stopped
    }

    internal class WebServiceApp : IDisposable
    {
        private static WebServiceApp _instance;
        public static WebServiceApp Instance
        {
            get
            {
                _instance = _instance ?? new WebServiceApp();
                return _instance;
            }
        }

        private IDisposable _app;
        private ServiceStatus _status;

        public event EventHandler<ServiceStatus> StatusChanged;

        public ServiceStatus Status
        {
            get
            {
                return _status;
            }
            private set
            {
                if (_status != value)
                {
                    if (StatusChanged != null)
                        StatusChanged(this, value);
                }

                _status = value;
            }
        }

        public string ServiceAddress { get; set; }

        private WebServiceApp()
        {
        }

        public void Start(string serviceAddress)
        {
            try
            {
                Dispose();
                _app = WebApp.Start<ChameleonDesktopWallpaperService>(url: serviceAddress);
                Status = ServiceStatus.Running;
                ServiceAddress = serviceAddress;

                NotifyChameleonApp(serviceAddress);
            }
            catch (Exception)
            {
                Status = ServiceStatus.Stopped;
                LogManager.GetLogger("ChameDesktop WebApp").InfoFormat("Failed to start service @ address {0}.", serviceAddress);
            }
        }

        public void Dispose()
        {
            if (_app != null)
            {
                _app.Dispose();
                _app = null;
            }

            Status = ServiceStatus.Stopped;
        }

        private void NotifyChameleonApp(string value)
        {
            // notify local chameleon app service address
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var chameleonPath = Path.Combine(path, @"Packages\30059Jarrey.MagicLockScreen_2cz0jn84nveec");
            var addressFile = Path.Combine(chameleonPath, @"LocalState\address.service");
            if (Directory.Exists(chameleonPath))
            {
                using (var fs = File.Open(addressFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        LogManager.GetLogger("ChameDesktop WebApp").InfoFormat("Inject service address {0}.", value);
                        sw.Write(value);
                    }
                }
            }
            else
            {
                LogManager.GetLogger("ChameDesktop WebApp").InfoFormat("Failed to inject service address.");
                MessageBox.Show(Resources.NotDetectChameleonApp, Resources.NoChameleonInstalled, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
