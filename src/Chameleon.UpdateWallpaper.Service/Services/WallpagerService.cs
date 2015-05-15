using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Chameleon.UpdateWallpage.Service.Services
{
    internal class WallpageService
    {
        #region Fields and Imported Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        #endregion

        public static bool SetWallpaper(string path, WallpaperStyle style)
        {
            var logger = LogManager.GetLogger("WallpageService");
            SetWallpaperStyle(style);
            var result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            if (result != 0)
                logger.InfoFormat("Set desktop wallpaper @ {0}, in mode {1}", path, style);
            else
                logger.ErrorFormat("Cannot desktop wallpaper @ {0}, in mode {1}", path, style);
            return result != 0;
        }

        private static void SetWallpaperStyle(WallpaperStyle style)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            switch (style)
            {
                case WallpaperStyle.Stretch:
                    key.SetValue(@"WallpaperStyle", "2");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case WallpaperStyle.Center:
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case WallpaperStyle.Tile:
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "1");
                    break;
                case WallpaperStyle.Fit: // (Windows 7 and later) 
                    key.SetValue(@"WallpaperStyle", "6");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case WallpaperStyle.Fill: // (Windows 7 and later) 
                    key.SetValue(@"WallpaperStyle", "10");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
            }
        }
    }

    internal enum WallpaperStyle : int
    {
        Tile, Center, Stretch, Fit, Fill
    }
}