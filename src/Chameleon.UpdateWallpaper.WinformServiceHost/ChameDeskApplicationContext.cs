using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chameleon.UpdateWallpaper.WinformServiceHost
{
    internal class ChameDeskApplicationContext : ApplicationContext
    {
        public ChameDeskApplicationContext(Form form)
        {
            form.HandleDestroyed += FormHandleDestroyed;
            form.Closed += FormClosed;
            form.Opacity = 0;
            form.Visible = true;
            Thread.Sleep(500);
            form.Visible = false;
            form.Opacity = 1;
        }

        private void FormClosed(object sender, EventArgs e)
        {
            this.ExitThreadCore();
        }

        private void FormHandleDestroyed(object sender, EventArgs e)
        {
            var form = sender as Form;
            if (form != null && !form.RecreatingHandle)
            {
                form.HandleDestroyed -= FormHandleDestroyed;
                FormClosed(sender, e);
            }
        }
    }
}
