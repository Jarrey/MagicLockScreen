namespace Chameleon.UpdateWallpaper.WinformServiceHost
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using Chameleon.UpdateWallpaper.WinformServiceHost.Properties;

    using log4net;

    public static class Program
    {
        private static Mutex mutex = new Mutex(false, "Chameleon.DesktopService.WinformHost");

        private static readonly ILog logger = LogManager.GetLogger("ChameDesktop");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (!mutex.WaitOne(TimeSpan.FromMilliseconds(500), false))
            {
                return;
            }

            try
            {
                Application.ApplicationExit += Application_ApplicationExit;
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                // Initialize log
                log4net.Config.XmlConfigurator.Configure();

                // Read default setting for monitor program
                HostSetting.CurrentSetting.AutoLaunch = AutoLaunchRegistryHelper.GetAutoLaunch();
                HostSetting.SaveSetting();

                WebServiceApp.Instance.Start(string.Format(Settings.Default.ServiceAddress, NetworkHelper.GetPort()));
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var form = new Monitor())
                {
                    Application.Run(new ChameDeskApplicationContext(form));
                }
            }
            finally
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error("Current AppDomain UnhandledException", e.ExceptionObject as Exception);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            logger.Error("Thread UnhandledException", e.Exception);
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            logger.Info("Exit host");
            WebServiceApp.Instance.Dispose();
        }
    }
}
