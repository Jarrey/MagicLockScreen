using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Chameleon.UpdateWallpaper.Service
{
    public class ChameleonDesktopWallpaperService
    {
        /// <summary>
        /// This code configures Web API. The Startup class is specified 
        /// as a type parameter in the WebApp.Start method.
        /// </summary>
        /// <param name="appBuilder">OWIN app builder.</param>
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            appBuilder.UseWebApi(config);
        }
    }
}